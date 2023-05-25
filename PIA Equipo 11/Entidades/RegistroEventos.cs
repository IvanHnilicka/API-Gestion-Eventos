using System.ComponentModel.DataAnnotations.Schema;

namespace PIA_Equipo_11.Entidades
{
    public class RegistroEventos
    {
        
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public int EventoId { get; set; }
        public Evento Evento { get; set; }
        public bool Asistencia { get; set; }
        public float Costo { get; set; }
        public bool Pagado { get; set; }
    }
}
