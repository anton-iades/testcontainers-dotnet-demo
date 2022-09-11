using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using BlogApp.Api.BlogModule.Data;
using BlogApp.Api.BlogModule.Models;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace BlogApp.Tests.Integration.Controllers;

public class CreateAuthorControllerTests : IClassFixture<IntegrationTestFactory<Program, BlogContext>>
{
    private readonly IntegrationTestFactory<Program, BlogContext> factory;

    private readonly IFixture fixture;

    public CreateAuthorControllerTests(IntegrationTestFactory<Program, BlogContext> factory)
    {
        this.factory = factory;
        this.fixture = new Fixture();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Create_Author(bool hasAvatar)
    {
        // Arrange
        var createAuthor = this.fixture.Build<CreateAuthor>()
            .With(x => x.Email, "test@example.com")
            .With(x => x.Avatar, hasAvatar ? new Uri("https://cdn.fakercloud.com/avatars/emmeffess_128.jpg") : null)
            .Create();

        var client = this.factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("authors", createAuthor);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull();
        var author = await response.Content.ReadFromJsonAsync<AuthorDetails>();
        author.ShouldNotBeNull();
        author.Id.ShouldNotBe(Guid.Empty);
        author.Name.ShouldBe(createAuthor.Name);
        author.Email.ShouldBe(createAuthor.Email);
        author.Avatar.ShouldBe(createAuthor.Avatar);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Should_Require_Author_Name(string? emptyName)
    {
        // Arrange
        var createAuthor = this.fixture.Build<CreateAuthor>()
            .With(x => x.Name, emptyName)
            .With(x => x.Email, "test@example.com")
            .With(x => x.Avatar, (Uri?)null)
            .Create();

        var client = this.factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("authors", createAuthor);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.ShouldNotBeNull();
        problemDetails.Title.ShouldBe("One or more validation errors occurred.");
        problemDetails.Status.ShouldBe(400);
        problemDetails.Errors.ShouldHaveSingleItem();
        problemDetails.Errors.ShouldContainKey("Name");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("notanemailaddress")]
    public async Task Should_Require_Valid_Email(string? invalidEmail)
    {
        // Arrange
        var createAuthor = this.fixture.Build<CreateAuthor>()
            .With(x => x.Email, invalidEmail)
            .With(x => x.Avatar, (Uri?)null)
            .Create();

        var client = this.factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("authors", createAuthor);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.ShouldNotBeNull();
        problemDetails.Title.ShouldBe("One or more validation errors occurred.");
        problemDetails.Status.ShouldBe(400);
        problemDetails.Errors.ShouldHaveSingleItem();
        problemDetails.Errors.ShouldContainKey("Email");
    }
}
