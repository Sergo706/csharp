using DocsParser.Models;
using Microsoft.AspNetCore.Identity;

namespace DocsParser.Services;

public class AccountsService(AppDbContext context, UserManager<AppUser> userManager)
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
        return await userManager.CreateAsync(user, data.Password);
    }
    public async Task GetUserProfile(string userId)
    {
        
    }
}
