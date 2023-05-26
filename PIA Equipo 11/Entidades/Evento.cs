using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace PIA_Equipo_11.Entidades
{
    public class Evento
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public string Ubicacion { get; set; }
        public int Capacidad { get; set; }
        public int Precio { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Organizador { get; set; }
        public List<RegistroEventos> RegistroEventos { get; set; }
        public List<EventosFavoritos> EventosFavoritos { get; set; }
        public List<ComentariosUsuario> ComentariosUsuario { get; set; }
        public List<Codigo> Codigo { get; set; }
    }
}