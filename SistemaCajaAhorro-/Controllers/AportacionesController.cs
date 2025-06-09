using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AportacionesController : ControllerBase
    {
        private readonly PublicContext _context;

        public AportacionesController(PublicContext context)
        {
            _context = context;
        }

        // GET: api/Aportaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Aportacione>>> GetAportaciones()
        {
            return await _context.Aportaciones
                .Include(a => a.IdSocioNavigation)
                .Include(a => a.IdTipoAportacionNavigation)
                .Include(a => a.UsuarioRegistroNavigation)
                .ToListAsync();
        }

        // GET: api/Aportaciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Aportacione>> GetAportacion(int id)
        {
            var aportacion = await _context.Aportaciones
                .Include(a => a.IdSocioNavigation)
                .Include(a => a.IdTipoAportacionNavigation)
                .Include(a => a.UsuarioRegistroNavigation)
                .FirstOrDefaultAsync(a => a.IdAportacion == id);

            if (aportacion == null)
            {
                return NotFound(new { mensaje = "Aportación no encontrada." });
            }

            return aportacion;
        }

        // GET: api/Aportaciones/socio/5
        [HttpGet("socio/{idSocio}")]
        public async Task<ActionResult<IEnumerable<Aportacione>>> GetAportacionesBySocio(int idSocio)
        {
            return await _context.Aportaciones
                .Include(a => a.IdTipoAportacionNavigation)
                .Where(a => a.IdSocio == idSocio)
                .OrderByDescending(a => a.FechaAportacion)
                .ToListAsync();
        }

        // POST: api/Aportaciones
        [HttpPost]
        public async Task<ActionResult<Aportacione>> PostAportacion(Aportacione aportacion)
        {
            // Validar que el socio existe
            var socio = await _context.Socios.FindAsync(aportacion.IdSocio);
            if (socio == null)
            {
                return BadRequest(new { mensaje = "El socio no existe." });
            }

            // Validar que el tipo de aportación existe
            var tipoAportacion = await _context.TiposAportacions.FindAsync(aportacion.IdTipoAportacion);
            if (tipoAportacion == null)
            {
                return BadRequest(new { mensaje = "El tipo de aportación no existe." });
            }

            // Validar que el usuario existe
            var usuario = await _context.Usuarios.FindAsync(aportacion.UsuarioRegistro);
            if (usuario == null)
            {
                return BadRequest(new { mensaje = "El usuario no existe." });
            }

            // Validar monto mínimo si es requerido
            if (tipoAportacion.MontoMinimo.HasValue && aportacion.Monto < tipoAportacion.MontoMinimo.Value)
            {
                return BadRequest(new { mensaje = $"El monto mínimo para este tipo de aportación es {tipoAportacion.MontoMinimo.Value:C}" });
            }

            aportacion.FechaRegistro = DateTime.Now;
            aportacion.Estado = "registrada";

            _context.Aportaciones.Add(aportacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAportacion), new { id = aportacion.IdAportacion }, aportacion);
        }

        // PUT: api/Aportaciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAportacion(int id, Aportacione aportacion)
        {
            if (id != aportacion.IdAportacion)
            {
                return BadRequest(new { mensaje = "El ID de la aportación no coincide." });
            }

            // Validar que la aportación existe
            var aportacionExistente = await _context.Aportaciones.FindAsync(id);
            if (aportacionExistente == null)
            {
                return NotFound(new { mensaje = "Aportación no encontrada." });
            }

            // No permitir modificar ciertos campos
            aportacion.FechaRegistro = aportacionExistente.FechaRegistro;
            aportacion.UsuarioRegistro = aportacionExistente.UsuarioRegistro;

            _context.Entry(aportacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AportacionExists(id))
                {
                    return NotFound(new { mensaje = "Aportación no encontrada." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Aportaciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAportacion(int id)
        {
            var aportacion = await _context.Aportaciones.FindAsync(id);
            if (aportacion == null)
            {
                return NotFound(new { mensaje = "Aportación no encontrada." });
            }

            // Solo permitir eliminar aportaciones en estado "registrada"
            if (aportacion.Estado != "registrada")
            {
                return BadRequest(new { mensaje = "Solo se pueden eliminar aportaciones en estado 'registrada'." });
            }

            _context.Aportaciones.Remove(aportacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Aportaciones/estado/registrada
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<Aportacione>>> GetAportacionesByEstado(string estado)
        {
            return await _context.Aportaciones
                .Include(a => a.IdSocioNavigation)
                .Include(a => a.IdTipoAportacionNavigation)
                .Where(a => a.Estado == estado)
                .OrderByDescending(a => a.FechaAportacion)
                .ToListAsync();
        }

        private bool AportacionExists(int id)
        {
            return _context.Aportaciones.Any(e => e.IdAportacion == id);
        }
    }
} 