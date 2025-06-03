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
                .Include(m => m.IdCuentaAhorroNavigation)
                .Include(m => m.UsuarioRegistroNavigation)
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();
        }

        // GET: api/MovimientosAhorro/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovimientosAhorro>> GetMovimientoAhorro(int id)
        {
            var movimiento = await _context.MovimientosAhorros
                .Include(m => m.IdCuentaAhorroNavigation)
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
                .Include(m => m.UsuarioRegistroNavigation)
                .Where(m => m.IdCuentaAhorro == idCuenta)
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();
        }

        // POST: api/MovimientosAhorro
        [HttpPost]
        public async Task<ActionResult<MovimientosAhorro>> PostMovimientoAhorro(MovimientosAhorro movimiento)
        {
            // Validar que la cuenta existe y está activa
            var cuenta = await _context.CuentasAhorros.FindAsync(movimiento.IdCuentaAhorro);
            if (cuenta == null)
            {
                return BadRequest(new { mensaje = "La cuenta de ahorro no existe." });
            }
            if (cuenta.Estado != "activa")
            {
                return BadRequest(new { mensaje = "La cuenta de ahorro no está activa." });
            }

            // Validar que el usuario existe
            var usuario = await _context.Usuarios.FindAsync(movimiento.UsuarioRegistro);
            if (usuario == null)
            {
                return BadRequest(new { mensaje = "El usuario no existe." });
            }

            // Validar el monto según el tipo de movimiento
            if (movimiento.TipoMovimiento == "retiro" && movimiento.Monto > cuenta.SaldoActual)
            {
                return BadRequest(new { mensaje = "Saldo insuficiente para realizar el retiro." });
            }

            // Establecer saldos
            movimiento.SaldoAnterior = cuenta.SaldoActual ?? 0;
            movimiento.SaldoNuevo = movimiento.TipoMovimiento == "deposito" 
                ? movimiento.SaldoAnterior + movimiento.Monto 
                : movimiento.SaldoAnterior - movimiento.Monto;
            movimiento.FechaMovimiento = DateTime.Now;

            // Actualizar saldo de la cuenta
            cuenta.SaldoActual = movimiento.SaldoNuevo;
            cuenta.FechaUltimoMovimiento = DateTime.Now;

            _context.MovimientosAhorros.Add(movimiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovimientoAhorro), new { id = movimiento.IdMovimiento }, movimiento);
        }

        // GET: api/MovimientosAhorro/tipo/deposito
        [HttpGet("tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<MovimientosAhorro>>> GetMovimientosByTipo(string tipo)
        {
            return await _context.MovimientosAhorros
                .Include(m => m.IdCuentaAhorroNavigation)
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
                .Include(m => m.IdCuentaAhorroNavigation)
                .Include(m => m.UsuarioRegistroNavigation)
                .Where(m => m.FechaMovimiento >= fechaInicio && m.FechaMovimiento <= fechaFin)
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();
        }

        private bool MovimientoAhorroExists(int id)
        {
            return _context.MovimientosAhorros.Any(e => e.IdMovimiento == id);
        }
    }
} 