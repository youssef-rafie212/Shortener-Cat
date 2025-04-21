using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class RegisterDeviceDto
    {
        [Required]
        public string DeviceToken { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}
