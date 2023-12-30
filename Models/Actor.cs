using System.ComponentModel.DataAnnotations;

namespace WebApiActor.Models
{
    public class Actor
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength:120)]
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public List<ActorPelicula> ActoresPeliculas { get; set;}

    }
}
