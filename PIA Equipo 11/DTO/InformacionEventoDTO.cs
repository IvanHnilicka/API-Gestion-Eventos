namespace PIA_Equipo_11.DTO
{
    public class InformacionEventoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public string Ubicacion { get; set; }
        public UsuarioDTO Organizador { get; set; }
    }
}
