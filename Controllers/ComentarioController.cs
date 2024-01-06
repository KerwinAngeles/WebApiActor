using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _user;

        public ComentarioController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> user)
        {
            _context = context;
            _mapper = mapper;
            _user = user;
        }

        [HttpGet(Name = "ObtenerComentarios")] // obteniendo todos los comentarios
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

        [HttpGet("{id}", Name = "ObtenerComentariosPorId")] // obteniendo un comentario por el id
        public async Task<ActionResult<ComentarioDTOId>> GetComentarioById(int id)
        {
            var comentario = await _context.Comentarios.FirstOrDefaultAsync(comentarioDB => comentarioDB.Id == id);
            if(comentario == null)
            {
                return NotFound();
            }

            return _mapper.Map<ComentarioDTOId>(comentario);
        }
            

        [HttpPost(Name = "CrearComentario")] // agregado un comentario
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> PostComentario([FromBody] ComentarioDTO comentarioDTO, int peliculaId)
        {
            var emailClain = HttpContext.User.Claims.Where(c => c.Type == "email").FirstOrDefault();
            var email = emailClain.Value;
            var usuario = await _user.FindByEmailAsync(email);
            var usuarioId = usuario.Id;

            var existePelicula = await _context.Peliculas.AnyAsync(p => p.Id == peliculaId);
            if(!existePelicula)
            {
                return NotFound();
            }

            var comentario = _mapper.Map<Comentario>(comentarioDTO);
            comentario.PeliculaId = peliculaId;
            comentario.UsuarioId = usuarioId;
            _context.Add(comentario);
            await _context.SaveChangesAsync();

            var entidadComentarioDTO = _mapper.Map<ComentarioDTOId>(comentario);
            return CreatedAtRoute("GetComentarioPorId", routeValues: new {id = comentario.Id}, value: entidadComentarioDTO);
        }

        [HttpPut("{id:int}", Name = "ActualizarComentario")] // actualizando comentario
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
