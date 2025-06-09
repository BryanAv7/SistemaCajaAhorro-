using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasAhorroController : ControllerBase
    {
        private readonly PublicContext _context;

        public CuentasAhorroController(PublicContext context)
        {
            _context = context;
        }

        // GET: api/CuentasAhorro
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CuentasAhorro>>> GetCuentasAhorro()
        {
            return await _context.CuentasAhorros
                .Include(c => c.IdSocioNavigation)
                .ToListAsync();
        }

        // GET: api/CuentasAhorro/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CuentasAhorro>> GetCuentaAhorro(int id)
        {
            var cuentaAhorro = await _context.CuentasAhorros
                .Include(c => c.IdSocioNavigation)
                .FirstOrDefaultAsync(c => c.IdCuentaAhorro == id);

            if (cuentaAhorro == null)
            {
                return NotFound(new { mensaje = "Cuenta de ahorro no encontrada." });
            }

            return cuentaAhorro;
        }

        // GET: api/CuentasAhorro/socio/5
        [HttpGet("socio/{idSocio}")]
        public async Task<ActionResult<IEnumerable<CuentasAhorro>>> GetCuentasBySocio(int idSocio)
        {
            return await _context.CuentasAhorros
                .Where(c => c.IdSocio == idSocio)
                .OrderByDescending(c => c.FechaApertura)
                .ToListAsync();
        }

        // POST: api/CuentasAhorro
        [HttpPost]
        public async Task<ActionResult<CuentasAhorro>> PostCuentaAhorro(CuentasAhorro cuentaAhorro)
        {
            // Validar que el socio existe
            var socio = await _context.Socios.FindAsync(cuentaAhorro.IdSocio);
            if (socio == null)
            {
                return BadRequest(new { mensaje = "El socio no existe." });
            }

            // Validar que el socio no tenga una cuenta del mismo tipo
            var cuentaExistente = await _context.CuentasAhorros
                .FirstOrDefaultAsync(c => c.IdSocio == cuentaAhorro.IdSocio && 
                                        c.TipoCuenta == cuentaAhorro.TipoCuenta &&
                                        c.Estado == "activa");
            if (cuentaExistente != null)
            {
                return BadRequest(new { mensaje = "El socio ya tiene una cuenta activa de este tipo." });
            }

            // Generar número de cuenta único
            cuentaAhorro.NumeroCuenta = await GenerarNumeroCuenta(cuentaAhorro.TipoCuenta);
            cuentaAhorro.Estado = "activa";
            cuentaAhorro.SaldoActual = 0;
            cuentaAhorro.FechaUltimoMovimiento = DateTime.Now;

            _context.CuentasAhorros.Add(cuentaAhorro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCuentaAhorro), new { id = cuentaAhorro.IdCuentaAhorro }, cuentaAhorro);
        }

        // PUT: api/CuentasAhorro/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCuentaAhorro(int id, CuentasAhorro cuentaAhorro)
        {
            if (id != cuentaAhorro.IdCuentaAhorro)
            {
                return BadRequest(new { mensaje = "El ID de la cuenta no coincide." });
            }

            var cuentaExistente = await _context.CuentasAhorros.FindAsync(id);
            if (cuentaExistente == null)
            {
                return NotFound(new { mensaje = "Cuenta de ahorro no encontrada." });
            }

            // No permitir modificar ciertos campos
            cuentaAhorro.NumeroCuenta = cuentaExistente.NumeroCuenta;
            cuentaAhorro.FechaApertura = cuentaExistente.FechaApertura;
            cuentaAhorro.IdSocio = cuentaExistente.IdSocio;

            _context.Entry(cuentaAhorro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CuentaAhorroExists(id))
                {
                    return NotFound(new { mensaje = "Cuenta de ahorro no encontrada." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/CuentasAhorro/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCuentaAhorro(int id)
        {
            var cuentaAhorro = await _context.CuentasAhorros.FindAsync(id);
            if (cuentaAhorro == null)
            {
                return NotFound(new { mensaje = "Cuenta de ahorro no encontrada." });
            }

            // Verificar si la cuenta tiene saldo
            if (cuentaAhorro.SaldoActual > 0)
            {
                return BadRequest(new { mensaje = "No se puede eliminar una cuenta con saldo." });
            }

            _context.CuentasAhorros.Remove(cuentaAhorro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/CuentasAhorro/estado/activa
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<CuentasAhorro>>> GetCuentasByEstado(string estado)
        {
            return await _context.CuentasAhorros
                .Include(c => c.IdSocioNavigation)
                .Where(c => c.Estado == estado)
                .OrderByDescending(c => c.FechaApertura)
                .ToListAsync();
        }

        private bool CuentaAhorroExists(int id)
        {
            return _context.CuentasAhorros.Any(e => e.IdCuentaAhorro == id);
        }

        private async Task<string> GenerarNumeroCuenta(string tipoCuenta)
        {
            // Obtener el último número de cuenta del tipo especificado
            var ultimaCuenta = await _context.CuentasAhorros
                .Where(c => c.TipoCuenta == tipoCuenta)
                .OrderByDescending(c => c.NumeroCuenta)
                .FirstOrDefaultAsync();

            string prefijo = tipoCuenta.Substring(0, 3).ToUpper();
            int numero = 1;

            if (ultimaCuenta != null)
            {
                // Extraer el número de la última cuenta y sumar 1
                string ultimoNumero = ultimaCuenta.NumeroCuenta.Substring(3);
                if (int.TryParse(ultimoNumero, out int ultimoNumeroInt))
                {
                    numero = ultimoNumeroInt + 1;
                }
            }

            // Formato: XXX-00000001
            return $"{prefijo}-{numero:D8}";
        }

        // GET: api/CuentasAhorro/resumen/{cedula}/{numeroCuenta}
        [HttpGet("resumen/{cedula}/{numeroCuenta}")]
        public async Task<IActionResult> ObtenerResumenCuenta(string cedula, string numeroCuenta)
        {
            // Buscar la cuenta por número y cédula
            var cuenta = await _context.CuentasAhorros
                .Include(c => c.IdSocioNavigation)
                .FirstOrDefaultAsync(c =>
                    c.NumeroCuenta == numeroCuenta &&
                    c.IdSocioNavigation.Cedula == cedula);

            if (cuenta == null)
            {
                return NotFound(new { mensaje = "Cuenta no encontrada." });
            }

            // Obtener los últimos 3 movimientos
            var movimientos = await _context.MovimientosAhorros
                .Where(m => m.IdCuentaAhorro == cuenta.IdCuentaAhorro)
                .OrderByDescending(m => m.FechaMovimiento)
                .Take(3)
                .Select(m => new
                {
                    m.TipoMovimiento,
                    m.Monto,
                    m.SaldoAnterior,
                    m.SaldoNuevo,
                    m.FechaMovimiento,
                    m.Descripcion
                })
                .ToListAsync();

            var resumen = new
            {
                socio = new
                {
                    cuenta.IdSocioNavigation.IdSocio,
                    cuenta.IdSocioNavigation.Nombres,
                    cuenta.IdSocioNavigation.Apellidos,
                    cuenta.IdSocioNavigation.Cedula
                },
                cuenta = new
                {
                    cuenta.NumeroCuenta,
                    cuenta.TipoCuenta,
                    cuenta.SaldoActual,
                    cuenta.Estado
                },
                ultimosMovimientos = movimientos
            };

            return Ok(resumen);
        }

    }
} 