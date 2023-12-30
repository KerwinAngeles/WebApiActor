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
    public class ComentarioController : ControllerBase
    {
        public readonly ApplicationDbContext _context;
        public readonly IMapper _mapper;
        public ComentarioController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTOId>>> GetComentarios(int peliculaId)
        {
            var existePelicula = await _context.Peliculas.AnyAsync(l => l.Id == peliculaId);
            if(!existePelicula)
            { 
                return NotFound();
            }

            var comentario = await _context.Comentarios.Where(c => c.PeliculaId == peliculaId).ToListAsync();
            return _mapper.Map<List<ComentarioDTOId>>(comentario);
        }

        [HttpGet("{id}", Name = "GetComentario")]
        public async Task<ActionResult<ComentarioDTOId>> GetComentarioById(int idComentario)
        {
            var comentario = await _context.Comentarios.FirstOrDefaultAsync(comentarioDB => comentarioDB.Id == idComentario);
            if(comentario == null)
            {
                return NotFound();
            }

            return _mapper.Map<ComentarioDTOId>(comentario);
        }
            

        [HttpPost]
        public async Task<ActionResult> PostComentario([FromBody] ComentarioDTO comentarioDTO, int peliculaId)
        {
            var existePelicula = await _context.Peliculas.AnyAsync(p => p.Id == peliculaId);
            if(!existePelicula)
            {
                return NotFound();
            }

            var comentario = _mapper.Map<Comentario>(comentarioDTO);
            comentario.PeliculaId = peliculaId;
            _context.Add(comentario);
            await _context.SaveChangesAsync();

            var entidadComentarioDTO = _mapper.Map<ComentarioDTOId>(comentario);
            return CreatedAtRoute("GetComentario", routeValues: new {id = comentario.Id}, value: entidadComentarioDTO);
        }
    }
}
