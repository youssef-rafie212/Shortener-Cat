using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        List<ShortUrl> Urls { get; set; } = [];
    }
}
