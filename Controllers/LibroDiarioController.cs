using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibroDiarioController : ControllerBase
    {
        private readonly PublicContext _context;

        public LibroDiarioController(PublicContext context)
        {
            _context = context;
        }

        // GET: api/LibroDiario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LibroDiario>>> GetLibroDiario()
        {
            return await _context.LibroDiarios
                .Include(l => l.UsuarioRegistroNavigation)
                .Include(l => l.DetalleLibroDiarios)
                    .ThenInclude(d => d.IdCuentaNavigation)
                .OrderByDescending(l => l.Fecha)
                .ToListAsync();
        }

        // GET: api/LibroDiario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LibroDiario>> GetAsiento(int id)
        {
            var asiento = await _context.LibroDiarios
                .Include(l => l.UsuarioRegistroNavigation)
                .Include(l => l.DetalleLibroDiarios)
                    .ThenInclude(d => d.IdCuentaNavigation)
                .FirstOrDefaultAsync(l => l.IdAsiento == id);

            if (asiento == null)
            {
                return NotFound(new { mensaje = "Asiento no encontrado." });
            }

            return asiento;
        }

        // POST: api/LibroDiario
        [HttpPost]
        public async Task<ActionResult<LibroDiario>> PostAsiento(LibroDiario asiento)
        {
            // Validar que el usuario existe
            var usuario = await _context.Usuarios.FindAsync(asiento.UsuarioRegistro);
            if (usuario == null)
            {
                return BadRequest(new { mensaje = "El usuario no existe." });
            }

            // Validar que hay al menos dos detalles (débito y crédito)
            if (asiento.DetalleLibroDiarios == null || asiento.DetalleLibroDiarios.Count < 2)
            {
                return BadRequest(new { mensaje = "El asiento debe tener al menos dos detalles (débito y crédito)." });
            }

            // Validar que los montos de débito y crédito son iguales
            decimal totalDebito = asiento.DetalleLibroDiarios.Where(d => d.Tipo == "debito").Sum(d => d.Monto);
            decimal totalCredito = asiento.DetalleLibroDiarios.Where(d => d.Tipo == "credito").Sum(d => d.Monto);

            if (totalDebito != totalCredito)
            {
                return BadRequest(new { mensaje = "Los montos de débito y crédito deben ser iguales." });
            }

            // Validar que todas las cuentas existen
            foreach (var detalle in asiento.DetalleLibroDiarios)
            {
                var cuenta = await _context.PlanCuentas.FindAsync(detalle.IdCuenta);
                if (cuenta == null)
                {
                    return BadRequest(new { mensaje = $"La cuenta {detalle.IdCuenta} no existe." });
                }
            }

            // Establecer valores por defecto
            asiento.Fecha = DateTime.Now;
            asiento.Estado = "registrado";
            asiento.Total = totalDebito;

            // Generar número de asiento
            asiento.NumeroAsiento = await GenerarNumeroAsiento();

            _context.LibroDiarios.Add(asiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAsiento), new { id = asiento.IdAsiento }, asiento);
        }

        // GET: api/LibroDiario/fecha/2024-01-01/2024-12-31
        [HttpGet("fecha/{fechaInicio}/{fechaFin}")]
        public async Task<ActionResult<IEnumerable<LibroDiario>>> GetAsientosByFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.LibroDiarios
                .Include(l => l.UsuarioRegistroNavigation)
                .Include(l => l.DetalleLibroDiarios)
                    .ThenInclude(d => d.IdCuentaNavigation)
                .Where(l => l.Fecha >= fechaInicio && l.Fecha <= fechaFin)
                .OrderByDescending(l => l.Fecha)
                .ToListAsync();
        }

        // GET: api/LibroDiario/cuenta/5
        [HttpGet("cuenta/{idCuenta}")]
        public async Task<ActionResult<IEnumerable<DetalleLibroDiario>>> GetMovimientosByCuenta(int idCuenta)
        {
            return await _context.DetalleLibroDiarios
                .Include(d => d.IdAsientoNavigation)
                .Include(d => d.IdCuentaNavigation)
                .Where(d => d.IdCuenta == idCuenta)
                .OrderByDescending(d => d.IdAsientoNavigation.Fecha)
                .ToListAsync();
        }

        // GET: api/LibroDiario/estado/registrado
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<LibroDiario>>> GetAsientosByEstado(string estado)
        {
            return await _context.LibroDiarios
                .Include(l => l.UsuarioRegistroNavigation)
                .Include(l => l.DetalleLibroDiarios)
                    .ThenInclude(d => d.IdCuentaNavigation)
                .Where(l => l.Estado == estado)
                .OrderByDescending(l => l.Fecha)
                .ToListAsync();
        }

        private async Task<string> GenerarNumeroAsiento()
        {
            // Obtener el último número de asiento
            var ultimoAsiento = await _context.LibroDiarios
                .OrderByDescending(l => l.NumeroAsiento)
                .FirstOrDefaultAsync();

            int numero = 1;
            if (ultimoAsiento != null)
            {
                // Extraer el número del último asiento y sumar 1
                string ultimoNumero = ultimoAsiento.NumeroAsiento.Substring(3);
                if (int.TryParse(ultimoNumero, out int ultimoNumeroInt))
                {
                    numero = ultimoNumeroInt + 1;
                }
            }

            // Formato: ASI-00000001
            return $"ASI-{numero:D8}";
        }
    }
} 