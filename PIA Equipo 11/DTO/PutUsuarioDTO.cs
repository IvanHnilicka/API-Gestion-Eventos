using PIA_Equipo_11.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PIA_Equipo_11.DTO
{
    public class PutUsuarioDTO
    {
        [Required(ErrorMessage = "Ingrese nombre")]
        [SoloLetras]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Ingrese telefono")]
        [SoloNumeros]
        public string Telefono { get; set; }
    }
}
