using System.ComponentModel.DataAnnotations;

namespace WebApiActor.DTO
{
    public class PeliculaDTO
    {
        [Required]
        [StringLength(maximumLength: 255)]
        public string Name { get; set; }
        public string Description { get; set; } 
        public string Category { get; set; }
        public List<int> ActoresIds { get; set; } 
    }
}
