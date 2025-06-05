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
            if (pago.MontoPago < cuotasPendientes.First().CuotaTotal)
            {
                return BadRequest(new { mensaje = $"El monto mínimo de pago debe ser {cuotasPendientes.First().CuotaTotal:C}" });
            }

            // Establecer valores por defecto
            pago.FechaPago = DateOnly.FromDateTime(DateTime.Now);
            //pago.Estado = "registrado";

            // Distribuir el pago entre las cuotas pendientes
            decimal montoRestante = pago.MontoPago;
            var cuotasPagadas = new List<TablaAmortizacion>();

            foreach (var cuota in cuotasPendientes)
            {
                if (montoRestante <= 0) break;

                if (montoRestante >= cuota.CuotaTotal)
                {
                    cuota.Estado = "pagada";
                    pago.FechaPago = DateOnly.FromDateTime(DateTime.Now);
                    montoRestante -= cuota.CuotaTotal;
                    cuotasPagadas.Add(cuota);
                }
                else
                {
                    // Si el monto restante no alcanza para la cuota completa, se aplica como pago parcial
                    cuota.Estado = "parcial";
                    pago.FechaPago = DateOnly.FromDateTime(DateTime.Now);
                    cuota.SaldoPendiente -= montoRestante;
                    montoRestante = 0;
                    cuotasPagadas.Add(cuota);
                }
            }

            // Verificar si el crédito ha sido pagado completamente
            if (!credito.TablaAmortizacions.Any(t => t.Estado == "pendiente"))
            {
                credito.Estado = "pagado";
                pago.FechaPago = DateOnly.FromDateTime(DateTime.Now);
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

        [HttpGet("fecha/{fechaInicio}/{fechaFin}")]
        public async Task<ActionResult<IEnumerable<PagosCredito>>> GetPagosByFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            var fechaInicioDateOnly = DateOnly.FromDateTime(fechaInicio);
            var fechaFinDateOnly = DateOnly.FromDateTime(fechaFin);

            return await _context.PagosCreditos
                .Include(p => p.IdCreditoNavigation)
                .Include(p => p.UsuarioRegistroNavigation)
                .Where(p => p.FechaPago >= fechaInicioDateOnly && p.FechaPago <= fechaFinDateOnly)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();
        }


        private bool PagoCreditoExists(int id)
        {
            return _context.PagosCreditos.Any(e => e.IdPago == id);
        }
    }
} 