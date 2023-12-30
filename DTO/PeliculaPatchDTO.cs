using System.ComponentModel.DataAnnotations;

namespace WebApiActor.DTO
{
    public class PeliculaPatchDTO
    {
        [Required]
        [StringLength(maximumLength: 255)]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }
}
