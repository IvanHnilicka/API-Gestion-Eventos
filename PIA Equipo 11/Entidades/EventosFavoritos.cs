using System.ComponentModel.DataAnnotations.Schema;

namespace PIA_Equipo_11.Entidades
{
    public class EventosFavoritos
    {
        
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        
        public int EventoId { get; set; }
        public Evento Evento { get; set; }

    }
}
