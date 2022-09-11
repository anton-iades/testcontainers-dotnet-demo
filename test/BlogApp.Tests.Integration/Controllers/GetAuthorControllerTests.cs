using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using BlogApp.Api.BlogModule.Data;
using BlogApp.Api.BlogModule.Extensions;
using BlogApp.Api.BlogModule.Models;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace BlogApp.Tests.Integration.Controllers;

public class GetAuthorControllerTests : IClassFixture<IntegrationTestFactory<Program, BlogContext>>
{
    private readonly IntegrationTestFactory<Program, BlogContext> factory;

    private readonly IFixture fixture;

    public GetAuthorControllerTests(IntegrationTestFactory<Program, BlogContext> factory)
    {
        this.factory = factory;
        this.fixture = new Fixture();
        this.fixture.Customize<Author>(c => c
            .With(x => x.Email, "test@example.com"));
    }

    [Fact]
    public async Task Should_Find_Author()
    {
        // Arrange
        var createdAuthor = this.fixture.Create<Author>();
        await this.factory.AddAuthorAsync(createdAuthor);

        var client = this.factory.CreateClient();

        // Act
        var response = await client.GetAsync($"authors/{createdAuthor.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var author = await response.Content.ReadFromJsonAsync<AuthorDetails>();
        author.ShouldBeEquivalentTo(createdAuthor.MapToViewModel());

        // Clean-up
        await this.factory.DeleteAuthorAsync(createdAuthor);
    }

    [Fact]
    public async Task Should_Not_Find_Author()
    {
        // Arrange
        var client = this.factory.CreateClient();
        var authorId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"authors/{authorId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.ShouldNotBeNull();
        problemDetails.Title.ShouldBe("Not Found");
        problemDetails.Status.ShouldBe(404);
    }

    [Fact]
    public async Task Should_Be_Bad_Request()
    {
        // Arrange
        var client = this.factory.CreateClient();

        // Act
        var response = await client.GetAsync($"authors/not-a-guid");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.ShouldNotBeNull();
        problemDetails.Title.ShouldBe("One or more validation errors occurred.");
        problemDetails.Status.ShouldBe(400);
        problemDetails.Errors["id"][0].ShouldBe("The value 'not-a-guid' is not valid.");
    }
}
