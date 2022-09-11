using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using BlogApp.Api.BlogModule.Data;
using BlogApp.Api.BlogModule.Models;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace BlogApp.Tests.Integration.Controllers;

public class EditAuthorControllerTests : IClassFixture<IntegrationTestFactory<Program, BlogContext>>
{
    private readonly IntegrationTestFactory<Program, BlogContext> factory;

    private readonly IFixture fixture;

    public EditAuthorControllerTests(IntegrationTestFactory<Program, BlogContext> factory)
    {
        this.factory = factory;
        this.fixture = new Fixture();
        this.fixture.Customize<Author>(c => c.With(x => x.Email, "test@example.com"));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Edit_Author(bool hasAvatar)
    {
        // Arrange
        var createdAuthor = this.fixture.Create<Author>();
        await this.factory.AddAuthorAsync(createdAuthor);

        var editAuthor = this.fixture.Build<EditAuthor>()
            .With(x => x.Email, "edited-test@example.com")
            .With(x => x.Avatar, hasAvatar ? new Uri("https://cdn.fakercloud.com/avatars/emmeffess_128.jpg") : null)
            .Create();

        var client = this.factory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync($"authors/{createdAuthor.Id}", editAuthor);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var author = await response.Content.ReadFromJsonAsync<AuthorDetails>();
        author.ShouldBeEquivalentTo(new AuthorDetails(createdAuthor.Id, editAuthor.Name, editAuthor.Email, editAuthor.Avatar));

        // Clean-up
        await this.factory.DeleteAuthorAsync(createdAuthor);
    }

    [Fact]
    public async Task Should_Not_Find_Author()
    {
        // Arrange
        var editAuthor = this.fixture.Build<EditAuthor>()
            .With(x => x.Email, "edited-test@example.com")
            .With(x => x.Avatar, (Uri?)null)
            .Create();

        var client = this.factory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync($"authors/{Guid.NewGuid()}", editAuthor);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.ShouldNotBeNull();
        problemDetails.Title.ShouldBe("Not Found");
        problemDetails.Status.ShouldBe(404);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Should_Require_Author_Name(string? emptyName)
    {
        // Arrange
        var createdAuthor = this.fixture.Create<Author>();
        await this.factory.AddAuthorAsync(createdAuthor);

        var editAuthor = this.fixture.Build<CreateAuthor>()
            .With(x => x.Name, emptyName)
            .With(x => x.Email, "edited-test@example.com")
            .With(x => x.Avatar, (Uri?)null)
            .Create();

        var client = this.factory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync($"authors/{createdAuthor.Id}", editAuthor);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.ShouldNotBeNull();
        problemDetails.Title.ShouldBe("One or more validation errors occurred.");
        problemDetails.Status.ShouldBe(400);
        problemDetails.Errors.ShouldHaveSingleItem();
        problemDetails.Errors.ShouldContainKey("Name");

        // Clean-up
        await this.factory.DeleteAuthorAsync(createdAuthor);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("notanemailaddress")]
    public async Task Should_Require_Valid_Email(string? invalidEmail)
    {
        // Arrange
        var createdAuthor = this.fixture.Create<Author>();
        await this.factory.AddAuthorAsync(createdAuthor);

        var editAuthor = this.fixture.Build<CreateAuthor>()
            .With(x => x.Email, invalidEmail)
            .With(x => x.Avatar, (Uri?)null)
            .Create();

        var client = this.factory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync($"authors/{createdAuthor.Id}", editAuthor);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.ShouldNotBeNull();
        problemDetails.Title.ShouldBe("One or more validation errors occurred.");
        problemDetails.Status.ShouldBe(400);
        problemDetails.Errors.ShouldHaveSingleItem();
        problemDetails.Errors.ShouldContainKey("Email");

        // Clean-up
        await this.factory.DeleteAuthorAsync(createdAuthor);
    }
}
