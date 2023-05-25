using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIA_Equipo_11.Entidades;
using PIA_Equipo_11.DTO;
using AutoMapper;

namespace PIA_Equipo_11.Controllers{ 
    [Route("api/usuarios")]
public class UsuarioController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    //Añadir el mapper
    private readonly IMapper mapper;

    public UsuarioController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
    {
            //Añadir el mapper
            this.mapper = mapper;            
            dbContext = context;
    }

    [HttpGet]   
    public async Task<List<Usuario>> GetUsuarios()
    {
        var usuarios = await dbContext.Usuarios.ToListAsync();
        return usuarios;
    }
        
    [HttpGet]
    [Route("api/usuarios/GetSinID")]
    public async Task<List<UsuarioDTO>> GetUsuariosSinID()
        {
            var usuarios = await dbContext.Usuarios.ToListAsync();
            var usuariodto = mapper.Map<List<UsuarioDTO>>(usuarios);
            return usuariodto;
     }

     [HttpPost]
    public async Task<ActionResult> PostUsuario(UsuarioDTO usuariodto)
    {
        var existeUsuario = await dbContext.Usuarios.AnyAsync(x => x.Nombre == usuariodto.Nombre);
        if (existeUsuario)
            {
                return BadRequest($"Ya existe un usuario con el nombre {usuariodto.Nombre}");
            }

        //Mappear los usuariodto a Usuario y guardarlo en usuario
        var usuario = mapper.Map<Usuario>(usuariodto);
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
        if (!existe)
        {
            return NotFound();
        }

        dbContext.Remove(new Usuario { Id = id });
        await dbContext.SaveChangesAsync();
        return Ok();
    }
}
}