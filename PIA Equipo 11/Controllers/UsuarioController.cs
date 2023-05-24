using Microsoft.AspNetCore.Mvc;
using PIA_Equipo_11.Entidades;

namespace PIA_Equipo_11.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuarioController: ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Usuario>> GetUsuarios()
        {
            return new List<Usuario>()
            {
                new Usuario() { Id = 1, Nombre = "Ivan", Correo = "ivan@outlook.com", Telefono = "8123958235"},
                new Usuario() { Id = 2, Nombre = "Raymundo", Correo = "raymundo@gmail.com", Telefono = "8126574823"}
            };
        }
    }
}
