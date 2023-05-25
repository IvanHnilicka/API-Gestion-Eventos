using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIA_Equipo_11.DTO;
using PIA_Equipo_11.Entidades;

namespace PIA_Equipo_11.Controllers
{
    [ApiController]
    [Route("api/eventos")]
    public class EventosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        //Añadir el mapper
        private readonly IMapper mapper;

        public EventosController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
        {
            //Añadir el mapper
            this.mapper = mapper;
            dbContext = context;
        }


        // Muestra lista de eventos
        [HttpGet]
        public async Task<List<Evento>> GetEventos()
        {

            var eventos = await dbContext.Eventos.ToListAsync();
            return eventos;
        }


        // Buscar eventos por nombre
        //Manda el DTO con el id del organizador
        [HttpGet("nombre/{nombre}")]
        public async Task<List<EventoDTO>> GetEventoNombre(string nombre)
        {
            var eventos = await dbContext.Eventos.Where(evento => evento.Nombre.Contains(nombre)).ToListAsync();
            var organizador = await dbContext.Usuarios.Where(u => eventos.Select(e => e.UsuarioId).Contains(u.Id)).Select(u => new
            {
                u.Nombre
            }).ToListAsync();
            
            var eventodto = mapper.Map<List<EventoDTO>>(eventos);

            /*var eventosConUsuario = eventos.Join(organizador,
                evento => evento.UsuarioId,
                organizador => organizador,
                (evento, organizador) => new { Evento = evento, Organizador = organizador })
                .ToList();*/

            return eventodto;
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

            }
            catch
            {
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
        public async Task<ActionResult> PostEvento(EventoDTO eventodto)
        {
            try
            {
                // Formatea la fecha a guardar en la base de datos
                var fecha = eventodto.Fecha.ToString("dd-MM-yyyy, HH:mm:ss");
                var fechaFormateada = DateTime.Parse(fecha);
                eventodto.Fecha = fechaFormateada;

                //Mappear los eventodto a Evento y guardarlo en usuario
                var evento = mapper.Map<Evento>(eventodto);                
                dbContext.Add(evento);
                await dbContext.SaveChangesAsync();

                return Ok();
            }
            catch
            {
                return BadRequest("Ingrese un formato de fecha valido");
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