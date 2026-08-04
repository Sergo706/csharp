using Scalar.AspNetCore;
using Handler;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseCors();
    app.Use(async (context, next) =>
    {
        Helpers.Log(context);
        await next();
    });
}



app.MapGet("/", () =>
{
    return Results.Ok(Movies.CurrentMovies);
});
app.MapPost("/add", (MoviesMem newMovie) =>
{
   Movies.CurrentMovies.Add(newMovie);
   return Results.Created($"{newMovie.Title}", newMovie);
});

app.Run("http://localhost:3000");