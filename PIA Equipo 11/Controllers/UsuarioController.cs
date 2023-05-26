using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            if (!existe)
            {
                return NotFound();
            }

            var correo = await dbContext.Usuarios.Where(x => x.Id == id).Select(y => y.Correo).FirstOrDefaultAsync();
            var cuenta = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == correo);
            dbContext.Users.Remove(cuenta);
            await dbContext.SaveChangesAsync();

            dbContext.Remove(new Usuario { Id = id });
            await dbContext.SaveChangesAsync();

            return Ok();
        }


        //Historial de asistencia
        [HttpGet("asistencia")]
        public async Task<ActionResult> GetHistorialAsistencia()
        {
            var usuarioId = getIdLogeado();

            //Obtiene los eventos asistidos y registrados
            var eventosAsistidos = await dbContext.RegistroEventos.Where(x => x.UsuarioId == usuarioId && x.Asistencia == true).Select(x => x.Evento).ToListAsync();
            var eventosRegistrados = await dbContext.RegistroEventos.Where(x => x.UsuarioId == usuarioId && x.Asistencia == false).Select(x => x.Evento).ToListAsync();


            List<InformacionEventoDTO> eventosAsistidosDTO = mapper.Map<List<InformacionEventoDTO>>(eventosAsistidos);
            List<InformacionEventoDTO> eventosRegistradosDTO = mapper.Map<List<InformacionEventoDTO>>(eventosRegistrados);


            //Crea un objeto anonimo para imprimirlos
            var objetoanonimo = new
            {
                Asistencia = eventosAsistidosDTO,
                Registro = eventosRegistradosDTO
            };

            return Ok(objetoanonimo);
        }


        // Añadir a favoritos
        [HttpPost("agregarFavorito")]
        public async Task<ActionResult> PostFavoritos(string nombre)
        {
            //Obtiene el evento
            var evento = await dbContext.Eventos.Where(evento => evento.Nombre == nombre).FirstOrDefaultAsync();

            //Obtiene el usuario
            var usuarioId = getIdLogeado();

            var datosFavorito = new EventosFavoritos
            {
                UsuarioId = usuarioId,
                EventoId = evento.Id,
            };


            dbContext.Add(datosFavorito);
            await dbContext.SaveChangesAsync();
            return Ok("Añadido a Favoritos exitosamente");
        }


        // Mostrar favoritos
        [HttpGet("eventosFavoritos/lista")]
        public async Task<List<InformacionEventoDTO>> GetFavoritos()
        {
            var usuarioId = getIdLogeado();
            var eventosFavoritos = await dbContext.EventosFavoritos.Where(ef => ef.UsuarioId == usuarioId).Select(x => x.Evento).ToListAsync();
            List<InformacionEventoDTO> eventosFavoritosDTO = mapper.Map<List<InformacionEventoDTO>>(eventosFavoritos);

            return eventosFavoritosDTO;
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
