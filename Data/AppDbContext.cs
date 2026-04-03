using Microsoft.EntityFrameworkCore;

namespace url_shortener.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ShortenedUrl> Urls { get; set; }
}
