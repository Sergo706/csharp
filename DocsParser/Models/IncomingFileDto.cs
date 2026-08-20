

using System.ComponentModel.DataAnnotations;
namespace DocsParser.Models;

public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly long _maxFileSizeMb;
    public MaxFileSizeAttribute(long maxFileSizeMb)
    {
        _maxFileSizeMb = maxFileSizeMb;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            double fileSizeMegaBytes = (double)file.Length / 1024 / 1024;
            if (fileSizeMegaBytes > _maxFileSizeMb)
            {
                return new ValidationResult($"File size exceeds the allowed limit of {_maxFileSizeMb} MB.");
            }
        }
        return ValidationResult.Success;
    }
};

public record FileUploadDto
{
    private static readonly HashSet<string> PdfToSupportedTarget =
        ["docx", "jpg", "pptx", "html"];

    private static readonly HashSet<string> SupportedSourceToPdf =
        ["md", "docx", "csv", "xls", "xlsx", "ods", "jpg", "pptx", "html"];

    private static readonly HashSet<string> MarkItDownSourceToMarkdown =
        ["pdf", "docx", "csv", "xls", "xlsx", "pptx", "html"];

    private static readonly HashSet<string> MarkdownToLibreOfficeTarget =
        ["pdf", "docx", "html"];

    [Required]
    [MaxFileSize(10)]
    public required IFormFile File { get; set; }

    [Required]
    [StringLength(30)]
    public required string TargetFormat { get; set; }

    public static bool IsSupportedInput(string extension)
    {
        extension = extension.TrimStart('.').ToLowerInvariant();
        return extension == "pdf" ||
               SupportedSourceToPdf.Contains(extension) ||
               MarkItDownSourceToMarkdown.Contains(extension);
    }

    public bool IsSupported(string convertTo, string from)
    {
        convertTo = convertTo.TrimStart('.').ToLowerInvariant();
        from = from.TrimStart('.').ToLowerInvariant();

        if (convertTo == "md")
        {
            return MarkItDownSourceToMarkdown.Contains(from);
        }

        if (from == "md")
        {
            return MarkdownToLibreOfficeTarget.Contains(convertTo);
        }

        if (from == "pdf")
        {
            return PdfToSupportedTarget.Contains(convertTo);
        }

        return convertTo == "pdf" && SupportedSourceToPdf.Contains(from);
    }

    public void Deconstruct(
        out string targetFormat,
        out IFormFile file
   )
    {
        targetFormat = TargetFormat;
        file = File;
    }
}
