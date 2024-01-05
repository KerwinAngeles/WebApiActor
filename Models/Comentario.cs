using Microsoft.AspNetCore.Identity;

namespace WebApiActor.Models
{
    public class Comentario
    {
        public int Id { get; set; }
        public string Contenido { get; set; }
        public int PeliculaId { get; set; }
        public Pelicula Pelicula { get; set; }
        public string UsuarioId { get; set; }
        public IdentityUser Usuario { get; set; }
    }
}
