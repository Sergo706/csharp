using DocsParser.Models;
using DocsParser.Services.Loggers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace DocsParser.Controllers;

[ApiController]

[Route("api/[controller]")]
public class CustomAuthControllerController(IAppLogger appLogger) : ControllerBase
{

    private readonly IAppLogger _applogger = appLogger;
    
    [HttpPost("auth/register-custom")]
    public IActionResult Get()
    {
        return Ok();
    }
}
