namespace BlogApp.Api.BlogModule.Models;

public record AuthorDetails(Guid Id, string Name, string Email, Uri? Avatar);
