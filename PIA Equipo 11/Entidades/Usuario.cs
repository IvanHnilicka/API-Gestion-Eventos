namespace PIA_Equipo_11.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public List<Evento> EventosCreados { get; set; }
        public List<RegistroEventos> RegistroEventos { get; set; }
        public List<EventosFavoritos> EventosFavoritos { get; set; }
        public List<ComentariosUsuario> ComentariosUsuario { get; set; }

    }
}
