using System.ComponentModel.DataAnnotations;

namespace PIA_Equipo_11.DTO
{
    public class EventoDTO
    {
        [Required(ErrorMessage = "Ingrese un nombre para el evento")]
        public string Nombre { get; set; }
        [MaxLength(255, ErrorMessage = "La longitud maxima para la descripcion es de 255 caracteres")]
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public string Ubicacion { get; set; }
        public int Capacidad { get; set; }
        public int Precio { get; set; }
    }
}
