﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIA_Equipo_11.DTO;
using PIA_Equipo_11.Entidades;

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
            var usuariodto = mapper.Map<List<UsuarioDTO>>(usuarios);
            return usuariodto;
        }

        
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


        // Modificar datos de usuario por su ID
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutUsuario(int id, UsuarioDTO usuariodto)
        {
            var existe = await dbContext.Usuarios.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            var usuario = mapper.Map<Usuario>(usuariodto);

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
