using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class PushNotiForOneDto
    {
        [Required]
        public string DeviceToken { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
