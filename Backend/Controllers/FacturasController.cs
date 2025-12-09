using Backend.Contex;
using Backend.Dtos;
using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturasController : ControllerBase
    {
        private readonly AppDBContex _context;

        public FacturasController(AppDBContex context)
        {
            _context = context;
        }

        // GET: api/Facturas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacturaResponseDto>>> GetFacturas()
        {
            var facturas = await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.Detalles!)
                    .ThenInclude(d => d.Producto)
                .ToListAsync();

            var resp = facturas.Select(f => new FacturaResponseDto
            {
                Id = f.Id,
                ClienteId = f.ClienteId,
                ClienteNombre = f.Cliente?.Nombre,
                Fecha = f.Fecha,
                TotalBruto = f.TotalBruto,
                Impuestos = f.Impuestos,
                TotalNeto = f.TotalNeto,
                Detalles = f.Detalles?.Select(d => new DetalleResponseDto
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    ProductoNombre = d.Producto?.Nombre,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList() ?? new List<DetalleResponseDto>()
            }).ToList();

            return Ok(resp);
        }

        // GET: api/Facturas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FacturaResponseDto>> GetFactura(int id)
        {
            var f = await _context.Facturas
                .Include(x => x.Cliente)
                .Include(x => x.Detalles!)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (f == null) return NotFound();

            var dto = new FacturaResponseDto
            {
                Id = f.Id,
                ClienteId = f.ClienteId,
                ClienteNombre = f.Cliente?.Nombre,
                Fecha = f.Fecha,
                TotalBruto = f.TotalBruto,
                Impuestos = f.Impuestos,
                TotalNeto = f.TotalNeto,
                Detalles = f.Detalles?.Select(d => new DetalleResponseDto
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    ProductoNombre = d.Producto?.Nombre,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList() ?? new List<DetalleResponseDto>()
            };

             return Ok(dto);
        }

        // PUT: api/Facturas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFactura(int id, Factura factura)
        {
            if (id != factura.Id)
            {
                return BadRequest();
            }

            _context.Entry(factura).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacturaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Facturas
        [HttpPost]
        public async Task<ActionResult<FacturaResponseDto>> PostFactura([FromBody] FacturaCreateDto dto)
        {
            if (dto == null || dto.Lineas == null || !dto.Lineas.Any())
                return BadRequest(new { error = "La factura debe contener al menos una línea" });

            // Validar cliente
            var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
            if (cliente == null) return NotFound(new { error = "Cliente no encontrado" });

            // Cargar productos involucrados
            var productIds = dto.Lineas.Select(l => l.ProductoId).Distinct().ToList();
            var productos = await _context.Productos.Where(p => productIds.Contains(p.Id)).ToListAsync();
            var prodDict = productos.ToDictionary(p => p.Id);

            // Validar existencia y stock
            foreach (var linea in dto.Lineas)
            {
                if (!prodDict.ContainsKey(linea.ProductoId))
                    return NotFound(new { error = $"Producto {linea.ProductoId} no encontrado" });
                if (linea.Cantidad <= 0)
                    return BadRequest(new { error = "Cantidad inválida" });

                var prod = prodDict[linea.ProductoId];
                // Si tienes campo EsServicio, omitir stock; si no, asumimos que Stock aplica siempre
                if (prod.Stock < linea.Cantidad)
                    return Conflict(new { error = $"Stock insuficiente para {prod.Nombre}" });
            }

            // Calcular totales
            decimal subtotal = 0m;
            var detalles = new List<DetalleFactura>();
            foreach (var linea in dto.Lineas)
            {
                var prod = prodDict[linea.ProductoId];
                decimal precio = prod.PrecioUnitario;
                decimal totalLinea = precio * linea.Cantidad;
                subtotal += totalLinea;

                detalles.Add(new DetalleFactura
                {
                    ProductoId = prod.Id,
                    Cantidad = linea.Cantidad,
                    PrecioUnitario = precio,
                    Subtotal = totalLinea
                });
            }

            const decimal TAX_RATE = 0.13m; // cambia si necesitas otro %
            decimal impuestos = Math.Round(subtotal * TAX_RATE, 2, MidpointRounding.AwayFromZero);
            decimal totalNeto = subtotal + impuestos;

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var factura = new Factura
                {
                    ClienteId = dto.ClienteId,
                    Fecha = DateTime.UtcNow,
                    TotalBruto = subtotal,
                    Impuestos = impuestos,
                    TotalNeto = totalNeto
                };

                _context.Facturas.Add(factura);
                await _context.SaveChangesAsync(); // genera factura.Id

                foreach (var d in detalles)
                {
                    d.FacturaId = factura.Id;
                    _context.DetalleFacturas.Add(d);

                    // actualizar stock
                    var prod = prodDict[d.ProductoId];
                    prod.Stock -= d.Cantidad;
                    _context.Productos.Update(prod);
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                // Mapear a DTO de respuesta
                var respuesta = new FacturaResponseDto
                {
                    Id = factura.Id,
                    ClienteId = cliente.Id,
                    ClienteNombre = cliente.Nombre,
                    Fecha = factura.Fecha,
                    TotalBruto = factura.TotalBruto,
                    Impuestos = factura.Impuestos,
                    TotalNeto = factura.TotalNeto,
                    Detalles = detalles.Select(x => new DetalleResponseDto
                    {
                        Id = x.Id,
                        ProductoId = x.ProductoId,
                        ProductoNombre = prodDict[x.ProductoId].Nombre,
                        Cantidad = x.Cantidad,
                        PrecioUnitario = x.PrecioUnitario,
                        Subtotal = x.Subtotal
                    }).ToList()
                };

                return CreatedAtAction(nameof(GetFactura), new { id = factura.Id }, respuesta);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // DELETE: api/Facturas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFactura(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null)
            {
                return NotFound();
            }

            _context.Facturas.Remove(factura);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FacturaExists(int id)
        {
            return _context.Facturas.Any(e => e.Id == id);
        }
    }
}
