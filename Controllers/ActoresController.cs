using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<ActorDTOId>>> GetActores()
        {
            var autores =  await _context.Actors.ToListAsync();
            return mapper.Map<List<ActorDTOId>>(autores);

        }
        [HttpGet("{id}/getActorId", Name = "GetActor")] // obteniendo actores por id
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
            var entidadActorDTO = mapper.Map<ActorDTOId>(actor);
            return CreatedAtRoute("GetActor", routeValues: new { id = actor.Id }, value: entidadActorDTO);
        }

        [HttpPut("{id}")] // Actualizando un actor
        public async Task<ActionResult> UpdateActor([FromBody] ActorDTO actorDTO, int id)
        {
        
            var existe = await _context.Actors.AnyAsync(actor => actor.Id == id);
            if (!existe)
            {
                return NotFound("El id del actor no se encuentra");   
            }

            var actor = mapper.Map<Actor>(actorDTO);
            actor.Id = id;
            _context.Update(actor);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch]
        public async Task<ActionResult> PatchActor(int id, JsonPatchDocument<ActorDTO> patchDocument)
        {
            if(patchDocument == null)
            {
                return BadRequest();
            }
            var actorDB = await _context.Actors.FirstOrDefaultAsync(a => a.Id == id);
            if (actorDB == null)
            {
                return NotFound();
            }
            var ActorDTO = mapper.Map<ActorDTO>(actorDB);

            patchDocument.ApplyTo(ActorDTO, ModelState);

            var esValido = TryValidateModel(ActorDTO);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(ActorDTO, actorDB);
            await _context.SaveChangesAsync();
            return NoContent();
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
            return NoContent();
        }


    }
}
