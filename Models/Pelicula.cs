using System.ComponentModel.DataAnnotations;

namespace WebApiActor.Models
{
    public class Pelicula
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 255)]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public List<Comentario> Commentary { get; set; }
        public List<ActorPelicula> ActoresPeliculas { get; set;}

    }
}
