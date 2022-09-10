using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Authors;

[ApiController]
[Route("[controller]")]
public class AuthorsController : ControllerBase
{
    [HttpGet("{id}")]
    public ActionResult<Author> GetAuthor(string id, CancellationToken cancellationToken)
    {
        return new Author { Id = id };
    }
}
