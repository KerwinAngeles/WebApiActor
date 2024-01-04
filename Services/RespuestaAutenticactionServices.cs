using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiActor.DTO;
using WebApiActor.Services.Interfaces;

namespace WebApiActor.Services
{
    public class RespuestaAutenticactionServices : IRespuestaAutentication
    {
        private readonly IConfiguration _configuration;

        public RespuestaAutenticactionServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public RespuestaAutenticacionDTO CreacionToken(CredencialesDTO credenciales)
        {
            var claims = new List<Claim>
            {
                new Claim("email", credenciales.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["keyjwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddYears(1);
            var securityToke = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);

            return new RespuestaAutenticacionDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToke),
                Expiracion = expiracion
            };

        }
    }
}
