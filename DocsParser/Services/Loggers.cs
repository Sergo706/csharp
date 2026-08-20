using Serilog;
using Serilog.Formatting.Json;

namespace DocsParser.Services.Loggers;

public interface IAppLogger
{
    Serilog.ILogger AppLogger { get; }
}

public interface IHttpLogger
{
    Serilog.ILogger HttpLogger { get; }
}

public class Loggers : IAppLogger, IHttpLogger
{
    public Serilog.ILogger HttpLogger { get; }
    public Serilog.ILogger AppLogger { get; }

    public Loggers()
    {
        AppLogger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(
                new JsonFormatter(),
                "logs/app.log",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true
            )
            .CreateLogger();

        HttpLogger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                new JsonFormatter(),
                "logs/http.log",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true
            )
            .CreateLogger();
    }
}