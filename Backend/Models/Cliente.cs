namespace Backend.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string RucNit { get; set; }
        public string? Direccion { get; set; }
        public string? Email { get; set; }

        // Navegación
        public ICollection<Factura>? Facturas { get; set; }
    }
}
