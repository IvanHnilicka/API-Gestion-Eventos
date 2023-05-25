namespace PIA_Equipo_11.DTO
{
    public class EventoDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public string Ubicacion { get; set; }
        public int Capacidad { get; set; }
        public int Precio { get; set; }
        public int UsuarioId { get; set; }
    }
}
