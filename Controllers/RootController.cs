using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiActor.Models;

namespace WebApiActor.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService _service;

        public RootController(IAuthorizationService service) 
        {
            _service = service;
        }
        [AllowAnonymous]
        [HttpGet(Name = "ObtenerRoot")]

        public async Task<ActionResult<List<DatoHateoas>>> ObtenerRutas()
        {
            var datosHateos = new List<DatoHateoas>();
            var admin = await _service.AuthorizeAsync(User, "Admin");

            datosHateos.Add(new DatoHateoas(enlace: Url.Link("ObtenerRoot", new { }),
                descripcion: "self", metodo: "GET"));

            datosHateos.Add(new DatoHateoas(enlace: Url.Link("ObtenerActores", new { }),
                descripcion: "Actores", metodo: "GET"));

            datosHateos.Add(new DatoHateoas(enlace: Url.Link("ObtenerPeliculas", new { }),
               descripcion: "Pelicula", metodo: "GET"));

            if(admin.Succeeded)
            {
                datosHateos.Add(new DatoHateoas(enlace: Url.Link("CrearActor", new { }),
                    descripcion: "Actores-crear", metodo: "POST"));

                datosHateos.Add(new DatoHateoas(enlace: Url.Link("CrearPelicula", new { }),
                    descripcion: "Pelicula-crear", metodo: "POST"));
            }

            return datosHateos;
        }

    }
}
