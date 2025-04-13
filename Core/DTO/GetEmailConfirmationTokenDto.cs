using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class ConfirmEmailDto
    {
        [Required]
        public string Token { get; set; }
    }
}
