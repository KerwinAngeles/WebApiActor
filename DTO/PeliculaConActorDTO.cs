namespace WebApiActor.DTO
{
    public class PeliculaConActorDTO : PeliculaDTOId
    {
        public List<ActorDTOId> Actors { get; set; }

    }
}
