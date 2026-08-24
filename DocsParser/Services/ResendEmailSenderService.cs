using DocsParser.Models;
using Microsoft.AspNetCore.Identity;
using Resend;
using Microsoft.Extensions.DependencyInjection;

namespace DocsParser.Services;

public class ResendEmailSenderService(IServiceProvider serviceProvider) : IEmailSender<AppUser>
{
    private readonly string _fromEmail = "noreply@riavzon.com";

    public async Task SendConfirmationLinkAsync(AppUser user, string email, string confirmationLink)
    {
        var resend = serviceProvider.GetRequiredService<IResend>();
        var message = new EmailMessage
        {
            From = _fromEmail,
            To = email,
            Subject = "Confirm your email",
            HtmlBody = $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>."
        };

        await resend.EmailSendAsync(message);
    }

    public async Task SendPasswordResetLinkAsync(AppUser user, string email, string resetLink)
    {
        var resend = serviceProvider.GetRequiredService<IResend>();
        var message = new EmailMessage
        {
            From = _fromEmail,
            To = email,
            Subject = "Reset your password",
            HtmlBody = $"Please reset your password by <a href='{resetLink}'>clicking here</a>."
        };

        await resend.EmailSendAsync(message);
    }

    public async Task SendPasswordResetCodeAsync(AppUser user, string email, string resetCode)
    {
        var resend = serviceProvider.GetRequiredService<IResend>();
        var message = new EmailMessage
        {
            From = _fromEmail,
            To = email,
            Subject = "Reset your password",
            HtmlBody = $"Please reset your password using the following code: {resetCode}"
        };

        await resend.EmailSendAsync(message);
    }
}
