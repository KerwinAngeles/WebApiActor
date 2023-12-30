namespace WebApiActor.Models
{
    public class ActorPelicula
    {
        public int PeliculaId { get; set; }
        public int ActorId { get; set; }
        public int Orden {  get; set; }
        public Pelicula Pelicula { get; set; }
        public Actor Actor { get; set; }
    }
}
