using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class GoogleLoginDto
    {
        [Required]
        public string Token { get; set; }
    }
}
