// Librerías
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocioController : ControllerBase
    {
        private readonly PublicContext _context;

        public SocioController(PublicContext context)
        {
            _context = context;
        }

        // GET: api/Socio -- Lista todos los socios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Socio>>> GetSocios()
        {
            return await _context.Socios.ToListAsync();
        }

        // GET: api/Socio/id -- Obtiene un socio por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Socio>> GetSocio(int id)
        {
            var socio = await _context.Socios.FindAsync(id);

            if (socio == null)
            {
                return NotFound(new { mensaje = "Socio no encontrado." });
            }

            return socio;
        }

        // POST: api/Socio -- Crea un nuevo socio
        [HttpPost]
        public async Task<ActionResult<Socio>> PostSocio(Socio socio)
        {
            socio.FechaCreacion = DateTime.Now;
            _context.Socios.Add(socio);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSocio), new { id = socio.IdSocio }, socio);
        }

        // PUT: api/Socio/id -- Actualiza un socio existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSocio(int id, Socio socio)
        {
            if (id != socio.IdSocio)
            {
                return BadRequest(new { mensaje = "El ID del socio no coincide." });
            }

            _context.Entry(socio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SocioExists(id))
                {
                    return NotFound(new { mensaje = "Socio no encontrado." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Socio/id -- Elimina un socio
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSocio(int id)
        {
            var socio = await _context.Socios.FindAsync(id);
            if (socio == null)
            {
                return NotFound(new { mensaje = "Socio no encontrado." });
            }

            _context.Socios.Remove(socio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Método auxiliar para verificar si el socio existe
        private bool SocioExists(int id)
        {
            return _context.Socios.Any(e => e.IdSocio == id);
        }
    }
}
