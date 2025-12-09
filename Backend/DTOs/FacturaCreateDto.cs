namespace Backend.Dtos
{
    public class FacturaLineaCreateDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
    public class FacturaCreateDto
    {
        public int ClienteId { get; set; }
        public List<FacturaLineaCreateDto> Lineas { get; set; } = new();
        public string? Comentarios { get; set; }
    }
}