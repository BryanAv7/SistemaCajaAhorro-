using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SesionesUsuarioController : ControllerBase
    {
        private readonly PublicContext _context;

        public SesionesUsuarioController(PublicContext context)
        {
            _context = context;
        }

        // GET: api/SesionesUsuario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SesionesUsuario>>> GetSesionesUsuario()
        {
            return await _context.SesionesUsuarios
                .Include(s => s.UsuarioNavigation)
                .OrderByDescending(s => s.FechaInicio)
                .ToListAsync();
        }

        // GET: api/SesionesUsuario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SesionesUsuario>> GetSesion(int id)
        {
            var sesion = await _context.SesionesUsuarios
                .Include(s => s.UsuarioNavigation)
                .FirstOrDefaultAsync(s => s.IdSesion == id);

            if (sesion == null)
            {
                return NotFound(new { mensaje = "Sesión no encontrada." });
            }

            return sesion;
        }

        // GET: api/SesionesUsuario/usuario/5
        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<SesionesUsuario>>> GetSesionesByUsuario(int idUsuario)
        {
            return await _context.SesionesUsuarios
                .Include(s => s.UsuarioNavigation)
                .Where(s => s.Usuario == idUsuario)
                .OrderByDescending(s => s.FechaInicio)
                .ToListAsync();
        }

        // GET: api/SesionesUsuario/activas
        [HttpGet("activas")]
        public async Task<ActionResult<IEnumerable<SesionesUsuario>>> GetSesionesActivas()
        {
            return await _context.SesionesUsuarios
                .Include(s => s.UsuarioNavigation)
                .Where(s => s.FechaFin == null)
                .OrderByDescending(s => s.FechaInicio)
                .ToListAsync();
        }

        // POST: api/SesionesUsuario
        [HttpPost]
        public async Task<ActionResult<SesionesUsuario>> PostSesion(SesionesUsuario sesion)
        {
            // Validar que el usuario existe
            var usuario = await _context.Usuarios.FindAsync(sesion.Usuario);
            if (usuario == null)
            {
                return BadRequest(new { mensaje = "El usuario no existe." });
            }

            // Validar que el usuario no tiene una sesión activa
            var sesionActiva = await _context.SesionesUsuarios
                .FirstOrDefaultAsync(s => s.Usuario == sesion.Usuario && s.FechaFin == null);
            if (sesionActiva != null)
            {
                return BadRequest(new { mensaje = "El usuario ya tiene una sesión activa." });
            }

            // Establecer la fecha de inicio
            sesion.FechaInicio = DateTime.Now;
            sesion.FechaFin = null;

            _context.SesionesUsuarios.Add(sesion);
            await _context.SaveChangesAsync();

            // Registrar la acción en el historial
            var historialAccion = new HistorialAccione
            {
                Usuario = sesion.Usuario,
                TipoAccion = "login",
                Descripcion = $"Inicio de sesión desde {sesion.Ip}",
                FechaAccion = DateTime.Now
            };
            _context.HistorialAcciones.Add(historialAccion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSesion), new { id = sesion.IdSesion }, sesion);
        }

        // PUT: api/SesionesUsuario/5/cerrar
        [HttpPut("{id}/cerrar")]
        public async Task<IActionResult> CerrarSesion(int id)
        {
            var sesion = await _context.SesionesUsuarios.FindAsync(id);
            if (sesion == null)
            {
                return NotFound(new { mensaje = "Sesión no encontrada." });
            }

            if (sesion.FechaFin != null)
            {
                return BadRequest(new { mensaje = "La sesión ya está cerrada." });
            }

            sesion.FechaFin = DateTime.Now;
            await _context.SaveChangesAsync();

            // Registrar la acción en el historial
            var historialAccion = new HistorialAccione
            {
                Usuario = sesion.Usuario,
                TipoAccion = "logout",
                Descripcion = $"Cierre de sesión desde {sesion.Ip}",
                FechaAccion = DateTime.Now
            };
            _context.HistorialAcciones.Add(historialAccion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/SesionesUsuario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSesion(int id)
        {
            var sesion = await _context.SesionesUsuarios.FindAsync(id);
            if (sesion == null)
            {
                return NotFound(new { mensaje = "Sesión no encontrada." });
            }

            _context.SesionesUsuarios.Remove(sesion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SesionExists(int id)
        {
            return _context.SesionesUsuarios.Any(e => e.IdSesion == id);
        }
    }
} 