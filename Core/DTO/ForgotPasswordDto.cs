using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class ForgotPasswordDto
    {
        [Required]
        public string UserId { get; set; }
    }
}
