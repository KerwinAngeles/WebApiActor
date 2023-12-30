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
    public class PeliculaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PeliculaController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

            if (pelicula.ActoresPeliculas != null)
            {
                for (int i = 1; i < pelicula.ActoresPeliculas.Count; i++)
                {
                    pelicula.ActoresPeliculas[i].Orden = i;
                }
            }
            _context.Add(pelicula);
            await _context.SaveChangesAsync();
            var entidadPeliculaDTO = _mapper.Map<PeliculaDTOId>(pelicula);
            return CreatedAtRoute("GetPelicula", routeValues: new { id = pelicula.Id }, value: entidadPeliculaDTO);
        }

        [HttpPut("{id}")] // Actualizando pelicula
        public async Task<ActionResult> PutPelicula([FromBody] PeliculaDTOId peliculaDTO, int idPelicula)
        {
            var existe = await _context.Peliculas.AnyAsync(p => p.Id == idPelicula);
            if (!existe)
            {
                return NotFound("Pelicula no encontrada");
            }

            var pelicula = _mapper.Map<Pelicula>(peliculaDTO);
            _context.Add(pelicula);
            await _context.SaveChangesAsync();
            return Ok("Pelicula actualizada");
        }
    }
}
