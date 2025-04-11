using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    // defines the stored short url generated from the original one.
    public class ShortUrl
    {
        [Key]
        public int Id { get; set; }

        public string Value { get; set; } = string.Empty;

        public string OriginalUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public DateTime ExpiresAt { get; set; }

        public int VisitCount { get; set; }

        public int MaxVisit { get; set; } = 10000;

        [ForeignKey("User")]
        public int UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
