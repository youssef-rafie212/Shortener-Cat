using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class GenerateShortUrlDto
    {
        [Required]
        [Url]
        public string OriginalUrl { get; set; }
    }
}
