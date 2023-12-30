using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiActor.Data;
using WebApiActor.DTO;
using WebApiActor.Models;

namespace WebApiActor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public ActoresController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }

        [HttpGet] // Obteniendo todo los actores
        public async Task<ActionResult<List<ActorDTOId>>> GetActores()
        {
            var autores =  await _context.Actors.ToListAsync();
            return mapper.Map<List<ActorDTOId>>(autores);

        }
        [HttpGet("{id}/getActorId")] // obteniendo actores por id
        public async Task<ActionResult<ActorConPeliculaDTO>> GetOneActor(int id)
        {
            var actor = await _context.Actors
                .Include(peliculaDB => peliculaDB.ActoresPeliculas)
                .ThenInclude(peliculaDeActor => peliculaDeActor.Pelicula)
                .FirstOrDefaultAsync(actorDB => actorDB.Id == id);

            if (actor == null)
            {
                return NotFound();
            }
            return mapper.Map<ActorConPeliculaDTO>(actor);
        }
        [HttpGet("{nombre}")] // Obteniendo actores por nombre
        public async Task<ActionResult<List<ActorDTOId>>> GetActorForName(string name)
        {
            var actorName = await _context.Actors.Where(actorDb => actorDb.Name.Contains(name)).ToListAsync();
            return mapper.Map<List<ActorDTOId>>(actorName);
        }

        [HttpPost] // Creando un actor
        public async Task<ActionResult> PostActor([FromBody] ActorDTO actorDTO)
        {

            var actor = mapper.Map<Actor>(actorDTO);
            _context.Add(actor);
            await _context.SaveChangesAsync();
            return Ok("Actor creado.");    
        }

        [HttpPut("{id}")] // Actualizando un actor
        public async Task<ActionResult> UpdateActor([FromBody] ActorDTOId actorDTO, int id)
        {
        
            var existe = await _context.Actors.AnyAsync(actor => actor.Id == id);
            if (!existe)
            {
                return NotFound("El id del actor no se encuentra");   
            }

            var actor = mapper.Map<Actor>(actorDTO);
            _context.Update(actor);
            await _context.SaveChangesAsync();
            return Ok("Actor actualizado");
        }

        [HttpDelete("{id}")] // Eliminando un actor
        public async Task<ActionResult> DeleteActor(int id)
        {
            var existe = await _context.Actors.AnyAsync(i => i.Id == id);
            if(!existe)
            {
                return NotFound("El id del actor no se encuentra");
            }
            _context.Remove(new Actor { Id = id });
            await _context.SaveChangesAsync();
            return Ok("Actor eliminado");
        }

    }
}
