using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
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

        [HttpGet] // obteniendo todos los comentarios
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

        [HttpGet("{id}", Name = "GetComentario")] // obteniendo un comentario por el id
        public async Task<ActionResult<ComentarioDTOId>> GetComentarioById(int id)
        {
            var comentario = await _context.Comentarios.FirstOrDefaultAsync(comentarioDB => comentarioDB.Id == id);
            if(comentario == null)
            {
                return NotFound();
            }

            return _mapper.Map<ComentarioDTOId>(comentario);
        }
            

        [HttpPost] // agregado un comentario
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

        [HttpPut("{id:int}")] // actualizando comentario
        public async Task<ActionResult> PutComentario([FromBody] ComentarioDTO comentarioDTO, int peliculaId, int id)
        {
            var existePelicula = await _context.Peliculas.AnyAsync(p => p.Id == peliculaId);
            if (!existePelicula)
            {
                return NotFound();
            }

            var existeComentario = await _context.Comentarios.AnyAsync(c => c.Id == id);
            if (!existeComentario)
            {
                return NotFound();
            }

            var comentario = _mapper.Map<Comentario>(comentarioDTO);
            comentario.Id = id;
            comentario.PeliculaId = peliculaId;

            _context.Update(comentario);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}
