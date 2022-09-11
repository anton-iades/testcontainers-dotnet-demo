using BlogApp.Api.BlogModule.Data;
using BlogApp.Api.BlogModule.Extensions;
using BlogApp.Api.BlogModule.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Api.BlogModule.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class AuthorsController : ControllerBase
{
    private readonly BlogContext db;

    public AuthorsController(BlogContext db)
    {
        this.db = db;
    }

    [HttpPost]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
    public async Task<ActionResult<AuthorDetails>> Create(CreateAuthor request, CancellationToken cancellationToken)
    {
        var newAuthor = new Author
        {
            Name = request.Name,
            Email = request.Email,
            Avatar = request.Avatar,
        };

        this.db.Add(newAuthor);

        await this.db.SaveChangesAsync(cancellationToken);

        var vm = newAuthor.MapToViewModel();

        return this.CreatedAtAction(nameof(Get), new { id = vm.Id }, vm);
    }

    [HttpGet("{id}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<AuthorDetails>> Get(Guid id, CancellationToken cancellationToken)
    {
        var author = await this.db.Authors.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (author is null)
        {
            return NotFound();
        }

        return author.MapToViewModel();
    }

    [HttpGet]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<AuthorDetails[]>> GetAll(CancellationToken cancellationToken)
    {
        var authors = await this.db.Authors.AsNoTracking()
            .ToListAsync(cancellationToken);

        return authors.Select(x => x.MapToViewModel()).ToArray();
    }

    [HttpPut("{id}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Edit))]
    public async Task<ActionResult<AuthorDetails>> Edit(Guid id, EditAuthor request, CancellationToken cancellationToken)
    {
        var author = await this.db.Authors
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (author is null)
        {
            return NotFound();
        }

        this.db.Entry(author).CurrentValues.SetValues(request);
        await this.db.SaveChangesAsync(cancellationToken);

        return author.MapToViewModel();
    }

    [HttpDelete("{id}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
    public async Task<ActionResult<AuthorDetails>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var author = await this.db.Authors
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (author is null)
        {
            return NotFound();
        }

        this.db.Remove(author);
        await this.db.SaveChangesAsync(cancellationToken);

        return author.MapToViewModel();
    }
}
