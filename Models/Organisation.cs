using System.ComponentModel.DataAnnotations;

namespace Stage2.Models
{
    public class Organisation
    {
        [Key]
        public string OrgId { get; set; } = Guid.NewGuid().ToString();
        [Required]
        [MaxLength(50)]
        public string name { get; set; }
        [Required]
        [MaxLength(100)]
        public string description { get; set; }
        public List<User> Users { get; set; } = new List<User>();
    }
}
