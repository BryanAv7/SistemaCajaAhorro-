using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParametrosGeneraleController : ControllerBase
    {
        private readonly PublicContext _context;

        public ParametrosGeneraleController(PublicContext context)
        {
            _context = context;
        }

        // GET: api/ParametrosGenerale -- Lista todos los parámetros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParametrosGenerale>>> GetParametrosGenerales()
        {
            return await _context.ParametrosGenerales
                .Include(p => p.UsuarioActualizacionNavigation)
                .ToListAsync();
        }

        // GET: api/ParametrosGenerale/id -- Obtiene un parámetro por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ParametrosGenerale>> GetParametrosGenerale(int id)
        {
            var parametro = await _context.ParametrosGenerales
                .Include(p => p.UsuarioActualizacionNavigation)
                .FirstOrDefaultAsync(p => p.IdParametro == id);

            if (parametro == null)
            {
                return NotFound(new { mensaje = "Parámetro no encontrado." });
            }

            return parametro;
        }

        // POST: api/ParametrosGenerale -- Crea un nuevo parámetro
        [HttpPost]
        public async Task<ActionResult<ParametrosGenerale>> PostParametrosGenerale(ParametrosGenerale parametro)
        {
            parametro.FechaActualizacion = DateTime.Now;
            _context.ParametrosGenerales.Add(parametro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetParametrosGenerale), new { id = parametro.IdParametro }, parametro);
        }

        // PUT: api/ParametrosGenerale/id -- Actualiza un parámetro existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParametrosGenerale(int id, ParametrosGenerale parametro)
        {
            if (id != parametro.IdParametro)
            {
                return BadRequest(new { mensaje = "El ID del parámetro no coincide." });
            }

            parametro.FechaActualizacion = DateTime.Now;
            _context.Entry(parametro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParametrosGeneraleExists(id))
                {
                    return NotFound(new { mensaje = "Parámetro no encontrado." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/ParametrosGenerale/id -- Elimina un parámetro por ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParametrosGenerale(int id)
        {
            var parametro = await _context.ParametrosGenerales.FindAsync(id);
            if (parametro == null)
            {
                return NotFound(new { mensaje = "Parámetro no encontrado." });
            }

            _context.ParametrosGenerales.Remove(parametro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Método auxiliar para verificar si existe el parámetro
        private bool ParametrosGeneraleExists(int id)
        {
            return _context.ParametrosGenerales.Any(p => p.IdParametro == id);
        }
    }
}
