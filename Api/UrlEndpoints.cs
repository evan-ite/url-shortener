using StackExchange.Redis;
using url_shortener.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace url_shortener.Endpoints;

public static class UrlEndpoints
{
    public static void MapUrlEndpoints(this WebApplication app)
    {
        app.MapPost("/shorten", async (ShortenRequest request, IDistributedCache cache, AppDbContext db) =>
        {
            var shortCode = Guid.NewGuid().ToString()[..6];

            db.Urls.Add(new ShortenedUrl { ShortCode = shortCode, OriginalUrl = request.Url });
            await db.SaveChangesAsync();

            await cache.SetStringAsync(shortCode, request.Url);

            return Results.Ok($"http://localhost:5292/{shortCode}");
        });

        app.MapGet("/{shortCode}", async (string shortCode, IDistributedCache cache, AppDbContext db) =>
        {
            var cachedUrl = await cache.GetStringAsync(shortCode);
            if (cachedUrl is not null) return Results.Redirect(cachedUrl);

            var entry = await db.Urls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
            if (entry is null) return Results.NotFound();

            await cache.SetStringAsync(shortCode, entry.OriginalUrl);
            return Results.Redirect(entry.OriginalUrl);
        });
    }
}

record ShortenRequest(string Url);