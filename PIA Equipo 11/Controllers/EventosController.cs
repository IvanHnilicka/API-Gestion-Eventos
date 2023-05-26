using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIA_Equipo_11.Entidades;
using System.IdentityModel.Tokens.Jwt;
using PIA_Equipo_11.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Runtime.CompilerServices;

namespace PIA_Equipo_11.Controllers
{
    [ApiController]
    [Route("api/eventos")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EventosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<EventosController> logger;
        private readonly IMapper mapper;

        public EventosController(ApplicationDbContext context, ILogger<EventosController> logger, IMapper mapper)
        {
            this.mapper = mapper;
            dbContext = context;
            this.logger = logger;
        }


        // Muestra lista de eventos
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<InformacionEventoDTO>>> GetEventos()
        {
            var eventos = await dbContext.Eventos.Include(x => x.Organizador).Include(x => x.ComentariosUsuario).ToListAsync();
            List<InformacionEventoDTO> eventosDTO = mapper.Map<List<InformacionEventoDTO>>(eventos);
            return Ok(eventosDTO);
        }


        // Buscar eventos por nombre
        [AllowAnonymous]
        [HttpGet("nombre/{nombre}")]
        public async Task<List<Evento>> GetEventoNombre(string nombre)
        {
            var eventos = await dbContext.Eventos.Where(evento => evento.Nombre.Contains(nombre)).ToListAsync();
            return eventos;
        }


        // Buscar eventos por fecha
        [HttpGet("fecha/{fecha}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Evento>>> GetEventoFecha(string fecha)
        {
            try
            {
                // Convierte la fecha de string a DateTime
                var fechaConvertida = DateTime.Parse(fecha);

                // Busca los eventos con esa fecha
                var eventos = await dbContext.Eventos.Where(evento => evento.Fecha.Date.Equals(fechaConvertida.Date)).ToListAsync();
                return eventos;

            } catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return BadRequest("Ingrese un formato de fecha valido (DD-MM-AAAA)");
            }
        }


        // Buscar evento por su ubicacion
        [HttpGet("ubicacion/{ubicacion}")]
        [AllowAnonymous]
        public async Task<List<Evento>> GetEventoUbicacion(string ubicacion)
        {
            var eventos = await dbContext.Eventos.Where(evento => evento.Ubicacion.Contains(ubicacion)).ToListAsync();
            return eventos;
        }


        [HttpGet("{nombre}/comentarios")]
        public async Task<ActionResult<List<ComentariosDTO>>> GetComentarios(string nombre)
        {
            var evento = await dbContext.Eventos.FirstOrDefaultAsync(x => x.Nombre == nombre);
            var comentarios = await dbContext.ComentariosUsuarios.Where(x => x.EventoId == evento.Id).ToListAsync();

            return Ok(comentarios);
        }


        /*
        [HttpGet("ObtenerEmail")]        
        public IActionResult GetEmail()
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
                    return Ok(email);
                }
            }            

            return NotFound("No se ha podido obtener el email");
        }
        */


        // Crear evento
        [HttpPost]        
        public async Task<ActionResult> PostEvento(EventoDTO eventodto)
        {
            try
            {
                var IdOrganizador = getIdLogeado();

                // Formatea la fecha a guardar en la base de datos
                var fecha = eventodto.Fecha.ToString("dd-MM-yyyy, HH:mm");
                var fechaFormateada = DateTime.Parse(fecha);
                eventodto.Fecha = fechaFormateada;

                //Mappear los eventodto a Evento y guardarlo en evento
                var evento = mapper.Map<Evento>(eventodto);
                evento.UsuarioId = IdOrganizador;
                dbContext.Add(evento);
                await dbContext.SaveChangesAsync();
                return Ok();
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return BadRequest("Error. "+ ex);
            }
        }


        // Crear comentarios de un evento
        [HttpPost("{nombre}/comentar")]
        public async Task<ActionResult> PutComentario(string nombre, string comentario)
        {
            var evento = await dbContext.Eventos.FirstOrDefaultAsync(x => x.Nombre == nombre);
            if (evento == null)
            {
                return BadRequest("No se encuentra nombre de evento");
            }

            var datosComentario = new ComentariosUsuario
            {
                UsuarioId = getIdLogeado(),
                EventoId = evento.Id,
                Comentario = comentario
            };

            dbContext.ComentariosUsuarios.Add(datosComentario);
            await dbContext.SaveChangesAsync();
            return Ok();
        }


        // Actualizar datos de un evento por su ID
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, EventoDTO eventoDTO)
        {
            var idLogeado = getIdLogeado();
            var idOrganizador = await dbContext.Eventos.Where(x => x.Id == id).Select(y => y.UsuarioId).FirstOrDefaultAsync();

            var exist = await dbContext.Eventos.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            if(idOrganizador != idLogeado)
            {
                return BadRequest("No puedes modificar este evento");
            }

            var evento = mapper.Map<Evento>(eventoDTO);
            evento.Id = id;
            evento.UsuarioId = getIdLogeado();

            dbContext.Update(evento);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }


        // Elimina evento por su ID
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteEvento(int id)
        {
            var existe = await dbContext.Eventos.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            dbContext.Remove(new Evento { Id = id });
            await dbContext.SaveChangesAsync();
            return Ok();
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

            logger.LogError("Error al buscar ID logeado");
            return -1;
        }





        //Lista de registro a eventos
        [HttpGet("registro")]
        public async Task<List<RegistroEventos>> GetRegistroEventos()
        {
            return await dbContext.RegistroEventos.Include(x => x.Evento).Include(y => y.Usuario).ToListAsync();

        }

        // Registro a eventos
        [HttpPost("registro/{nombre}")]
        public async Task<ActionResult> PostRegistro(string nombre)
        {
            //Obtiene el evento
            var evento = await dbContext.Eventos.Where(evento => evento.Nombre == nombre).FirstOrDefaultAsync();
            var capacidad = evento.Capacidad;
            var registros = dbContext.Eventos.Where(evento => evento.Nombre == nombre).Count() - 1;
            if(capacidad - registros == 0)
            {
                return BadRequest("No hay lugares disponibles para el evento");
            }

            //Obtiene el usuario
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email");

                if (emailClaim != null)
                {
                    var email = emailClaim.Value;
                    var usuario = dbContext.Usuarios.Where(u => u.Correo == email).FirstOrDefault();
                    var datosRegistro = new RegistroEventos
                    {
                        UsuarioId = usuario.Id,
                        EventoId = evento.Id,
                        Asistencia = false,
                        Costo = evento.Precio,
                        Pagado = false,

                        /*Evento = eventos,
                        Usuario = usuario*/
                    };
                    dbContext.Add(datosRegistro);
                    await dbContext.SaveChangesAsync();
                    return Ok(usuario);

                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest("No se ha podido obtener la autorizacion. ");
            }
        }

        //Borrar registro a evento
        [HttpDelete("registro")]
        public async Task<ActionResult> DeleteRegistro(string nombre)
        {
            //Obtiene el evento
            var eventos = await dbContext.Eventos.Where(evento => evento.Nombre.Contains(nombre)).FirstOrDefaultAsync();

            //Obtiene el usuario            
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email");

                if (emailClaim != null)
                {
                    var email = emailClaim.Value;
                    var usuario = dbContext.Usuarios.Where(u => u.Correo == email).FirstOrDefault();

                    dbContext.Remove(new RegistroEventos { UsuarioId = usuario.Id, EventoId = eventos.Id });
                    await dbContext.SaveChangesAsync();
                    return Ok();

                }
                return Ok("Email null");
            }
            return Ok("Error en el token");
        }


        //Asistir a evento
        [HttpPut("asistencia")]
        public async Task<ActionResult> asistir(string nombre)
        {
            var exist = await dbContext.Eventos.AnyAsync(x => x.Nombre == nombre);
            if (!exist)
            {
                return NotFound();
            }
            //Obtiene el eventoid
            var evento = await dbContext.Eventos.Where(evento => evento.Nombre.Contains(nombre)).FirstOrDefaultAsync();
            var eventoId = evento.Id;
            //Obtiene el usuarioid
            var usuarioId = getIdLogeado();
            //Obtiene el registro
            var registroEvento = dbContext.RegistroEventos.Where(x => x.EventoId == eventoId && x.UsuarioId == usuarioId).FirstOrDefault();

            if(registroEvento == null) 
            {
                return BadRequest("No se encuentra registrado en el evento");
            }

            registroEvento.Asistencia = true;
            registroEvento.Pagado = true;

            dbContext.Update(registroEvento);
            await dbContext.SaveChangesAsync();
            return Ok(registroEvento.Costo);
        }


        //Eventos populares
        [HttpGet("popular")]
        public async Task<ActionResult> GetEventosPopulares()
        {
            var eventosBaratos = await dbContext.Eventos.Where(x => x.Precio <= 500).ToListAsync();
            List<InformacionExtraEventoDTO> eventosBaratosDTO = mapper.Map<List<InformacionExtraEventoDTO>>(eventosBaratos);

            var eventosGrandes = await dbContext.Eventos.Where(x => x.Capacidad >= 2000).ToListAsync();
            List<InformacionExtraEventoDTO> eventosGrandesDTO = mapper.Map<List<InformacionExtraEventoDTO>>(eventosGrandes);

            var objetoanonimo = new
            {
                Baratos = eventosBaratosDTO,
                Masivos = eventosGrandesDTO
            };

            return Ok(objetoanonimo);
        }
    }
}
