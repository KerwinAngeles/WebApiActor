using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;


        public RespuestaAutenticactionServices(IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<RespuestaAutenticacionDTO> CreacionToken(CredencialesDTO credenciales)
        {
            var claims = new List<Claim>
            {
                new Claim("email", credenciales.Email)
            };

            var usuario = await  _userManager.FindByEmailAsync(credenciales.Email);
            var ClaimDB = await _userManager.GetClaimsAsync(usuario);

            claims.AddRange(ClaimDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["keyjwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddMinutes(7);
            var securityToke = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);
            

            return new RespuestaAutenticacionDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToke),
                Expiracion = expiracion
            };

        }
    }
}
