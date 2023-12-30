using AutoMapper;
using WebApiActor.DTO;
using WebApiActor.Models;

namespace WebApiActor.Utilidades
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ActorDTO, Actor>();
            CreateMap<ActorDTOId, Actor>();
            CreateMap<Actor, ActorDTOId>();
            CreateMap<Actor, ActorConPeliculaDTO>()
                .ForMember(peliculaDB => peliculaDB.Pelicula,
                opciones => opciones.MapFrom(MapActoresPeliculas));

            CreateMap<PeliculaDTO, Pelicula>().ForMember(pelicula => pelicula.ActoresPeliculas,
                opciones => opciones.MapFrom(MapActorPelicula));

            CreateMap<PeliculaDTOId, Pelicula>();
            CreateMap<Pelicula, PeliculaDTOId>();
            CreateMap<Pelicula, PeliculaConActorDTO>().ForMember(peliculaDB => peliculaDB.Actors,
                opciones => opciones.MapFrom(MapPeliculaActor));

            CreateMap<ComentarioDTO, Comentario>();
            CreateMap<ComentarioDTOId,  Comentario>();
            CreateMap<Comentario, ComentarioDTOId>();
        }

        private List<ActorDTOId> MapPeliculaActor(Pelicula pelicula, PeliculaDTOId peliDTO)
        {
            var result = new List<ActorDTOId>();

            if(pelicula.ActoresPeliculas == null) { return result; }

            foreach (var actorPelicula in pelicula.ActoresPeliculas)
            {
                result.Add(new ActorDTOId()
                {
                    Id = actorPelicula.Actor.Id,
                    Name = actorPelicula.Actor.Name,
                    LastName = actorPelicula.Actor.LastName,
                    Age = actorPelicula.Actor.Age
                });
            }

            return result;
        }

        private List<PeliculaDTOId> MapActoresPeliculas(Actor actor, ActorDTOId actorDTO)
        {
            var result = new List<PeliculaDTOId>();
            if(actor.ActoresPeliculas == null) { return result; }
            foreach (var actorPelicula in actor.ActoresPeliculas)
            {
                result.Add(new PeliculaDTOId()
                {
                    ID = actorPelicula.Pelicula.Id,
                    Name = actorPelicula.Pelicula.Name,
                    Description = actorPelicula.Pelicula.Description,
                    Category = actorPelicula.Pelicula.Category
                });
            }
            return result;
        }

        private List<ActorPelicula> MapActorPelicula(PeliculaDTO peliDTO, Pelicula pelicula)
        {
            var result = new List<ActorPelicula>();
            if(peliDTO.ActoresIds == null) { return result;}
            foreach (var actorId in peliDTO.ActoresIds)
            {
                result.Add(new ActorPelicula { ActorId = actorId });
            }
            return result;
        }
    }
}
