using System.ComponentModel.DataAnnotations;

namespace Stage2.Data.DTOs
{
    public class OrganisationCreationDTO
    {
     
        public string? orgId { get; set; }
        [Required]
        [StringLength(50)]
        public string? name { get; set; }
        [Required]
        [StringLength(100)]
        public string? description { get; set; }
    }
}
