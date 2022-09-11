using BlogApp.Api.BlogModule.Data;
using BlogApp.Api.BlogModule.Models;

namespace BlogApp.Api.BlogModule.Extensions;

public static class MapperExtensions
{
    public static AuthorDetails MapToViewModel(this Author src)
    {
        if (src is null)
        {
            throw new ArgumentNullException(nameof(src));
        }

        var dst = new AuthorDetails(src.Id, src.Name, src.Email, src.Avatar);

        return dst;
    }
}
