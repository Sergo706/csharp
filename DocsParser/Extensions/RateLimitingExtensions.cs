using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
namespace DocsParser.Extensions;

public static class RateLimitPolicies
{
    public const string Auth = "auth";
    public const string Conversion = "conversion";
}

public static class RateLimitingExtensions
{
    public static IServiceCollection AddAppRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode =
                StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter =
                PartitionedRateLimiter.Create<HttpContext, string>(
                    context => CreatePartition(
                        context,
                        permitLimit: 50,
                        window: TimeSpan.FromMinutes(1)));

            options.AddPolicy(
                RateLimitPolicies.Auth,
                context => CreatePartition(
                    context,
                    permitLimit: 10,
                    window: TimeSpan.FromMinutes(1)));

            options.AddPolicy(
                RateLimitPolicies.Conversion,
                context => CreatePartition(
                    context,
                    permitLimit: 5,
                    window: TimeSpan.FromMinutes(1)));
        });

        return services;
    }

    private static RateLimitPartition<string> CreatePartition(
        HttpContext context,
        int permitLimit,
        TimeSpan window)
    {
        string? userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        string ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        string key = userId is not null ? $"user:{userId}" : $"ip:{ipAddress}";

        return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = permitLimit,
                QueueLimit = 0,
                Window = window
            });
    }
}