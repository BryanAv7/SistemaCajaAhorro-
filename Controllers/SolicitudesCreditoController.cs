using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudesCreditoController : ControllerBase
    {
        private readonly PublicContext _context;

        public SolicitudesCreditoController(PublicContext context)
        {
            _context = context;
        }

        // GET: api/SolicitudesCredito
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SolicitudesCredito>>> GetSolicitudesCredito()
        {
            return await _context.SolicitudesCreditos
                .Include(s => s.IdSocioNavigation)
                .Include(s => s.UsuarioEvaluadorNavigation)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();
        }

        // GET: api/SolicitudesCredito/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SolicitudesCredito>> GetSolicitudCredito(int id)
        {
            var solicitud = await _context.SolicitudesCreditos
                .Include(s => s.IdSocioNavigation)
                .Include(s => s.UsuarioEvaluadorNavigation)
                .FirstOrDefaultAsync(s => s.IdSolicitud == id);

            if (solicitud == null)
            {
                return NotFound(new { mensaje = "Solicitud de crédito no encontrada." });
            }

            return solicitud;
        }

        // GET: api/SolicitudesCredito/socio/5
        [HttpGet("socio/{idSocio}")]
        public async Task<ActionResult<IEnumerable<SolicitudesCredito>>> GetSolicitudesBySocio(int idSocio)
        {
            return await _context.SolicitudesCreditos
                .Include(s => s.UsuarioEvaluadorNavigation)
                .Where(s => s.IdSocio == idSocio)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();
        }

        // GET: api/SolicitudesCredito/estado/pendiente
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<SolicitudesCredito>>> GetSolicitudesByEstado(string estado)
        {
            return await _context.SolicitudesCreditos
                .Include(s => s.IdSocioNavigation)
                .Include(s => s.UsuarioEvaluadorNavigation)
                .Where(s => s.Estado == estado)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();
        }

        // POST: api/SolicitudesCredito
        [HttpPost]
        public async Task<ActionResult<SolicitudesCredito>> PostSolicitudCredito(SolicitudesCredito solicitud)
        {
            // Validar que el socio existe y está activo
            var socio = await _context.Socios.FindAsync(solicitud.IdSocio);
            if (socio == null)
            {
                return BadRequest(new { mensaje = "El socio no existe." });
            }
            if (socio.Estado != "activo")
            {
                return BadRequest(new { mensaje = "El socio no está activo." });
            }

            // Validar que el socio no tenga solicitudes pendientes
            var solicitudPendiente = await _context.SolicitudesCreditos
                .FirstOrDefaultAsync(s => s.IdSocio == solicitud.IdSocio && s.Estado == "pendiente");
            if (solicitudPendiente != null)
            {
                return BadRequest(new { mensaje = "El socio ya tiene una solicitud pendiente." });
            }

            // Validar que el socio no tenga créditos activos
            var creditoActivo = await _context.Creditos
                .FirstOrDefaultAsync(c => c.IdSocio == solicitud.IdSocio && c.Estado == "activo");
            if (creditoActivo != null)
            {
                return BadRequest(new { mensaje = "El socio ya tiene un crédito activo." });
            }

            // Establecer valores por defecto
            solicitud.FechaSolicitud = DateOnly.FromDateTime(DateTime.Now);
            solicitud.Estado = "pendiente";

            _context.SolicitudesCreditos.Add(solicitud);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSolicitudCredito), new { id = solicitud.IdSolicitud }, solicitud);
        }

        // PUT: api/SolicitudesCredito/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSolicitudCredito(int id, SolicitudesCredito solicitud)
        {
            if (id != solicitud.IdSolicitud)
            {
                return BadRequest(new { mensaje = "El ID de la solicitud no coincide." });
            }

            var solicitudExistente = await _context.SolicitudesCreditos.FindAsync(id);
            if (solicitudExistente == null)
            {
                return NotFound(new { mensaje = "Solicitud de crédito no encontrada." });
            }

            // No permitir modificar ciertos campos
            solicitud.FechaSolicitud = solicitudExistente.FechaSolicitud;
            solicitud.IdSocio = solicitudExistente.IdSocio;

            _context.Entry(solicitud).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SolicitudCreditoExists(id))
                {
                    return NotFound(new { mensaje = "Solicitud de crédito no encontrada." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/SolicitudesCredito/5/evaluar
        [HttpPut("{id}/evaluar")]
        public async Task<IActionResult> EvaluarSolicitud(int id, [FromBody] EvaluacionSolicitud evaluacion)
        {
            var solicitud = await _context.SolicitudesCreditos.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound(new { mensaje = "Solicitud de crédito no encontrada." });
            }

            if (solicitud.Estado != "pendiente")
            {
                return BadRequest(new { mensaje = "La solicitud ya ha sido evaluada." });
            }

            // Validar que el evaluador existe
            var evaluador = await _context.Usuarios.FindAsync(evaluacion.UsuarioEvaluador);
            if (evaluador == null)
            {
                return BadRequest(new { mensaje = "El evaluador no existe." });
            }

            solicitud.Estado = evaluacion.Estado;
            solicitud.MotivoRechazo = evaluacion.MotivoRechazo;
            solicitud.UsuarioEvaluador = evaluacion.UsuarioEvaluador;
            solicitud.FechaEvaluacion = DateOnly.FromDateTime(DateTime.Now);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SolicitudCreditoExists(id))
                {
                    return NotFound(new { mensaje = "Solicitud de crédito no encontrada." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/SolicitudesCredito/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSolicitudCredito(int id)
        {
            var solicitud = await _context.SolicitudesCreditos.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound(new { mensaje = "Solicitud de crédito no encontrada." });
            }

            // Solo permitir eliminar solicitudes pendientes
            if (solicitud.Estado != "pendiente")
            {
                return BadRequest(new { mensaje = "Solo se pueden eliminar solicitudes pendientes." });
            }

            _context.SolicitudesCreditos.Remove(solicitud);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SolicitudCreditoExists(int id)
        {
            return _context.SolicitudesCreditos.Any(e => e.IdSolicitud == id);
        }
    }

    public class EvaluacionSolicitud
    {
        public string Estado { get; set; } = null!;
        public string? MotivoRechazo { get; set; }
        public int UsuarioEvaluador { get; set; }
    }
} 