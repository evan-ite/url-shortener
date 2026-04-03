using StackExchange.Redis;
using url_shortener.Data;
using Microsoft.EntityFrameworkCore;

namespace url_shortener.Endpoints;

public static class UrlEndpoints
{
    public static void MapUrlEndpoints(this WebApplication app)
    {
        app.MapPost("/shorten", async (ShortenRequest request, IConnectionMultiplexer redis, AppDbContext db) =>
        {
            var shortCode = Guid.NewGuid().ToString()[..6];

            db.Urls.Add(new ShortenedUrl { ShortCode = shortCode, OriginalUrl = request.Url });
            await db.SaveChangesAsync();

            var cache = redis.GetDatabase();
            await cache.StringSetAsync(shortCode, request.Url);

            return Results.Ok($"http://localhost:5292/{shortCode}");
        });

        app.MapGet("/{shortCode}", async (string shortCode, IConnectionMultiplexer redis, AppDbContext db) =>
        {
            var cache = redis.GetDatabase();
            var cachedUrl = await cache.StringGetAsync(shortCode);
            if (!cachedUrl.IsNullOrEmpty) return Results.Redirect(cachedUrl!);

            var entry = await db.Urls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
            if (entry is null) return Results.NotFound();

            await cache.StringSetAsync(shortCode, entry.OriginalUrl);
            return Results.Redirect(entry.OriginalUrl);
        });
    }
}

record ShortenRequest(string Url);