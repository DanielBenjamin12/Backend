namespace Backend.Model
{
    public class Factura
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public decimal TotalBruto { get; set; }
        public decimal Impuestos { get; set; }
        public decimal TotalNeto { get; set; }

        // Navegación
        public Cliente? Cliente { get; set; }
        public ICollection<DetalleFactura>? Detalles { get; set; }
    }
}
