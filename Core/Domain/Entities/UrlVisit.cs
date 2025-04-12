using Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class UrlVisit
    {
        [Key]
        public int Id { get; set; }

        DateTime VisitedAt { get; set; } = DateTime.UtcNow;

        string IPAddress { get; set; } = string.Empty;

        string Country { get; set; } = string.Empty;

        DeviceType DeviceType { get; set; }

        string Referrer { get; set; } = string.Empty;

        string UserAgent { get; set; } = string.Empty;

        [ForeignKey("ShortUrl")]
        int ShortUrlId { get; set; }

        ShortUrl ShortUrl { get; set; }
    }
}
