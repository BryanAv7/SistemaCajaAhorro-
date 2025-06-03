using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanCuentasController : ControllerBase
    {
        private readonly PublicContext _context;

        public PlanCuentasController(PublicContext context)
        {
            _context = context;
        }

        // GET: api/PlanCuentas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanCuenta>>> GetPlanCuentas()
        {
            return await _context.PlanCuentas
                .OrderBy(c => c.Codigo)
                .ToListAsync();
        }

        // GET: api/PlanCuentas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlanCuenta>> GetCuenta(int id)
        {
            var cuenta = await _context.PlanCuentas.FindAsync(id);

            if (cuenta == null)
            {
                return NotFound(new { mensaje = "Cuenta no encontrada." });
            }

            return cuenta;
        }

        // GET: api/PlanCuentas/codigo/1001
        [HttpGet("codigo/{codigo}")]
        public async Task<ActionResult<PlanCuenta>> GetCuentaByCodigo(string codigo)
        {
            var cuenta = await _context.PlanCuentas
                .FirstOrDefaultAsync(c => c.Codigo == codigo);

            if (cuenta == null)
            {
                return NotFound(new { mensaje = "Cuenta no encontrada." });
            }

            return cuenta;
        }

        // GET: api/PlanCuentas/tipo/activo
        [HttpGet("tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<PlanCuenta>>> GetCuentasByTipo(string tipo)
        {
            return await _context.PlanCuentas
                .Where(c => c.Tipo == tipo)
                .OrderBy(c => c.Codigo)
                .ToListAsync();
        }

        // GET: api/PlanCuentas/nivel/1
        [HttpGet("nivel/{nivel}")]
        public async Task<ActionResult<IEnumerable<PlanCuenta>>> GetCuentasByNivel(int nivel)
        {
            return await _context.PlanCuentas
                .Where(c => c.Nivel == nivel)
                .OrderBy(c => c.Codigo)
                .ToListAsync();
        }

        // POST: api/PlanCuentas
        [HttpPost]
        public async Task<ActionResult<PlanCuenta>> PostCuenta(PlanCuenta cuenta)
        {
            // Validar que el código no existe
            var cuentaExistente = await _context.PlanCuentas
                .FirstOrDefaultAsync(c => c.Codigo == cuenta.Codigo);
            if (cuentaExistente != null)
            {
                return BadRequest(new { mensaje = "Ya existe una cuenta con este código." });
            }

            // Validar que la cuenta padre existe si se especifica
            if (cuenta.IdCuentaPadre.HasValue)
            {
                var cuentaPadre = await _context.PlanCuentas.FindAsync(cuenta.IdCuentaPadre);
                if (cuentaPadre == null)
                {
                    return BadRequest(new { mensaje = "La cuenta padre no existe." });
                }

                // Validar que el nivel es correcto
                if (cuenta.Nivel != cuentaPadre.Nivel + 1)
                {
                    return BadRequest(new { mensaje = "El nivel de la cuenta debe ser uno más que el nivel de la cuenta padre." });
                }
            }
            else if (cuenta.Nivel != 1)
            {
                return BadRequest(new { mensaje = "Las cuentas sin padre deben ser de nivel 1." });
            }

            // Validar que el tipo es válido
            string[] tiposValidos = { "activo", "pasivo", "capital", "ingreso", "gasto" };
            if (!tiposValidos.Contains(cuenta.Tipo.ToLower()))
            {
                return BadRequest(new { mensaje = "El tipo de cuenta no es válido." });
            }

            _context.PlanCuentas.Add(cuenta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCuenta), new { id = cuenta.IdCuenta }, cuenta);
        }

        // PUT: api/PlanCuentas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCuenta(int id, PlanCuenta cuenta)
        {
            if (id != cuenta.IdCuenta)
            {
                return BadRequest(new { mensaje = "El ID de la cuenta no coincide." });
            }

            var cuentaExistente = await _context.PlanCuentas.FindAsync(id);
            if (cuentaExistente == null)
            {
                return NotFound(new { mensaje = "Cuenta no encontrada." });
            }

            // No permitir modificar el código
            cuenta.Codigo = cuentaExistente.Codigo;

            // Validar que la cuenta padre existe si se especifica
            if (cuenta.IdCuentaPadre.HasValue)
            {
                var cuentaPadre = await _context.PlanCuentas.FindAsync(cuenta.IdCuentaPadre);
                if (cuentaPadre == null)
                {
                    return BadRequest(new { mensaje = "La cuenta padre no existe." });
                }

                // Validar que el nivel es correcto
                if (cuenta.Nivel != cuentaPadre.Nivel + 1)
                {
                    return BadRequest(new { mensaje = "El nivel de la cuenta debe ser uno más que el nivel de la cuenta padre." });
                }
            }
            else if (cuenta.Nivel != 1)
            {
                return BadRequest(new { mensaje = "Las cuentas sin padre deben ser de nivel 1." });
            }

            // Validar que el tipo es válido
            string[] tiposValidos = { "activo", "pasivo", "capital", "ingreso", "gasto" };
            if (!tiposValidos.Contains(cuenta.Tipo.ToLower()))
            {
                return BadRequest(new { mensaje = "El tipo de cuenta no es válido." });
            }

            _context.Entry(cuenta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CuentaExists(id))
                {
                    return NotFound(new { mensaje = "Cuenta no encontrada." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/PlanCuentas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCuenta(int id)
        {
            var cuenta = await _context.PlanCuentas.FindAsync(id);
            if (cuenta == null)
            {
                return NotFound(new { mensaje = "Cuenta no encontrada." });
            }

            // Validar que no tiene cuentas hijas
            var tieneHijas = await _context.PlanCuentas
                .AnyAsync(c => c.IdCuentaPadre == id);
            if (tieneHijas)
            {
                return BadRequest(new { mensaje = "No se puede eliminar una cuenta que tiene cuentas hijas." });
            }

            // Validar que no tiene movimientos
            var tieneMovimientos = await _context.DetalleLibroDiarios
                .AnyAsync(d => d.IdCuenta == id);
            if (tieneMovimientos)
            {
                return BadRequest(new { mensaje = "No se puede eliminar una cuenta que tiene movimientos." });
            }

            _context.PlanCuentas.Remove(cuenta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CuentaExists(int id)
        {
            return _context.PlanCuentas.Any(e => e.IdCuenta == id);
        }
    }
} 