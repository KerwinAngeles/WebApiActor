namespace WebApiActor.DTO
{
    public class ActorConPeliculaDTO : ActorDTOId
    {
        public List<PeliculaDTOId> Pelicula { get; set; }
    }
}
