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
            var actorDTO = resultado.Value as ActorDTOId;
            if (actorDTO == null)
            {
                var actoresDTO = resultado.Value as List<ActorDTOId> ?? throw new ArgumentNullException("Se esperaba una instancia de actorDTO");
                actoresDTO.ForEach(async actor => await _enlaces.GenerarElancesActor(actor));
                resultado.Value = actoresDTO;
            }
            else
            {
                await _enlaces.GenerarElancesActor(actorDTO);
            }
        
            await next();
        }
    }
}
