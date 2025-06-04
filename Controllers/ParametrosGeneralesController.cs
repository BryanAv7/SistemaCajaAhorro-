using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParametrosGeneralesController : ControllerBase
    {
        private readonly PublicContext _context;

        public ParametrosGeneralesController(PublicContext context)
        {
            _context = context;
        }

        // GET: api/ParametrosGenerales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParametrosGenerale>>> GetParametrosGenerales()
        {
            return await _context.ParametrosGenerales.ToListAsync();
        }

        // GET: api/ParametrosGenerales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ParametrosGenerale>> GetParametro(int id)
        {
            var parametro = await _context.ParametrosGenerales.FindAsync(id);

            if (parametro == null)
            {
                return NotFound(new { mensaje = "Parámetro no encontrado." });
            }

            return parametro;
        }

        // GET: api/ParametrosGenerales/clave/tasa_interes
        [HttpGet("clave/{clave}")]
        public async Task<ActionResult<ParametrosGenerale>> GetParametroByClave(string clave)
        {
            var parametro = await _context.ParametrosGenerales
                .FirstOrDefaultAsync(p => p.Clave == clave);

            if (parametro == null)
            {
                return NotFound(new { mensaje = "Parámetro no encontrado." });
            }

            return parametro;
        }

        // PUT: api/ParametrosGenerales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParametro(int id, ParametrosGenerale parametro)
        {
            if (id != parametro.IdParametro)
            {
                return BadRequest(new { mensaje = "El ID del parámetro no coincide." });
            }

            // Validar que el usuario existe
            var usuario = await _context.Usuarios.FindAsync(parametro.UsuarioModificacion);
            if (usuario == null)
            {
                return BadRequest(new { mensaje = "El usuario no existe." });
            }

            // Establecer la fecha de modificación
            parametro.FechaModificacion = DateTime.Now;

            _context.Entry(parametro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Registrar la acción en el historial
                var historialAccion = new HistorialAccione
                {
                    Usuario = parametro.UsuarioModificacion,
                    TipoAccion = "modificar",
                    Descripcion = $"Modificación del parámetro {parametro.Clave}",
                    FechaAccion = DateTime.Now
                };
                _context.HistorialAcciones.Add(historialAccion);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParametroExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ParametrosGenerales
        [HttpPost]
        public async Task<ActionResult<ParametrosGenerale>> PostParametro(ParametrosGenerale parametro)
        {
            // Validar que el usuario existe
            var usuario = await _context.Usuarios.FindAsync(parametro.UsuarioModificacion);
            if (usuario == null)
            {
                return BadRequest(new { mensaje = "El usuario no existe." });
            }

            // Validar que no existe un parámetro con la misma clave
            var parametroExistente = await _context.ParametrosGenerales
                .FirstOrDefaultAsync(p => p.Clave == parametro.Clave);
            if (parametroExistente != null)
            {
                return BadRequest(new { mensaje = "Ya existe un parámetro con esta clave." });
            }

            // Establecer la fecha de modificación
            parametro.FechaModificacion = DateTime.Now;

            _context.ParametrosGenerales.Add(parametro);
            await _context.SaveChangesAsync();

            // Registrar la acción en el historial
            var historialAccion = new HistorialAccione
            {
                Usuario = parametro.UsuarioModificacion,
                TipoAccion = "crear",
                Descripcion = $"Creación del parámetro {parametro.Clave}",
                FechaAccion = DateTime.Now
            };
            _context.HistorialAcciones.Add(historialAccion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetParametro), new { id = parametro.IdParametro }, parametro);
        }

        // DELETE: api/ParametrosGenerales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParametro(int id)
        {
            var parametro = await _context.ParametrosGenerales.FindAsync(id);
            if (parametro == null)
            {
                return NotFound(new { mensaje = "Parámetro no encontrado." });
            }

            _context.ParametrosGenerales.Remove(parametro);
            await _context.SaveChangesAsync();

            // Registrar la acción en el historial
            var historialAccion = new HistorialAccione
            {
                Usuario = parametro.UsuarioModificacion,
                TipoAccion = "eliminar",
                Descripcion = $"Eliminación del parámetro {parametro.Clave}",
                FechaAccion = DateTime.Now
            };
            _context.HistorialAcciones.Add(historialAccion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ParametroExists(int id)
        {
            return _context.ParametrosGenerales.Any(e => e.IdParametro == id);
        }
    }
} 