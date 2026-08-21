using DocsParser.Services.Convertor;
using DocsParser.Services.Loggers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using DocsParser.Models;
using System.Text.RegularExpressions;
using System.Security.Claims;
using DocsParser.Services;
using DocsParser.Extensions;
using Microsoft.AspNetCore.RateLimiting;

namespace DocsParser.Controllers;

[ApiController]
[Route("api/documents")]
[Consumes("multipart/form-data")]
public class DocumentController(Convertor converter, IAppLogger appLogger, DocumentService documentService) : ControllerBase
{
    private readonly Convertor _converter = converter;
    private readonly IAppLogger _appLogger = appLogger;
    private readonly DocumentService _documentService = documentService;
    private string? UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    private static string SanitizeFileName(string name, int maxBaseLength = 100)
    {
        var baseName = Path.GetFileNameWithoutExtension(name) ?? "file";
        baseName = Regex.Replace(baseName, @"[^\w\-\.]", "_"); 
        
        if (baseName.Length > maxBaseLength) baseName = baseName.Substring(0, maxBaseLength);
        var ext = Path.GetExtension(name).TrimStart('.');
        return $"{baseName}.{ext}";
    }

    [HttpPost("convert")]
    [EnableRateLimiting(RateLimitPolicies.Conversion)]
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
            if (!string.IsNullOrWhiteSpace(UserId))
            {
                    try
                    {
                        await _documentService.AddDocumentHistory(outFileName, UserId,  target, ext);
                    }
                    catch (Exception ex)
                    {
                      _appLogger.AppLogger.Error(ex, "failed to save history conversion for authenticated user");
                    }
            };

            return File(convertedBytes, mimeType, outFileName);
        }
        catch (Exception ex)
        {
            _appLogger.AppLogger.Error(ex, "Conversion failed for file {Filename} from {Ext} to {Target}", filename, ext, target);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while converting the file.");
        }
    }
} 
