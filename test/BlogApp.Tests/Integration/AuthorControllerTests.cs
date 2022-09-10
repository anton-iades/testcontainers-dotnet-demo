using System.Text.Json;
using BlogApp.Api.Authors;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace BlogApp.Tests.Integration;

public class AuthorControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public AuthorControllerTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task GetAuthor_Should_Find_Author()
    {
        // Arrange
        var client = this.factory.CreateClient();
        var authorId = Guid.NewGuid().ToString();

        // Act
        var result = await client.GetAsync($"authors/{authorId}");

        // Assert
        result.IsSuccessStatusCode.ShouldBeTrue();
        var author = await JsonSerializer.DeserializeAsync<Author>(
            await result.Content.ReadAsStreamAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true, });
        author.ShouldBeEquivalentTo(new Author
        {
            Id = authorId,
        });
    }
}