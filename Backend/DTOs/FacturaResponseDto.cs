namespace Backend.Dtos
{
    public class DetalleResponseDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string? ProductoNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
    public class FacturaResponseDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string? ClienteNombre { get; set; }
        public DateTime Fecha { get; set; }
        public decimal TotalBruto { get; set; }
        public decimal Impuestos { get; set; }
        public decimal TotalNeto { get; set; }
        public List<DetalleResponseDto> Detalles { get; set; } = new();
    }
}