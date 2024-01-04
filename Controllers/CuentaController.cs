using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registro(CredencialesDTO credenciales)
        {
            var usuario = new IdentityUser { UserName = credenciales.Email, Email = credenciales.Email };
            var resultado = await _userManager.CreateAsync(usuario, credenciales.Password);
            if(resultado.Succeeded)
            {
                return _respuesta.CreacionToken(credenciales);
            }
            else
            {
                return BadRequest(resultado.Errors);   
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesDTO credenciales)
        {
            var resultado = await _signInManager.PasswordSignInAsync(credenciales.Email, credenciales.Password,
                isPersistent: false, lockoutOnFailure: false);

            if(resultado.Succeeded)
            {
                return _respuesta.CreacionToken(credenciales);
            }
            else
            {
                return BadRequest("Login Incorrecto");
            }
        }
    }
}
