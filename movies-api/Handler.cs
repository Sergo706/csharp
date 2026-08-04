namespace Handler;

public record MoviesMem
{
    public required string Title { get; init; }
    public required string Genre { get; set; }
    public required string Description { get; set; }
    public required DateTime ReleaseYear {get; set; }

}
public static class Movies
{
    public static List<MoviesMem> CurrentMovies { get; set; } = new List<MoviesMem>()
    {
        new() {
            Title = "Inception",
            Genre = "Sci-Fi",
            Description = "A thief steals corporate secrets through dream-sharing.",
            ReleaseYear = new DateTime(2010, 7, 16)
        },
        new()
        {
            Title = "The Matrix",
            Genre = "Sci-Fi",
            Description = "A computer hacker learns about the true nature of his reality.",
            ReleaseYear = new DateTime(1999, 3, 31)
        },
        new()
        {
            Title = "Neon Chase",
            Genre = "Action",
            Description = "A rogue courier races through a futuristic city to stop a corporate conspiracy.",
            ReleaseYear = new DateTime(2026, 1, 1)
        },
        new()
        {
            Title = "Moonlit Harvest",
            Genre = "Mystery",
            Description = "An investigator returns to her hometown to solve a string of strange disappearances.",
            ReleaseYear = new DateTime(2024, 1, 1)
        }

    };
}

public static class Helpers
{
    public static void Log(HttpContext ctx)
    {
        var meth = ctx.Request.Method;
        var headers = string.Join(" | ", ctx.Request.Headers.Select(h => $"{h.Key}: {h.Value}"));
        var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
        Console.WriteLine($"{meth} : {headers} : {ip}");
    }
}