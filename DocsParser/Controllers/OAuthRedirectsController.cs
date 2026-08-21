
using System.Security.Claims;
using AspNet.Security.OAuth.GitHub;
using DocsParser.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using DocsParser.Extensions;

namespace DocsParser.Controllers;

[Route("api/auth")]
[ApiController]
[EnableRateLimiting(RateLimitPolicies.Auth)]

public class OAuthRedirects(
    SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager,
    IConfiguration configuration
    ) : ControllerBase
{
    private readonly SignInManager<AppUser> _signInManager = signInManager;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly string _dashboardUrl = configuration["Frontend:DashboardUrl"] ?? "http://localhost:3000/dashboard";

    [HttpGet("login/{provider}")]
    public IResult StartFlow(string provider)
    {
        var scheme = provider.ToLowerInvariant() switch
        {
            "google" => GoogleDefaults.AuthenticationScheme,
            "github" => GitHubAuthenticationDefaults.AuthenticationScheme,
            _ => null
        };
        if (scheme is null) return Results.NotFound();


        var properties = new AuthenticationProperties
        {
            RedirectUri = $"/api/auth/{provider}-success"
        };

        return Results.Challenge(
            properties,
            authenticationSchemes: [scheme]);
    }
    [HttpGet("{provider}-success")]
    public async Task<IResult> SuccessCallBack(string provider)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null) return Results.Unauthorized();
   

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);
        if (result.Succeeded) return Results.Redirect(_dashboardUrl);

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
            return Results.BadRequest("An email address is required from the OAuth provider.");
        
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new AppUser
            {
                Name = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? info.Principal.FindFirstValue(ClaimTypes.Name) ?? "Unknown",
                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "Unknown",
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                AvatarUrl = info.Principal.FindFirstValue("avatar")
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded) return Results.BadRequest("Failed to create user.");
        }
        var linkResult = await _userManager.AddLoginAsync(user, info);
        if (!linkResult.Succeeded) return Results.BadRequest("Failed to link OAuth account.");

        await _signInManager.SignInAsync(user, isPersistent: true);

        return Results.Redirect(_dashboardUrl);
    }
}
