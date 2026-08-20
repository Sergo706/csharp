using DocsParser.Services.Convertor;
using DocsParser.Services.Loggers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using DocsParser.Models;
using System.Text.RegularExpressions;

namespace DocsParser.Controllers;

[ApiController]
[Route("api/documents")]
[Consumes("multipart/form-data")]
public class DocumentController(Convertor converter, IAppLogger appLogger) : ControllerBase
{
    private readonly Convertor _converter = converter;
    private readonly IAppLogger _appLogger = appLogger;
    private static string SanitizeFileName(string name, int maxBaseLength = 100)
    {
        var baseName = Path.GetFileNameWithoutExtension(name) ?? "file";
        baseName = Regex.Replace(baseName, @"[^\w\-\.]", "_"); 
        
        if (baseName.Length > maxBaseLength) baseName = baseName.Substring(0, maxBaseLength);
        var ext = Path.GetExtension(name).TrimStart('.');
        return $"{baseName}.{ext}";
    }

    [HttpPost("convert")]
    public async Task<IActionResult> ConvertFile([FromForm] FileUploadDto dto)
    {
        if (dto.File.Length == 0)
        {
            return BadRequest("File is empty.");
        }

        var (targetFormat, uploadedFile) = dto;
        string filename = Path.GetFileName(uploadedFile.FileName);
        string ext = Path.GetExtension(filename).TrimStart('.').ToLowerInvariant();

        if (!FileUploadDto.IsSupportedInput(ext))
        {
            return BadRequest("Input file type is not allowed.");
        }

        string target = targetFormat.Trim().TrimStart('.').ToLowerInvariant();
        if (!dto.IsSupported(target, ext))
        {
            return BadRequest($"Conversion from '{ext}' to '{target}' is not supported.");
        }

        string safeFileName = SanitizeFileName(filename);
        var baseSafe = Path.GetFileNameWithoutExtension(safeFileName);
        string outFileName = $"{baseSafe}_{DateTime.UtcNow:yyyyMMddHHmmss}.{target}";

        var provider = new FileExtensionContentTypeProvider();
        if (provider.TryGetContentType(filename, out var expectedInputContentType) &&
            !string.IsNullOrWhiteSpace(uploadedFile.ContentType) &&
            !uploadedFile.ContentType.StartsWith(
                expectedInputContentType.Split('/')[0],
                StringComparison.OrdinalIgnoreCase))
        {
            _appLogger.AppLogger.Warning(
                "Uploaded content-type {UploadedContentType} does not match expected input content type {ExpectedInputContentType} for file {Filename}",
                uploadedFile.ContentType,
                expectedInputContentType,
                filename);
            return Forbid();
        }

        using var stream = uploadedFile.OpenReadStream();
        try
        {
            byte[] convertedBytes = await _converter.ConvertDocumentAsync(stream, ext, target);

            if (!provider.TryGetContentType(outFileName, out var detectedContentType))
                detectedContentType = "application/octet-stream";

            string mimeType = detectedContentType;

            return File(convertedBytes, mimeType, outFileName);
        }
        catch (Exception ex)
        {
            _appLogger.AppLogger.Error(ex, "Conversion failed for file {Filename} from {Ext} to {Target}", filename, ext, target);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while converting the file.");
        }
    }
} 
