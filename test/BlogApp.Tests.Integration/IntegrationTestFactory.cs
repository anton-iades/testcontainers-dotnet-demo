using BlogApp.Api.BlogModule.Data;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Tests.Integration;

public class IntegrationTestFactory<TEntryPoint, TDbContext> : WebApplicationFactory<TEntryPoint>, IAsyncLifetime
    where TEntryPoint : class
    where TDbContext : DbContext
{
    private readonly TestcontainerDatabase testContainer;

    public IntegrationTestFactory()
    {
        this.testContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "test-db",
                Username = "test-user",
                Password = "test-pswd"
            })
            .WithImage("postgres:14.5")
            .WithCleanUp(true)
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder.ConfigureTestServices(services =>
    {
        // find and remove the production context
        services.RemoveDbContext<TDbContext>();

        // add the test container context
        services.AddDbContext<TDbContext>(options => options.UseNpgsql(this.testContainer.ConnectionString));

        // ensure schema is created
        services.EnsureDbCreated<TDbContext>();
    });

    public Task InitializeAsync() => this.testContainer.StartAsync();

    public new Task DisposeAsync() => this.testContainer.DisposeAsync().AsTask();

    public async Task AddAuthorAsync(Author author)
    {
        using var scope = this.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        using var db = scopedServices.GetRequiredService<BlogContext>();
        db.Add(author);
        await db.SaveChangesAsync();
    }

    public async Task AddAuthorsAsync(IEnumerable<Author> authors)
    {
        using var scope = this.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        using var db = scopedServices.GetRequiredService<BlogContext>();
        db.AddRange(authors);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAuthorAsync(Author author)
    {
        using var scope = this.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        using var db = scopedServices.GetRequiredService<BlogContext>();
        db.Remove(author);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAuthorsAsync(IEnumerable<Author> authors)
    {
        using var scope = this.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        using var db = scopedServices.GetRequiredService<BlogContext>();
        db.RemoveRange(authors);
        await db.SaveChangesAsync();
    }
}
