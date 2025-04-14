using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class UpdateShortUrlDto
    {
        [Required]
        [Url]
        public string NewValue { get; set; }

        [Required]
        [Url]
        public string NewOriginalUrl { get; set; }

        [Required]
        public int NewVisitCount { get; set; }
    }
}
