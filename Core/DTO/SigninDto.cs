using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class SigninDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
