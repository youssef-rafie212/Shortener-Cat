using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class UrlVisit
    {
        [Key]
        public int Id { get; set; }

        public DateTime VisitedAt { get; set; } = DateTime.UtcNow;

        public string? IPAddress { get; set; } = string.Empty;

        public string? Country { get; set; } = string.Empty;

        public string DeviceType { get; set; } = string.Empty;

        public string? Referrer { get; set; } = string.Empty;

        public string? UserAgent { get; set; } = string.Empty;

        [ForeignKey("ShortUrl")]
        public int ShortUrlId { get; set; }

        public ShortUrl ShortUrl { get; set; }
    }
}
