using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialAccioneController : ControllerBase
    {
        private readonly PublicContext _context;

        public HistorialAccioneController(PublicContext context)
        {
            _context = context;
        }

        // GET: api/HistorialAccione -- Lista todo el historial de acciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistorialAccione>>> GetHistorialAcciones()
        {
            return await _context.HistorialAcciones
                .Include(h => h.IdUsuarioNavigation)
                .ToListAsync();
        }

        // GET: api/HistorialAccione/id -- Obtiene un historial por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<HistorialAccione>> GetHistorialAccione(int id)
        {
            var historial = await _context.HistorialAcciones
                .Include(h => h.IdUsuarioNavigation)
                .FirstOrDefaultAsync(h => h.IdHistorial == id);

            if (historial == null)
            {
                return NotFound(new { mensaje = "Registro de historial no encontrado." });
            }

            return historial;
        }

        // POST: api/HistorialAccione -- Crea un nuevo registro en el historial
        [HttpPost]
        public async Task<ActionResult<HistorialAccione>> PostHistorialAccione(HistorialAccione historialAccione)
        {
            historialAccione.FechaAccion = DateTime.Now;
            _context.HistorialAcciones.Add(historialAccione);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHistorialAccione), new { id = historialAccione.IdHistorial }, historialAccione);
        }

        // PUT: api/HistorialAccione/id -- Actualiza un historial existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHistorialAccione(int id, HistorialAccione historialAccione)
        {
            if (id != historialAccione.IdHistorial)
            {
                return BadRequest(new { mensaje = "ID de historial no coincide." });
            }

            _context.Entry(historialAccione).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HistorialAccioneExists(id))
                {
                    return NotFound(new { mensaje = "Registro de historial no encontrado." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/HistorialAccione/id -- Elimina un historial por ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistorialAccione(int id)
        {
            var historial = await _context.HistorialAcciones.FindAsync(id);
            if (historial == null)
            {
                return NotFound(new { mensaje = "Registro de historial no encontrado." });
            }

            _context.HistorialAcciones.Remove(historial);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Método auxiliar para verificar si un historial existe
        private bool HistorialAccioneExists(int id)
        {
            return _context.HistorialAcciones.Any(e => e.IdHistorial == id);
        }
    }
}
