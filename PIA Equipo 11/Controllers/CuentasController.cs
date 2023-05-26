using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PIA_Equipo_11.DTO;
using PIA_Equipo_11.Entidades;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PIA_Equipo_11.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController: ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public CuentasController(ApplicationDbContext context,UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager, IMapper mapper)
        {
            this.dbContext = context;
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.mapper = mapper;
        }


        [HttpPost("registro")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credenciales)
        {

            var usuario = new IdentityUser { UserName = credenciales.Email, Email = credenciales.Email };
            var resultado = await userManager.CreateAsync(usuario, credenciales.Password);

            if (resultado.Succeeded)
            {
                /*
                var usuarioController = new UsuarioController(dbContext, mapper);
                var datosUsuario = new UsuarioDTO
                {
                    Nombre = credenciales.Nombre,
                    Telefono = credenciales.Telefono,
                    Correo = credenciales.Email
                };

                await usuarioController.PostUsuario(datosUsuario);
                */

                var correoRegistrado = await dbContext.Usuarios.AnyAsync(x => x.Correo == credenciales.Email);

                // Valida que no exista correo en base de datos
                if (correoRegistrado)
                {
                    return BadRequest("El correo ya se encuentra registrado");
                }

                var datosUsuario = new Usuario
                {
                    Nombre = credenciales.Nombre,
                    Telefono = credenciales.Telefono,
                    Correo = credenciales.Email
                };

                dbContext.Usuarios.Add(datosUsuario);
                await dbContext.SaveChangesAsync();

                return generarTokenRegistro(credenciales);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }


        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesLoginDTO credenciales)
        {
            var result = await signInManager.PasswordSignInAsync(credenciales.Email,
                credenciales.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return generarTokenLogin(credenciales);
            }
            else
            {
                return BadRequest("No ha sido posible iniciar sesion");
            }
        }


        [HttpGet]
        public async Task<List<IdentityUser>> GetCredenciales()
        {
            var credenciales = await dbContext.Users.ToListAsync();
            return credenciales;
        }


        private RespuestaAutenticacion generarTokenRegistro(CredencialesUsuario credenciales)
        {
            var claims = new List<Claim> 
            { 
                new Claim("email", credenciales.Email),
                new Claim("fecha", DateTime.Now.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyjwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.Now.AddHours(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds);

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiration
            };
        }


        private RespuestaAutenticacion generarTokenLogin(CredencialesLoginDTO credenciales)
        {
            var claims = new List<Claim>
            {
                new Claim("email", credenciales.Email),
                new Claim("fecha", DateTime.Now.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyjwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.Now.AddHours(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds);

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiration
            };
        }
    }
}
