using System.Diagnostics;
using System.ComponentModel;

namespace DocsParser.Services.Convertor;


public class Convertor(IConfiguration configuration)
{
    private readonly string _markItDownExecutable =
        configuration["DocumentConversion:MarkItDownExecutable"] ?? "markitdown";

    private static string GetPdfInputFilter(string targetFormat)
    {
        return targetFormat switch
        {
            "docx" => "writer_pdf_import",
            "pptx" => "impress_pdf_import",
            "jpg" => "draw_pdf_import",
            _ => throw new NotSupportedException(
                $"Conversion from PDF to '{targetFormat}' is not supported.")
        };
    }

    private static string GetLibreOfficeOutputFilter(string inputExtension, string targetFormat)
    {
        if (inputExtension != "md")
        {
            return targetFormat;
        }

        return targetFormat switch
        {
            "pdf" => "pdf:writer_pdf_Export",
            "docx" => "docx:Office Open XML Text",
            "html" => "html:HTML (StarWriter)",
            _ => throw new NotSupportedException(
                $"Conversion from Markdown to '{targetFormat}' is not supported.")
        };
    }

    private static async Task<(int ExitCode, string StandardOutput, string StandardError)> RunProcessAsync(
        ProcessStartInfo startInfo,
        TimeSpan timeout,
        string processName)
    {
        using var process = new Process { StartInfo = startInfo };

        try
        {
            process.Start();
        }
        catch (Win32Exception ex)
        {
            throw new InvalidOperationException(
                $"Could not start {processName}. Verify that '{startInfo.FileName}' is installed and available on PATH.",
                ex);
        }

        Task<string> readOutputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> readErrorTask = process.StandardError.ReadToEndAsync();
        using var cts = new CancellationTokenSource(timeout);

        try
        {
            await process.WaitForExitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                await process.WaitForExitAsync();
            }

            throw new TimeoutException($"{processName} document conversion timed out.");
        }

        return (
            process.ExitCode,
            await readOutputTask,
            await readErrorTask
        );
    }

    private static async Task<byte[]> ConvertPdfToHtml(
        string inputFilePath,
        string outputFilePath)
    {
        if (Path.GetExtension(inputFilePath) != ".pdf")
        {
            throw new InvalidOperationException("The input is not a PDF file.");
        }

        string outputDirectory = Path.GetDirectoryName(outputFilePath)
            ?? throw new InvalidOperationException("The HTML output directory is invalid.");

        var startInfo = new ProcessStartInfo
        {
            FileName = "pdf2htmlEX",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add("--embed-css");
        startInfo.ArgumentList.Add("1");
        startInfo.ArgumentList.Add("--embed-font");
        startInfo.ArgumentList.Add("1");
        startInfo.ArgumentList.Add("--embed-image");
        startInfo.ArgumentList.Add("1");
        startInfo.ArgumentList.Add("--embed-javascript");
        startInfo.ArgumentList.Add("1");
        startInfo.ArgumentList.Add("--embed-outline");
        startInfo.ArgumentList.Add("1");
        startInfo.ArgumentList.Add("--embed-external-font");
        startInfo.ArgumentList.Add("1");
        startInfo.ArgumentList.Add("--split-pages");
        startInfo.ArgumentList.Add("0");
        startInfo.ArgumentList.Add("--zoom");
        startInfo.ArgumentList.Add("1.4");
        startInfo.ArgumentList.Add("--dpi");
        startInfo.ArgumentList.Add("200");
        startInfo.ArgumentList.Add("--dest-dir");
        startInfo.ArgumentList.Add(outputDirectory);
        startInfo.ArgumentList.Add(inputFilePath);
        startInfo.ArgumentList.Add(Path.GetFileName(outputFilePath));

        var result = await RunProcessAsync(
            startInfo,
            TimeSpan.FromSeconds(60),
            "pdf2htmlEX");

        if (result.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"pdf2htmlEX conversion failed. {result.StandardError}");
        }

        if (!File.Exists(outputFilePath))
        {
            throw new FileNotFoundException(
                "pdf2htmlEX succeeded, but the HTML file is missing.",
                outputFilePath);
        }

        return await File.ReadAllBytesAsync(outputFilePath);
    }

    private async Task<byte[]> ConvertToMarkdownAsync(
        string inputFilePath,
        string outputFilePath)
    {
        string markItDownInputPath = inputFilePath;

        if (Path.GetExtension(inputFilePath) == ".pdf")
        {
            string outputDirectory = Path.GetDirectoryName(outputFilePath)
                ?? throw new InvalidOperationException("The Markdown output directory is invalid.");

            string intermediateDirectory = Path.Combine(outputDirectory, "pdf-to-docx");
            string profileDirectory = Path.Combine(outputDirectory, "pdf-to-docx-profile");

            Directory.CreateDirectory(intermediateDirectory);
            Directory.CreateDirectory(profileDirectory);

            string intermediateDocxPath = Path.Combine(
                intermediateDirectory,
                $"{Path.GetFileNameWithoutExtension(inputFilePath)}.docx");

            var libreOfficeStartInfo = new ProcessStartInfo
            {
                FileName = "soffice",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            libreOfficeStartInfo.ArgumentList.Add(
                $"-env:UserInstallation={new Uri(profileDirectory).AbsoluteUri}");
            libreOfficeStartInfo.ArgumentList.Add($"--infilter={GetPdfInputFilter("docx")}");
            libreOfficeStartInfo.ArgumentList.Add("--headless");
            libreOfficeStartInfo.ArgumentList.Add("--convert-to");
            libreOfficeStartInfo.ArgumentList.Add("docx:Office Open XML Text");
            libreOfficeStartInfo.ArgumentList.Add("--outdir");
            libreOfficeStartInfo.ArgumentList.Add(intermediateDirectory);
            libreOfficeStartInfo.ArgumentList.Add(inputFilePath);

            var libreOfficeResult = await RunProcessAsync(
                libreOfficeStartInfo,
                TimeSpan.FromSeconds(60),
                "LibreOffice PDF-to-DOCX");

            if (libreOfficeResult.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"LibreOffice PDF-to-DOCX conversion failed " +
                    $"(Exit Code {libreOfficeResult.ExitCode}). " +
                    $"Error: {libreOfficeResult.StandardError}. " +
                    $"Output: {libreOfficeResult.StandardOutput}");
            }

            if (!File.Exists(intermediateDocxPath))
            {
                throw new FileNotFoundException(
                    "LibreOffice succeeded, but the intermediate DOCX file is missing.",
                    intermediateDocxPath);
            }

            markItDownInputPath = intermediateDocxPath;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = _markItDownExecutable,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add(markItDownInputPath);
        startInfo.ArgumentList.Add("-o");
        startInfo.ArgumentList.Add(outputFilePath);

        var result = await RunProcessAsync(
            startInfo,
            TimeSpan.FromSeconds(60),
            "MarkItDown");

        if (result.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"MarkItDown conversion failed (Exit Code {result.ExitCode}). " +
                $"Error: {result.StandardError}. Output: {result.StandardOutput}");
        }

        if (!File.Exists(outputFilePath))
        {
            throw new FileNotFoundException(
                "MarkItDown succeeded, but the Markdown output file is missing.",
                outputFilePath);
        }

        return await File.ReadAllBytesAsync(outputFilePath);
    }

    public async Task<byte[]> ConvertDocumentAsync(Stream inputStream, string inputExtension, string targetFormat)
    {
        inputExtension = inputExtension.TrimStart('.').ToLowerInvariant();
        targetFormat = targetFormat.TrimStart('.').ToLowerInvariant();

        string workDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        string inputFilePath = Path.Combine(workDir, $"input.{inputExtension}");
        string outputDir = Path.Combine(workDir, "out");
        string profileDir = Path.Combine(workDir, "profile");

        Directory.CreateDirectory(workDir);
        Directory.CreateDirectory(outputDir);
        Directory.CreateDirectory(profileDir);

        try
        {
            await using (var fileStream = new FileStream(inputFilePath, FileMode.Create))
            {
                await inputStream.CopyToAsync(fileStream);
            }

            if (targetFormat == "md")
            {
                string markdownOutputPath = Path.Combine(outputDir, "input.md");
                return await ConvertToMarkdownAsync(inputFilePath, markdownOutputPath);
            }

            if (inputExtension == "pdf" && targetFormat == "html")
            {
                string htmlOutputPath = Path.Combine(outputDir, "input.html");
                return await ConvertPdfToHtml(inputFilePath, htmlOutputPath);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "soffice",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            startInfo.ArgumentList.Add($"-env:UserInstallation={new Uri(profileDir).AbsoluteUri}");

            if (inputExtension == "pdf")
            {
                startInfo.ArgumentList.Add($"--infilter={GetPdfInputFilter(targetFormat)}");
            }
            else if (inputExtension == "md")
            {
                startInfo.ArgumentList.Add("--infilter=Markdown");
            }
            else if (targetFormat != "pdf")
            {
                throw new NotSupportedException(
                    $"Conversion from '{inputExtension}' to '{targetFormat}' is not supported.");
            }

            startInfo.ArgumentList.Add("--headless");
            startInfo.ArgumentList.Add("--convert-to");
            startInfo.ArgumentList.Add(GetLibreOfficeOutputFilter(inputExtension, targetFormat));
            startInfo.ArgumentList.Add("--outdir");
            startInfo.ArgumentList.Add(outputDir);
            startInfo.ArgumentList.Add(inputFilePath);

            var result = await RunProcessAsync(
                startInfo,
                TimeSpan.FromSeconds(30),
                "LibreOffice");

            if (result.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"LibreOffice conversion failed (Exit Code {result.ExitCode}). " +
                    $"Error: {result.StandardError}. Output: {result.StandardOutput}");
            }

            string outputFilePath = Path.Combine(outputDir, $"input.{targetFormat}");
            if (!File.Exists(outputFilePath))
                throw new FileNotFoundException("LibreOffice succeeded, but output file is missing.");

            return await File.ReadAllBytesAsync(outputFilePath);
        }
        finally
        {
            if (Directory.Exists(workDir))
            {
                try { Directory.Delete(workDir, true); }
                catch { }
            }
        }
    }
}
