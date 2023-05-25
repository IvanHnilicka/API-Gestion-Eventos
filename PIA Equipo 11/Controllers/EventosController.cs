using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIA_Equipo_11.Entidades;

namespace PIA_Equipo_11.Controllers
{
    [ApiController]
    [Route("api/eventos")]
    public class EventosController: ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ILogger<EventosController> logger;

        public EventosController(ApplicationDbContext context, ILogger<EventosController> logger)
        {
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


        // Crear evento
        [HttpPost]
        public async Task<ActionResult> PostEvento(Evento evento)
        {
            /*
            // Busca el usuario logeado para registrarlo como creador del evento
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var organizador = await userManager.FindByEmailAsync(email);
            evento.UsuarioId = organizador.Id;
            */

            try
            {
                // Formatea la fecha a guardar en la base de datos
                var fecha = evento.Fecha.ToString("dd-MM-yyyy, HH:mm");
                var fechaFormateada = DateTime.Parse(fecha);
                evento.Fecha = fechaFormateada;

                dbContext.Add(evento);
                await dbContext.SaveChangesAsync();

                return Ok();
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return BadRequest("Error. " + ex);
            }
        }


        // Modificar datos del evento por su ID
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutEvento(int id, Evento evento)
        {
            var existe = await dbContext.Eventos.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

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
