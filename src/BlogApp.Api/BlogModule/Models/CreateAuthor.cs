using System.ComponentModel.DataAnnotations;

namespace BlogApp.Api.BlogModule.Models;

public class CreateAuthor
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public Uri? Avatar { get; set; }
}
