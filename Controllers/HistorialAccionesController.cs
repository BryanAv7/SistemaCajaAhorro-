using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialAccionesController : ControllerBase
    {
        private readonly PublicContext _context;

        public HistorialAccionesController(PublicContext context)
        {
            _context = context;
        }

        // GET: api/HistorialAcciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistorialAccione>>> GetHistorialAcciones()
        {
            return await _context.HistorialAcciones
                .Include(h => h.UsuarioNavigation)
                .OrderByDescending(h => h.FechaAccion)
                .ToListAsync();
        }

        // GET: api/HistorialAcciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HistorialAccione>> GetAccion(int id)
        {
            var accion = await _context.HistorialAcciones
                .Include(h => h.UsuarioNavigation)
                .FirstOrDefaultAsync(h => h.IdAccion == id);

            if (accion == null)
            {
                return NotFound(new { mensaje = "Acción no encontrada." });
            }

            return accion;
        }

        // GET: api/HistorialAcciones/usuario/5
        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<HistorialAccione>>> GetAccionesByUsuario(int idUsuario)
        {
            return await _context.HistorialAcciones
                .Include(h => h.UsuarioNavigation)
                .Where(h => h.Usuario == idUsuario)
                .OrderByDescending(h => h.FechaAccion)
                .ToListAsync();
        }

        // GET: api/HistorialAcciones/tipo/login
        [HttpGet("tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<HistorialAccione>>> GetAccionesByTipo(string tipo)
        {
            return await _context.HistorialAcciones
                .Include(h => h.UsuarioNavigation)
                .Where(h => h.TipoAccion == tipo)
                .OrderByDescending(h => h.FechaAccion)
                .ToListAsync();
        }

        // GET: api/HistorialAcciones/fecha/2024-01-01/2024-12-31
        [HttpGet("fecha/{fechaInicio}/{fechaFin}")]
        public async Task<ActionResult<IEnumerable<HistorialAccione>>> GetAccionesByFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.HistorialAcciones
                .Include(h => h.UsuarioNavigation)
                .Where(h => h.FechaAccion >= fechaInicio && h.FechaAccion <= fechaFin)
                .OrderByDescending(h => h.FechaAccion)
                .ToListAsync();
        }

        // POST: api/HistorialAcciones
        [HttpPost]
        public async Task<ActionResult<HistorialAccione>> PostAccion(HistorialAccione accion)
        {
            // Validar que el usuario existe
            var usuario = await _context.Usuarios.FindAsync(accion.Usuario);
            if (usuario == null)
            {
                return BadRequest(new { mensaje = "El usuario no existe." });
            }

            // Validar que el tipo de acción es válido
            string[] tiposValidos = { "login", "logout", "crear", "modificar", "eliminar", "consultar" };
            if (!tiposValidos.Contains(accion.TipoAccion.ToLower()))
            {
                return BadRequest(new { mensaje = "El tipo de acción no es válido." });
            }

            // Establecer la fecha de la acción
            accion.FechaAccion = DateTime.Now;

            _context.HistorialAcciones.Add(accion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAccion), new { id = accion.IdAccion }, accion);
        }

        // DELETE: api/HistorialAcciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccion(int id)
        {
            var accion = await _context.HistorialAcciones.FindAsync(id);
            if (accion == null)
            {
                return NotFound(new { mensaje = "Acción no encontrada." });
            }

            _context.HistorialAcciones.Remove(accion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AccionExists(int id)
        {
            return _context.HistorialAcciones.Any(e => e.IdAccion == id);
        }
    }
} 