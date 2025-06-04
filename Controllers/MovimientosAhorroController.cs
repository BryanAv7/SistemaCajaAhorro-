using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientosAhorroController : ControllerBase
    {
        private readonly PublicContext _context;

        public MovimientosAhorroController(PublicContext context)
        {
            _context = context;
        }

        // GET: api/MovimientosAhorro
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimientosAhorro>>> GetMovimientosAhorro()
        {
            return await _context.MovimientosAhorros
                .Include(m => m.CuentaAhorroNavigation)
                .Include(m => m.UsuarioRegistroNavigation)
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();
        }

        // GET: api/MovimientosAhorro/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovimientosAhorro>> GetMovimiento(int id)
        {
            var movimiento = await _context.MovimientosAhorros
                .Include(m => m.CuentaAhorroNavigation)
                .Include(m => m.UsuarioRegistroNavigation)
                .FirstOrDefaultAsync(m => m.IdMovimiento == id);

            if (movimiento == null)
            {
                return NotFound(new { mensaje = "Movimiento no encontrado." });
            }

            return movimiento;
        }

        // GET: api/MovimientosAhorro/cuenta/5
        [HttpGet("cuenta/{idCuenta}")]
        public async Task<ActionResult<IEnumerable<MovimientosAhorro>>> GetMovimientosByCuenta(int idCuenta)
        {
            return await _context.MovimientosAhorros
                .Include(m => m.CuentaAhorroNavigation)
                .Include(m => m.UsuarioRegistroNavigation)
                .Where(m => m.CuentaAhorro == idCuenta)
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();
        }

        // POST: api/MovimientosAhorro
        [HttpPost]
        public async Task<ActionResult<MovimientosAhorro>> PostMovimiento(MovimientosAhorro movimiento)
        {
            // Validar que la cuenta existe
            var cuenta = await _context.CuentasAhorros.FindAsync(movimiento.CuentaAhorro);
            if (cuenta == null)
            {
                return BadRequest(new { mensaje = "La cuenta de ahorro no existe." });
            }

            // Validar que el usuario existe
            var usuario = await _context.Usuarios.FindAsync(movimiento.UsuarioRegistro);
            if (usuario == null)
            {
                return BadRequest(new { mensaje = "El usuario no existe." });
            }

            // Validar el tipo de movimiento
            if (movimiento.TipoMovimiento != "deposito" && movimiento.TipoMovimiento != "retiro")
            {
                return BadRequest(new { mensaje = "Tipo de movimiento inválido. Debe ser 'deposito' o 'retiro'." });
            }

            // Validar el monto
            if (movimiento.Monto <= 0)
            {
                return BadRequest(new { mensaje = "El monto debe ser mayor a cero." });
            }

            // Para retiros, validar que hay saldo suficiente
            if (movimiento.TipoMovimiento == "retiro" && cuenta.SaldoActual < movimiento.Monto)
            {
                return BadRequest(new { mensaje = "Saldo insuficiente para realizar el retiro." });
            }

            // Establecer la fecha del movimiento
            movimiento.FechaMovimiento = DateTime.Now;

            // Calcular el nuevo saldo
            movimiento.SaldoNuevo = movimiento.TipoMovimiento == "deposito" 
                ? cuenta.SaldoActual + movimiento.Monto 
                : cuenta.SaldoActual - movimiento.Monto;

            // Establecer el saldo anterior
            movimiento.SaldoAnterior = cuenta.SaldoActual;

            // Agregar el movimiento
            _context.MovimientosAhorros.Add(movimiento);

            try
            {
                await _context.SaveChangesAsync();

                // Registrar la acción en el historial
                var historialAccion = new HistorialAccione
                {
                    Usuario = movimiento.UsuarioRegistro,
                    TipoAccion = movimiento.TipoMovimiento,
                    Descripcion = $"Movimiento de {movimiento.TipoMovimiento} por {movimiento.Monto} en cuenta {movimiento.CuentaAhorro}",
                    FechaAccion = DateTime.Now
                };
                _context.HistorialAcciones.Add(historialAccion);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetMovimiento), new { id = movimiento.IdMovimiento }, movimiento);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovimientoExists(movimiento.IdMovimiento))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE: api/MovimientosAhorro/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovimiento(int id)
        {
            var movimiento = await _context.MovimientosAhorros.FindAsync(id);
            if (movimiento == null)
            {
                return NotFound(new { mensaje = "Movimiento no encontrado." });
            }

            // No permitir eliminar movimientos antiguos
            if ((DateTime.Now - movimiento.FechaMovimiento).TotalDays > 30)
            {
                return BadRequest(new { mensaje = "No se pueden eliminar movimientos con más de 30 días de antigüedad." });
            }

            _context.MovimientosAhorros.Remove(movimiento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/MovimientosAhorro/tipo/deposito
        [HttpGet("tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<MovimientosAhorro>>> GetMovimientosByTipo(string tipo)
        {
            return await _context.MovimientosAhorros
                .Include(m => m.CuentaAhorroNavigation)
                .Include(m => m.UsuarioRegistroNavigation)
                .Where(m => m.TipoMovimiento == tipo)
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();
        }

        // GET: api/MovimientosAhorro/fecha/2024-01-01/2024-12-31
        [HttpGet("fecha/{fechaInicio}/{fechaFin}")]
        public async Task<ActionResult<IEnumerable<MovimientosAhorro>>> GetMovimientosByFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.MovimientosAhorros
                .Include(m => m.CuentaAhorroNavigation)
                .Include(m => m.UsuarioRegistroNavigation)
                .Where(m => m.FechaMovimiento >= fechaInicio && m.FechaMovimiento <= fechaFin)
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();
        }

        private bool MovimientoExists(int id)
        {
            return _context.MovimientosAhorros.Any(e => e.IdMovimiento == id);
        }
    }
} 