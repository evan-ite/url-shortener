using StackExchange.Redis;
using url_shortener.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace url_shortener.Endpoints;

public static class UrlEndpoints
{
    public static void MapUrlEndpoints(this WebApplication app)
    {
        app.MapPost("/shorten", async (ShortenRequest body, HttpRequest request, IDistributedCache cache, AppDbContext db) =>
        {
            var shortCode = Guid.NewGuid().ToString()[..6];

            db.Urls.Add(new ShortenedUrl { ShortCode = shortCode, OriginalUrl = body.Url });
            await db.SaveChangesAsync();

            await cache.SetStringAsync(shortCode, body.Url);

            return Results.Ok($"{request.Scheme}://{request.Host}/{shortCode}");
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