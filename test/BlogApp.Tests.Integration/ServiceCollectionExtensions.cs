using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Tests.Integration;

public static class ServiceCollectionExtensions
{
    public static void RemoveDbContext<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    {
        var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<TDbContext>));

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }
    }

    public static void EnsureDbCreated<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    {
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var dbContext = scopedServices.GetRequiredService<TDbContext>();
        dbContext.Database.EnsureCreated();
    }
}