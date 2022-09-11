using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Api.BlogModule.Data;

[Table("author")]
public class Author
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("name")]
    public string Name { get; set; } = null!;

    [Required]
    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("avatar")]
    public Uri? Avatar { get; set; }
}
