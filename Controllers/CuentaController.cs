using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApiActor.DTO;
using WebApiActor.Services.Interfaces;

namespace WebApiActor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentaController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRespuestaAutentication _respuesta;
        private readonly SignInManager<IdentityUser> _signInManager;
        public CuentaController(UserManager<IdentityUser> userManager, IRespuestaAutentication respuesta, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _respuesta = respuesta;
            _signInManager = signInManager;

        }
        [HttpPost("Registro")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registro(CredencialesDTO credenciales)
        {
            var usuario = new IdentityUser { UserName = credenciales.Email, Email = credenciales.Email };
            var resultado = await _userManager.CreateAsync(usuario, credenciales.Password);
            if(resultado.Succeeded)
            {
                return await _respuesta.CreacionToken(credenciales);
            }
            else
            {
                return BadRequest(resultado.Errors);   
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesDTO credenciales)
        {
            var resultado = await _signInManager.PasswordSignInAsync(credenciales.Email, credenciales.Password,
                isPersistent: false, lockoutOnFailure: false);

            if(resultado.Succeeded)
            {
                return await _respuesta.CreacionToken(credenciales);
            }
            else
            {
                return BadRequest("Login Incorrecto");
            }
        }

        [HttpGet("NuevoToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Renovar()
        {
            var emailClain = HttpContext.User.Claims.Where(c => c.Type == "email").FirstOrDefault();
            var email = emailClain.Value;

            CredencialesDTO credenciales = new CredencialesDTO()
            {
                Email = email
                
            };

            return await _respuesta.CreacionToken(credenciales);
        }

        [HttpPost("CreateAdmin")]
        public async Task<ActionResult> CrearAdmin(AdminDTO adminDTO)
        {
            var usuario = await _userManager.FindByEmailAsync(adminDTO.email);
            await _userManager.AddClaimAsync(usuario, new Claim("Admin", "true"));
            return NoContent();
        }

        [HttpPost("RemoveAdmin")]
        public async Task<ActionResult> RemoverAdmin(AdminDTO adminDTO)
        {
            var usuario = await _userManager.FindByEmailAsync(adminDTO.email);
            await _userManager.RemoveClaimAsync(usuario, new Claim("Admin", "true"));
            return NoContent();
        }
    }
}
