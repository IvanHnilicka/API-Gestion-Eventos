namespace PIA_Equipo_11.Entidades
{
    public class Codigo
    {
        public int Id { get; set; }
        public string CodigoPromocional { get; set; }
        public float Descuento { get; set; }
        public bool Valido { get; set; }
        public int EventoId { get; set; }
        public Evento Evento { get; set; }

    }
}
