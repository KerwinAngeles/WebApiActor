using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiActor.Utilidades
{
    public class HATEOSFiltroAtribute : ResultFilterAttribute
    {
        protected bool DebeIncluirHATEOS(ResultExecutingContext context)
        {
            var resultado = context.Result as ObjectResult;
            if(!RespuestaExitosas(resultado))
            {
                return false;
            }
            var cabecera = context.HttpContext.Request.Headers["incluirHATEOAS"];
            
            if (cabecera.Count == 0)
            {
                return false;
            }

            var valorCabecera = cabecera[0];
            if (!valorCabecera.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }

        private bool RespuestaExitosas(ObjectResult result)
        {
            if(result == null || result.Value == null)
            {
                return false;
            }

            if(result.StatusCode.HasValue && !result.StatusCode.Value.ToString().StartsWith("2"))
            {
                return false;
            }
            return true;
        }
    }
}
