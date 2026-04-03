namespace url_shortener.Endpoints;

public static class UrlEndpoints
{
    public static void MapUrlEndpoints(this WebApplication app)
    {
        app.MapPost("/shorten", () =>
        {
            // your logic here
        });

        app.MapGet("/{shortCode}", () =>
        {
            // your logic here
        });
    }
}
