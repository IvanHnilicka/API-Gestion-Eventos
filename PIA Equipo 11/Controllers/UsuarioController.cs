using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIA_Equipo_11.DTO;
using PIA_Equipo_11.Entidades;
using System.IdentityModel.Tokens.Jwt;

namespace PIA_Equipo_11.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuarioController: ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public UsuarioController(ApplicationDbContext context, IMapper mapper)
        {
            this.mapper = mapper;
            dbContext = context;
        }

        // Muestra la lista de usuarios
        [HttpGet]
        public async Task<List<UsuarioDTO>> GetUsuarios()
        {
            var usuarios = await dbContext.Usuarios.ToListAsync();
            var usuarioDTO = mapper.Map<List<UsuarioDTO>>(usuarios);
            return usuarioDTO;
        }

        
        /*
        // Crear usuario
        [HttpPost]
        public async Task<ActionResult> PostUsuario(UsuarioDTO usuariodto)
        {
            var correoRegistrado = await dbContext.Usuarios.AnyAsync(x => x.Correo ==  usuariodto.Correo);

            // Valida que no exista correo en base de datos
            if (correoRegistrado)
            {
                return BadRequest("El correo ya se encuentra registrado");
            }

            var usuario = mapper.Map<Usuario>(usuariodto);
            dbContext.Add(usuario);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
        */


        // Modificar datos de usuario
        [HttpPut]
        public async Task<ActionResult> PutUsuario(PutUsuarioDTO datosNuevos)
        {
            var usuario = await dbContext.Usuarios.FirstOrDefaultAsync(x => x.Id == getIdLogeado());

            usuario.Nombre = datosNuevos.Nombre;
            usuario.Telefono = datosNuevos.Telefono;

            dbContext.Update(usuario);
            await dbContext.SaveChangesAsync();
            return Ok();
        }


        // Eliminar usuario por su ID
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteUsuario(int id)
        {
            var existe = await dbContext.Usuarios.AnyAsync(x => x.Id == id);
            if(!existe)
            {
                return NotFound();
            }

            dbContext.Remove(new Usuario { Id = id });
            await dbContext.SaveChangesAsync();
            return Ok();
        }


        //Historial de asistencia
        [HttpGet("asistencia")]
        public async Task<ActionResult> GetHistorialAsistencia()
        {
            //Obtiene el usuarioid
            var usuarioId = getIdLogeado();

            //Obtiene los eventos asistidos y registrados
            var eventosAsistidos = await dbContext.RegistroEventos.Where(x => x.UsuarioId == usuarioId && x.Asistencia == true).Select(x => x.Evento).ToListAsync();
            var eventosRegistrados = await dbContext.RegistroEventos.Where(x => x.UsuarioId == usuarioId && x.Asistencia == false).Select(x => x.Evento).ToListAsync();

            //Los pasa a bonito uwu
            List<InformacionEventoDTO> eventosAsistidosDTO = mapper.Map<List<InformacionEventoDTO>>(eventosAsistidos);
            List<InformacionEventoDTO> eventosRegistradosDTO = mapper.Map<List<InformacionEventoDTO>>(eventosRegistrados);

            //Crea un objeto anonimo para imprimirlos
            var objetoanonimo = new
            {
                Asistencia = eventosAsistidosDTO,
                Registro = eventosRegistradosDTO
            };

            //Los imprime
            return Ok(objetoanonimo);

        }


        // Retorna el id del usuario que esta logeado
        private int getIdLogeado()
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email");

                if (emailClaim != null)
                {
                    var email = emailClaim.Value;
                    return dbContext.Usuarios.Where(u => u.Correo == email).FirstOrDefault().Id;
                }
            }

            return -1;
        }

    }
}
