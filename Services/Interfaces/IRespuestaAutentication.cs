using WebApiActor.DTO;

namespace WebApiActor.Services.Interfaces
{
    public interface IRespuestaAutentication
    {
        Task<RespuestaAutenticacionDTO> CreacionToken (CredencialesDTO credenciales);
    }
}
