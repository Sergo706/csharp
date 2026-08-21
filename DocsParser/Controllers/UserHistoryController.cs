

using System.Security.Claims;
using DocsParser.Services;
using DocsParser.Services.Loggers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocsParser.Controllers;

[ApiController]
[Route("api/history")]
[Authorize]
public class AccountHistoryController(IAppLogger appLogger, DocumentService documentService) : ControllerBase
{
    private readonly IAppLogger _appLogger = appLogger;
    private readonly DocumentService _documentService = documentService;

    private string? UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    [HttpGet] 
    public async Task<IActionResult> GetHistory()
    {
        if (UserId == null) return Unauthorized();

        try
        {
            var data = await _documentService.GetAllDocumentHistory(UserId);
            return Ok(data);
        } catch (Exception err)
        {
            _appLogger.AppLogger.Error("Error getting data", err);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while converting the file.");
        }
    }
}
