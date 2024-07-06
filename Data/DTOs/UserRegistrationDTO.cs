using System.ComponentModel.DataAnnotations;

namespace Stage2.Data.DTOs
{
    public class UserRegistrationDTO
    {
        [Required]
        [StringLength(100)]
        public string firstName { get; set; }
        [Required]
        [StringLength(100)]
        public string lastName { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        [Phone]
        public string phone { get; set; }
    }
}
