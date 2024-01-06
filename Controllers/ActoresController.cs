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
using WebApiActor.Utilidades;

namespace WebApiActor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService _authorizationService;

        public ActoresController(ApplicationDbContext context, IMapper mapper, IAuthorizationService service)
        {
            _context = context;
            this.mapper = mapper;
            _authorizationService = service;
        }

        [HttpGet(Name = "ObtenerActores")] // Obteniendo todo los actores
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOSActorFilterAttribute))]
        public async Task<ActionResult<List<ActorDTOId>>> GetActores([FromHeader] string incluirHATEOAS)
        {
            var autores =  await _context.Actors.ToListAsync();
            return mapper.Map<List<ActorDTOId>>(autores);

        }
        [HttpGet("{id}/getActorId", Name = "ObtenerActoresPorId")] // obteniendo actores por id
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOSActorFilterAttribute))]
        public async Task<ActionResult<ActorConPeliculaDTO>> GetOneActor(int id, [FromHeader] string incluirHATEOAS)
        {
            var actor = await _context.Actors
                .Include(peliculaDB => peliculaDB.ActoresPeliculas)
                .ThenInclude(peliculaDeActor => peliculaDeActor.Pelicula)
                .FirstOrDefaultAsync(actorDB => actorDB.Id == id);

            if (actor == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<ActorConPeliculaDTO>(actor);
            return dto;
        }

        [HttpGet("{nombre}", Name = "ObtenerActoresPorNombre")] // Obteniendo actores por nombre
        public async Task<ActionResult<List<ActorDTOId>>> GetActorForName(string name)
        {
            var actorName = await _context.Actors.Where(actorDb => actorDb.Name.Contains(name)).ToListAsync();
            return mapper.Map<List<ActorDTOId>>(actorName);
        }

        [HttpPost(Name = "CrearActor")] // Creando un actor
        public async Task<ActionResult> PostActor([FromBody] ActorDTO actorDTO)
        {

            var actor = mapper.Map<Actor>(actorDTO);
            _context.Add(actor);
            await _context.SaveChangesAsync();
            var entidadActorDTO = mapper.Map<ActorDTOId>(actor);
            return CreatedAtRoute("ObtenerActoresPorId", routeValues: new { id = actor.Id }, value: entidadActorDTO);
        }

        [HttpPut("{id}", Name = "ActualizarActor")] // Actualizando un actor
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

        [HttpPatch(Name = "ActualizarAtributoDeActor")]
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

        [HttpDelete("{id}", Name = "EliminarActor")] // Eliminando un actor
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
