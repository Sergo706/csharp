using System.Text;
using System.Web;
using DocsParser.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace DocsParser.Services;

public class AccountsService(UserManager<AppUser> userManager, IEmailSender<AppUser> emailSender, IConfiguration config)
{

    public async Task<IdentityResult> AddNewUser(CustomRegisterDto data)
    {
        var user = new AppUser
        {
            UserName = data.Email,
            Email = data.Email,
            Name = data.Name,
            LastName = data.LastName,
        };
        var result = await userManager.CreateAsync(user, data.Password);

        if (result.Succeeded)
        {
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var frontendUrl = config["Frontend:DashboardUrl"]?.Replace("/dashboard", "") ?? "http://localhost:3000";
            var builder = new UriBuilder($"{frontendUrl}/confirm-email");
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["userId"] = user.Id;
            query["code"] = code;
            builder.Query = query.ToString();

            var confirmationLink = builder.ToString();

            await emailSender.SendConfirmationLinkAsync(user, data.Email, confirmationLink);
        }

        return result;
    }
    public async Task<UserProfileDto?> GetUserProfile(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return null;

        return new UserProfileDto(
            Name: user.Name,
            LastName: user.LastName,
            AvatarUrl: user.AvatarUrl,
            CreatedAt: user.CreatedAt
        );
    }
}
