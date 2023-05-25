using Microsoft.AspNetCore.Authorization;
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


        // Muestra lista de usuarios
        [HttpGet]
        public async Task<List<Usuario>> GetUsuarios()
        {
            var usuarios = await dbContext.Usuarios.ToListAsync();
            return usuarios;
        }


        // Crear usuario
        [HttpPost]
        public async Task<ActionResult> PostUsuario(Usuario usuario)
        {
            var correoRegistrado = await dbContext.Usuarios.AnyAsync(x => x.Correo ==  usuario.Correo);

            // Valida que no exista correo en base de datos
            if (correoRegistrado)
            {
                return BadRequest("El correo ya se encuentra registrado");
            }

            dbContext.Add(usuario);
            await dbContext.SaveChangesAsync();

            return Ok();
        }


        // Modificar datos de usuario por su ID
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
    }
}
