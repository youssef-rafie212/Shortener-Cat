using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities
{
    public class ExpiredToken
    {
        [Key]
        public int Id { get; set; }

        public string Value { get; set; }
    }
}
