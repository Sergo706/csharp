using DocsParser.Models;
using DocsParser.Services;
using DocsParser.Services.Loggers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DocsParser.Controllers;

[ApiController]

[Route("api/[controller]")]
public class CustomAuthControllerController(IAppLogger appLogger, AccountsService accountsService, UserManager<AppUser> userManager) : ControllerBase
{

    private readonly IAppLogger _applogger = appLogger;
    private readonly AccountsService _accountsService = accountsService;
    private readonly UserManager<AppUser> _userManager = userManager;


    [HttpPost("auth/register-custom")]
    public async Task<IResult> Register([FromForm] CustomRegisterDto input)
    {
        try
        {
            var result = await _accountsService.AddNewUser(input);
            if (result.Succeeded)
            {
                return Results.Created();
            }
            return Results.BadRequest(result.Errors);
        }
        catch (Exception err)
        {
            _applogger.AppLogger.Error(err, "Error Creating user account!");
            return Results.InternalServerError("Server Error. Please try again later.");
        }
    }

    [HttpGet("auth/profile")]
    [Authorize]
    public async Task<IResult> GetUserProfile()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Results.Unauthorized();
        try
        {
            var profile = await _accountsService.GetUserProfile(userId);
            if (profile == null)
            {
                _applogger.AppLogger.Warning("Cannot get authenticated user profile!");
                return Results.NotFound();
            }
            return Results.Ok(profile);
        }
        catch (Exception err)
        {
            _applogger.AppLogger.Error(err, "Error getting user profile!");
            return Results.InternalServerError("Server Error. Please try again later.");
        }

    }
}
