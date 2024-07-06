using System.ComponentModel.DataAnnotations;

namespace Stage2.Models
{
    public class User
    {
        [Key]
        public string UserId { get; set; } = Guid.NewGuid().ToString();
        [Required]
        [MaxLength(50)]
        public string firstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string lastName { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string phone { get; set; }
        public List<Organisation> Organisations { get; set; } = new List<Organisation>();
    }
}

