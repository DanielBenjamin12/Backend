using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Contex
{
    public class AppDBContex: DbContext
    {
        public AppDBContex(DbContextOptions<AppDBContex> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<DetalleFactura> DetalleFacturas { get; set; }
    }
}
