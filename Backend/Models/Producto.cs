namespace Backend.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Stock { get; set; }

        // Navegación
        public ICollection<DetalleFactura>? DetallesFactura { get; set; }
    }
}
