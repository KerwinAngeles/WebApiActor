using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using WebApiActor.DTO;
using WebApiActor.Models;

namespace WebApiActor.Services
{
    public class GeneradorEnlaces
    {
        private readonly IAuthorizationService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;

        public GeneradorEnlaces(IAuthorizationService service, IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor)
        {
            _service = service;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper ConstruirHelper()
        {
            var factoria = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factoria.GetUrlHelper(_actionContextAccessor.ActionContext);
        }

        private async Task<bool> Admin()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var resultado = await _service.AuthorizeAsync(httpContext.User, "Admin");
            return resultado.Succeeded;

        }
         
        public async Task GenerarElancesActor(ActorDTOId actorDTO)
        {
            var admin = await Admin();
            var Url = ConstruirHelper();

            actorDTO.Enlaces.Add(new DatoHateoas(
                enlace: Url.Link("ObtenerActoresPorId", new { id = actorDTO.Id }),
                descripcion: "self",
                metodo: "GET"));

            if (admin)
            {
                actorDTO.Enlaces.Add(new DatoHateoas(
                enlace: Url.Link("ActualizarActor", new { id = actorDTO.Id }),
                descripcion: "actualizar-actor",
                metodo: "PUT"));
                actorDTO.Enlaces.Add(new DatoHateoas(
                    enlace: Url.Link("EliminarActor", new { id = actorDTO.Id }),
                    descripcion: "eliminar-actor",
                    metodo: "DELETE"));
            }
        }
    }
}
