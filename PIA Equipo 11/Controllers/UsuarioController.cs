using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIA_Equipo_11.Entidades;

namespace PIA_Equipo_11.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuarioController: ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public UsuarioController(ApplicationDbContext context, IConfiguration configuration)
        {
            dbContext = context;
        }

        [HttpGet]
        public async Task<List<Usuario>> GetUsuarios()
        {
            var usuarios = await dbContext.Usuarios.ToListAsync();
            return usuarios;
        }

        [HttpPost]
        public async Task<ActionResult> PostUsuario(Usuario usuario)
        {
            dbContext.Add(usuario);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutUsuario(int id, Usuario usuario)
        {
            var existe = await dbContext.Usuarios.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            dbContext.Update(usuario);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

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
    }
}
