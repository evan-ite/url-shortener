using StackExchange.Redis;

namespace url_shortener.Endpoints;

public static class UrlEndpoints
{
    public static void MapUrlEndpoints(this WebApplication app)
    {
        app.MapPost("/shorten", async (ShortenRequest request, IConnectionMultiplexer redis) =>
        {
            var db = redis.GetDatabase();
            var shortCode = Guid.NewGuid().ToString()[..6];
            await db.StringSetAsync(shortCode, request.Url);
            return Results.Ok($"http://localhost:5292/{shortCode}");
        });

        app.MapGet("/{shortCode}", async (string shortCode, IConnectionMultiplexer redis) =>
        {
            var db = redis.GetDatabase();
            var url = await db.StringGetAsync(shortCode);
            if (url.IsNullOrEmpty) return Results.NotFound();
            return Results.Redirect(url!);
        });
    }
}

record ShortenRequest(string Url);