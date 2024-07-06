using System.ComponentModel.DataAnnotations;

namespace Stage2.Data.DTOs
{
    public class UserLoginDTO
    {
        [EmailAddress]
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }
}
