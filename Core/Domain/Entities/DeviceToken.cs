using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class DeviceToken
    {
        [Key]
        public int Id { get; set; }

        public string Value { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
