using Serilog;
using Microsoft.AspNetCore.HttpLogging;
using DocsParser.Services.Loggers;
using DocsParser.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using DocsParser.Services.Convertor;
using DocsParser.Extensions;

var builder = WebApplication.CreateBuilder(args);
var loggers = new Loggers();
builder.Services.AddSingleton<IAppLogger>(loggers);
builder.Services.AddScoped<Convertor>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAntiforgery();
builder.Services.AddValidation();
builder.Services.AddAuthorization();
builder.Services.AddAppRateLimiting();

builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "a";
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "a";
        options.CallbackPath = "/api/auth/callback/google";
        options.ClaimActions.MapJsonKey("avatar", "picture", "url");
    })
    .AddGitHub(options =>
    {
        options.ClientId = builder.Configuration["GitHub:ClientId"] ?? "a";
        options.ClientSecret = builder.Configuration["GitHub:ClientSecret"] ?? "a";
        options.CallbackPath = "/api/auth/callback/github";
        options.Scope.Add("user:email");
        options.ClaimActions.MapJsonKey("avatar", "avatar_url", "url");
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.Cookie.Path = "/";
    options.SlidingExpiration = true;
});

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(loggers.HttpLogger);
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders |
                            HttpLoggingFields.ResponsePropertiesAndHeaders;

    logging.RequestHeaders.Add("User-Agent");
    logging.RequestHeaders.Add("Host");
    logging.ResponseHeaders.Add("Content-Type");
});

builder.AddDataBase();
var app = builder.Build();
app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();
app.MapGroup("/api/auth")
    .MapIdentityApi<AppUser>()
    .RequireRateLimiting(RateLimitPolicies.Auth);
app.MapControllers();
app.MigrateDb();
app.Run();
