using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class PushNotiForMultipleDto
    {
        [Required]
        public List<string> DeviceTokens { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
