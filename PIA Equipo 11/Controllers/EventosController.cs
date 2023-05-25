using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIA_Equipo_11.Entidades;

namespace PIA_Equipo_11.Controllers
{
    [ApiController]
    [Route("api/eventos")]
    public class EventosController: ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public EventosController(ApplicationDbContext context, IConfiguration configuration)
        {
            dbContext = context;
        }

        [HttpGet]
        public async Task<List<Evento>> GetEventos()
        {
            var eventos = await dbContext.Eventos.ToListAsync();
            return eventos;
        }

        [HttpPost]
        public async Task<ActionResult> PostEvento(Evento evento)
        {
            dbContext.Add(evento);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

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
