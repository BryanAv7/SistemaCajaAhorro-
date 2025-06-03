using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosCreditoController : ControllerBase
    {
        private readonly PublicContext _context;

        public PagosCreditoController(PublicContext context)
        {
            _context = context;
        }

        // GET: api/PagosCredito
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PagosCredito>>> GetPagosCredito()
        {
            return await _context.PagosCreditos
                .Include(p => p.IdCreditoNavigation)
                .Include(p => p.UsuarioRegistroNavigation)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();
        }

        // GET: api/PagosCredito/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PagosCredito>> GetPagoCredito(int id)
        {
            var pago = await _context.PagosCreditos
                .Include(p => p.IdCreditoNavigation)
                .Include(p => p.UsuarioRegistroNavigation)
                .FirstOrDefaultAsync(p => p.IdPago == id);

            if (pago == null)
            {
                return NotFound(new { mensaje = "Pago no encontrado." });
            }

            return pago;
        }

        // GET: api/PagosCredito/credito/5
        [HttpGet("credito/{idCredito}")]
        public async Task<ActionResult<IEnumerable<PagosCredito>>> GetPagosByCredito(int idCredito)
        {
            return await _context.PagosCreditos
                .Include(p => p.UsuarioRegistroNavigation)
                .Where(p => p.IdCredito == idCredito)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();
        }

        // POST: api/PagosCredito
        [HttpPost]
        public async Task<ActionResult<PagosCredito>> PostPagoCredito(PagosCredito pago)
        {
            // Validar que el crédito existe y está activo
            var credito = await _context.Creditos
                .Include(c => c.TablaAmortizacions)
                .FirstOrDefaultAsync(c => c.IdCredito == pago.IdCredito);
            
            if (credito == null)
            {
                return BadRequest(new { mensaje = "El crédito no existe." });
            }
            if (credito.Estado != "activo")
            {
                return BadRequest(new { mensaje = "El crédito no está activo." });
            }

            // Validar que el usuario existe
            var usuario = await _context.Usuarios.FindAsync(pago.UsuarioRegistro);
            if (usuario == null)
            {
                return BadRequest(new { mensaje = "El usuario no existe." });
            }

            // Obtener cuotas pendientes ordenadas por fecha de vencimiento
            var cuotasPendientes = credito.TablaAmortizacions
                .Where(t => t.Estado == "pendiente")
                .OrderBy(t => t.FechaVencimiento)
                .ToList();

            if (!cuotasPendientes.Any())
            {
                return BadRequest(new { mensaje = "No hay cuotas pendientes para este crédito." });
            }

            // Validar que el monto del pago sea suficiente para al menos una cuota
            if (pago.Monto < cuotasPendientes.First().CuotaMensual)
            {
                return BadRequest(new { mensaje = $"El monto mínimo de pago debe ser {cuotasPendientes.First().CuotaMensual:C}" });
            }

            // Establecer valores por defecto
            pago.FechaPago = DateTime.Now;
            pago.Estado = "registrado";

            // Distribuir el pago entre las cuotas pendientes
            decimal montoRestante = pago.Monto;
            var cuotasPagadas = new List<TablaAmortizacion>();

            foreach (var cuota in cuotasPendientes)
            {
                if (montoRestante <= 0) break;

                if (montoRestante >= cuota.CuotaMensual)
                {
                    cuota.Estado = "pagada";
                    cuota.FechaPago = DateOnly.FromDateTime(DateTime.Now);
                    montoRestante -= cuota.CuotaMensual;
                    cuotasPagadas.Add(cuota);
                }
                else
                {
                    // Si el monto restante no alcanza para la cuota completa, se aplica como pago parcial
                    cuota.Estado = "parcial";
                    cuota.FechaPago = DateOnly.FromDateTime(DateTime.Now);
                    cuota.SaldoFinal -= montoRestante;
                    montoRestante = 0;
                    cuotasPagadas.Add(cuota);
                }
            }

            // Verificar si el crédito ha sido pagado completamente
            if (!credito.TablaAmortizacions.Any(t => t.Estado == "pendiente"))
            {
                credito.Estado = "pagado";
                credito.FechaPago = DateOnly.FromDateTime(DateTime.Now);
            }

            _context.PagosCreditos.Add(pago);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPagoCredito), new { id = pago.IdPago }, pago);
        }

        // GET: api/PagosCredito/estado/registrado
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<PagosCredito>>> GetPagosByEstado(string estado)
        {
            return await _context.PagosCreditos
                .Include(p => p.IdCreditoNavigation)
                .Include(p => p.UsuarioRegistroNavigation)
                .Where(p => p.Estado == estado)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();
        }

        // GET: api/PagosCredito/fecha/2024-01-01/2024-12-31
        [HttpGet("fecha/{fechaInicio}/{fechaFin}")]
        public async Task<ActionResult<IEnumerable<PagosCredito>>> GetPagosByFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.PagosCreditos
                .Include(p => p.IdCreditoNavigation)
                .Include(p => p.UsuarioRegistroNavigation)
                .Where(p => p.FechaPago >= fechaInicio && p.FechaPago <= fechaFin)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();
        }

        private bool PagoCreditoExists(int id)
        {
            return _context.PagosCreditos.Any(e => e.IdPago == id);
        }
    }
} 