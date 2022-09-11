using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using BlogApp.Api.BlogModule.Data;
using BlogApp.Api.BlogModule.Models;
using Shouldly;

namespace BlogApp.Tests.Integration.Controllers;

public class GetAllAuthorControllerTests : IClassFixture<IntegrationTestFactory<Program, BlogContext>>
{
    private readonly IntegrationTestFactory<Program, BlogContext> factory;

    private readonly IFixture fixture;

    public GetAllAuthorControllerTests(IntegrationTestFactory<Program, BlogContext> factory)
    {
        this.factory = factory;
        this.fixture = new Fixture();
        this.fixture.Customize<Author>(c => c.With(x => x.Email, "test@example.com"));
    }

    [Fact]
    public async Task Should_Get_All_Authors()
    {
        // Arrange
        const int authorCount = 4;
        var createdAuthors = this.fixture.CreateMany<Author>(authorCount);
        await this.factory.AddAuthorsAsync(createdAuthors);

        var client = this.factory.CreateClient();

        // Act
        var response = await client.GetAsync("authors");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var authors = await response.Content.ReadFromJsonAsync<AuthorDetails[]>();
        authors.ShouldNotBeNull();
        authors.Length.ShouldBe(authorCount);

        // Clean-up
        await this.factory.DeleteAuthorsAsync(createdAuthors);
    }

    [Fact]
    public async Task Should_Return_Empty_Collection()
    {
        // Arrange
        var client = this.factory.CreateClient();

        // Act
        var response = await client.GetAsync("authors");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var authors = await response.Content.ReadFromJsonAsync<AuthorDetails[]>();
        authors.ShouldBeEmpty();
    }
}
