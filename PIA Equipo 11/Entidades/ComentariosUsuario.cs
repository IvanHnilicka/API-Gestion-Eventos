namespace PIA_Equipo_11.Entidades
{
    public class ComentariosUsuario
    {
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public int EventoId { get; set; }
        public Evento Evento { get; set; }
        public string Comentario { get; set; }

    }
}
