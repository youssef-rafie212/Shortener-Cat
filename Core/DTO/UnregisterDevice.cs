using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class UnregisterDevice
    {
        [Required]
        public string DeviceToken { get; set; }
    }
}
