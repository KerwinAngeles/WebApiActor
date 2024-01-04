using WebApiActor.DTO;

namespace WebApiActor.Services.Interfaces
{
    public interface IRespuestaAutentication
    {
        RespuestaAutenticacionDTO CreacionToken (CredencialesDTO credenciales);
    }
}
