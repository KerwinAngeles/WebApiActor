using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiActor.DTO;
using WebApiActor.Services;

namespace WebApiActor.Utilidades
{
    public class HATEOSActorFilterAttribute : HATEOSFiltroAtribute
    {
        private readonly GeneradorEnlaces _enlaces;
        public HATEOSActorFilterAttribute(GeneradorEnlaces enlaces) 
        {
            _enlaces = enlaces;
        }
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var debeIncluir = DebeIncluirHATEOS(context);
            if(!debeIncluir)
            {
                await next();
                return;
            }

            var resultado = context.Result as ObjectResult;
            var modelo = resultado.Value as ActorDTOId ?? throw new ArgumentNullException("Se esperaba una instancia de actorDTO");
            await _enlaces.GenerarElancesActor(modelo);
            await next();
        }
    }
}
