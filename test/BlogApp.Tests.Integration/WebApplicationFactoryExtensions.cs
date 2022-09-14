using BlogApp.Api.BlogModule.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Tests.Integration;

public static class WebApplicationFactoryExtensions
{
    public static async Task AddAuthorAsync<TProgram>(this WebApplicationFactory<TProgram> factory, Author author) where TProgram : class
    {
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        using var db = scopedServices.GetRequiredService<BlogContext>();
        db.Add(author);
        await db.SaveChangesAsync();
    }

    public static async Task AddAuthorsAsync<TProgram>(this WebApplicationFactory<TProgram> factory, IEnumerable<Author> authors) where TProgram : class
    {
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        using var db = scopedServices.GetRequiredService<BlogContext>();
        db.AddRange(authors);
        await db.SaveChangesAsync();
    }

    public static async Task DeleteAuthorAsync<TProgram>(this WebApplicationFactory<TProgram> factory, Author author) where TProgram : class
    {
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        using var db = scopedServices.GetRequiredService<BlogContext>();
        db.Remove(author);
        await db.SaveChangesAsync();
    }

    public static async Task DeleteAuthorsAsync<TProgram>(this WebApplicationFactory<TProgram> factory, IEnumerable<Author> authors) where TProgram : class
    {
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        using var db = scopedServices.GetRequiredService<BlogContext>();
        db.RemoveRange(authors);
        await db.SaveChangesAsync();
    }
}
