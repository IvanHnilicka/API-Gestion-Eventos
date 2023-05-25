using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIA_Equipo_11.Entidades;
using System.IdentityModel.Tokens.Jwt;
using PIA_Equipo_11.DTO;

namespace PIA_Equipo_11.Controllers
{
    [ApiController]
    [Route("api/eventos")]
    public class EventosController: ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<EventosController> logger;
        //Añadir el mapper
        private readonly IMapper mapper;

        public EventosController(ApplicationDbContext context, ILogger<EventosController> logger, IMapper mapper)
        {
            this.mapper = mapper;
            dbContext = context;
            this.logger = logger;
        }


        // Muestra lista de eventos
        [HttpGet]
        public async Task<List<Evento>> GetEventos()
        {
            var eventos = await dbContext.Eventos.ToListAsync();
            return eventos;
        }


        // Buscar eventos por nombre
        [HttpGet("nombre/{nombre}")]
        public async Task<List<Evento>> GetEventoNombre(string nombre)
        {
            var eventos = await dbContext.Eventos.Where(evento => evento.Nombre.Contains(nombre)).ToListAsync();
            return eventos;
        }


        // Buscar eventos por fecha
        [HttpGet("fecha/{fecha}")]
        public async Task<ActionResult<List<Evento>>> GetEventoFecha(string fecha)
        {
            try
            {
                // Convierte la fecha de string a DateTime
                var fechaConvertida = DateTime.Parse(fecha);

                // Busca los eventos con esa fecha
                var eventos = await dbContext.Eventos.Where(evento => evento.Fecha.Date.Equals(fechaConvertida.Date)).ToListAsync();
                return eventos;

            }catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return BadRequest("Ingrese un formato de fecha valido (DD-MM-AAAA)");
            }
        }


        // Buscar evento por su ubicacion
        [HttpGet("ubicacion/{ubicacion}")]
        public async Task<List<Evento>> GetEventoUbicacion(string ubicacion)
        {
            var eventos = await dbContext.Eventos.Where(evento => evento.Ubicacion.Contains(ubicacion)).ToListAsync();
            return eventos;
        }


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


        // Crear evento
        [HttpPost]        
        public async Task<ActionResult> PostEvento(EventoDTO eventodto)
        {

            // Busca el usuario logeado para registrarlo como creador del evento
            //var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();

            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email");

                if (emailClaim != null)
                {
                    var email = emailClaim.Value;
                    var organizador = dbContext.Usuarios.Where(u => u.Correo == email).FirstOrDefault().Id;
                    eventodto.UsuarioId = organizador;                    
                }
            }                                                           

            try
            {
                // Formatea la fecha a guardar en la base de datos
                var fecha = eventodto.Fecha.ToString("dd-MM-yyyy, HH:mm");
                var fechaFormateada = DateTime.Parse(fecha);
                eventodto.Fecha = fechaFormateada;

                //Mappear los eventodto a Evento y guardarlo en evento
                var evento = mapper.Map<Evento>(eventodto);
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


        // Modificar datos del evento por su ID
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutEvento(int id, EventoDTO eventodto)
        {
            var existe = await dbContext.Eventos.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            //Mappear los eventodto a Evento y guardarlo en usuario
            var evento = mapper.Map<Evento>(eventodto);
            dbContext.Update(evento);
            await dbContext.SaveChangesAsync();
            return Ok();
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
    }
}
