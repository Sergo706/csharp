

using DocsParser.Services;
using Microsoft.EntityFrameworkCore;

namespace DocsParser.Models;

public static class DataExtension
{
    public static void MigrateDb(this WebApplication builder)
    {
        using var scope = builder.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }

    public static void AddDataBase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var serverVersion = ServerVersion.AutoDetect(connectionString);
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, serverVersion));
            
        builder.Services.AddScoped<DocumentService>();
    }
}
