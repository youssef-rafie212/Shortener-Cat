using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class PushNotiForAllDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
