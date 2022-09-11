using Microsoft.EntityFrameworkCore;

namespace BlogApp.Api.BlogModule.Data;

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options)
        : base(options)
    {
    }

    public DbSet<Author> Authors => Set<Author>();
}
