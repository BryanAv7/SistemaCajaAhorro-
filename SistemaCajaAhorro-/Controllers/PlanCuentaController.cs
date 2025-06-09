using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanCuentaController : ControllerBase
    {
        private readonly PublicContext _context;

        public PlanCuentaController(PublicContext context)
        {
            _context = context;
        }

        // GET: api/PlanCuenta -- Lista todos los planes de cuenta
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanCuenta>>> GetPlanCuentas()
        {
            return await _context.PlanCuentas
                .Include(p => p.CuentaPadreNavigation)
                .Include(p => p.InverseCuentaPadreNavigation)
                .ToListAsync();
        }

        // GET: api/PlanCuenta/id -- Obtiene un plan de cuenta por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<PlanCuenta>> GetPlanCuenta(int id)
        {
            var planCuenta = await _context.PlanCuentas
                .Include(p => p.CuentaPadreNavigation)
                .Include(p => p.InverseCuentaPadreNavigation)
                .FirstOrDefaultAsync(p => p.IdCuenta == id);

            if (planCuenta == null)
            {
                return NotFound(new { mensaje = "Plan de cuenta no encontrado." });
            }

            return planCuenta;
        }

        // POST: api/PlanCuenta -- Crea un nuevo plan de cuenta
        [HttpPost]
        public async Task<ActionResult<PlanCuenta>> PostPlanCuenta(PlanCuenta planCuenta)
        {
            planCuenta.FechaCreacion = DateTime.Now;
            _context.PlanCuentas.Add(planCuenta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlanCuenta), new { id = planCuenta.IdCuenta }, planCuenta);
        }

        // PUT: api/PlanCuenta/id -- Actualiza un plan de cuenta existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlanCuenta(int id, PlanCuenta planCuenta)
        {
            if (id != planCuenta.IdCuenta)
            {
                return BadRequest(new { mensaje = "El ID del plan de cuenta no coincide." });
            }

            _context.Entry(planCuenta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanCuentaExists(id))
                {
                    return NotFound(new { mensaje = "Plan de cuenta no encontrado." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/PlanCuenta/id -- Elimina un plan de cuenta
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlanCuenta(int id)
        {
            var planCuenta = await _context.PlanCuentas.FindAsync(id);
            if (planCuenta == null)
            {
                return NotFound(new { mensaje = "Plan de cuenta no encontrado." });
            }

            _context.PlanCuentas.Remove(planCuenta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Método auxiliar para verificar si el plan de cuenta existe
        private bool PlanCuentaExists(int id)
        {
            return _context.PlanCuentas.Any(e => e.IdCuenta == id);
        }
    }
}
