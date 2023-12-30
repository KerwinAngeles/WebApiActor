using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiActor.Data;
using WebApiActor.DTO;
using WebApiActor.Models;
using WebApiActor.Services.Interfaces;

namespace WebApiActor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IOrdenarActores _ordenarActores;

        public PeliculaController(ApplicationDbContext context, IMapper mapper, IOrdenarActores ordenarActores)
        {
            _context = context;
            _mapper = mapper;
            _ordenarActores = ordenarActores;
        }

        [HttpGet] // Obteniendo todas las peliculas
        public async Task<ActionResult<List<PeliculaDTOId>>> GetPelicula()
        {
            var pelicula = await _context.Peliculas.ToListAsync();
            return _mapper.Map<List<PeliculaDTOId>>(pelicula);
        }

        [HttpGet("{id}", Name ="GetPelicula")] // Obtener pelicula por id
        public async Task<ActionResult<PeliculaConActorDTO>> GetPeliculaId(int id)
        {
            var pelicula = await _context.Peliculas
                .Include(actoresDB => actoresDB.ActoresPeliculas)
                .ThenInclude(actoresDePelicula => actoresDePelicula.Actor)
                .FirstOrDefaultAsync(peliculaDB => peliculaDB.Id == id);

            if(pelicula == null)
            {
                return NotFound();
            }

            pelicula.ActoresPeliculas = pelicula.ActoresPeliculas.OrderBy(o => o.Orden).ToList();
            return _mapper.Map<PeliculaConActorDTO>(pelicula);
        }

        [HttpPost] // Agregando pelicula
        public async Task<ActionResult> PostPelicula([FromBody] PeliculaDTO peliculaDTO)
        {
            if(peliculaDTO.ActoresIds == null)
            {
                return BadRequest("No se puede crear una pelicula sin actores");
            }

            var actor = await _context.Actors.Where(actordb => peliculaDTO.ActoresIds
                .Contains(actordb.Id))
                .Select(x => x.Id)
                .ToListAsync();

            if(peliculaDTO.ActoresIds.Count != actor.Count)
            {
                return BadRequest("No existe uno de los actores enviados");
            }

            var pelicula = _mapper.Map<Pelicula>(peliculaDTO);

            _ordenarActores.OrdenarActores(pelicula);
           
            _context.Add(pelicula);
            await _context.SaveChangesAsync();
            var entidadPeliculaDTO = _mapper.Map<PeliculaDTOId>(pelicula);
            return CreatedAtRoute("GetPelicula", routeValues: new { id = pelicula.Id }, value: entidadPeliculaDTO);
        }

        [HttpPut] // Actualizando pelicula
        public async Task<ActionResult> PutPelicula([FromBody] PeliculaDTO peliculaDTO, int idPelicula)
        {
           var peliculaDB = await _context.Peliculas.Include(p => p.ActoresPeliculas).FirstOrDefaultAsync(p => p.Id == idPelicula);
            if(peliculaDB == null)
            {
                return NotFound();
            }
            _ordenarActores.OrdenarActores(peliculaDB);
            peliculaDB = _mapper.Map(peliculaDTO, peliculaDB);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch] // actualizando un valor de la pelicula
        public async Task<ActionResult> PatchPelicula(int id, JsonPatchDocument<PeliculaPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var peliculaDB = await _context.Peliculas.FirstOrDefaultAsync(p => p.Id == id);
            if(peliculaDB == null)
            {
                return NotFound();
            }

            var peliculaDTO = _mapper.Map<PeliculaPatchDTO>(peliculaDB);
            patchDocument.ApplyTo(peliculaDTO, ModelState);

            var esValido = TryValidateModel(peliculaDTO);
            if(!esValido)
            {
                return BadRequest();
            }

            _mapper.Map(peliculaDTO, peliculaDB);
            await _context.SaveChangesAsync();
            return NoContent();

        }

        [HttpDelete("Id:int")]
        public async Task<ActionResult> DeletePelicula(int id)
        {
            var existePelicula = await _context.Peliculas.AnyAsync(p => p.Id == id);
            if (!existePelicula)
            {
                return NotFound();
            }

            _context.Remove(new Pelicula {Id = id});
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
