using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly PublicContext _context;

        public ReportesController(PublicContext context)
        {
            _context = context;
        }

        // GET: api/Reportes/resumen-aportaciones
        [HttpGet("resumen-aportaciones")]
        public async Task<ActionResult<IEnumerable<VistaResumenAportacione>>> GetResumenAportaciones()
        {
            return await _context.VistaResumenAportaciones
                .OrderByDescending(r => r.TotalAportaciones)
                .ToListAsync();
        }

        // GET: api/Reportes/resumen-aportaciones/socio/5
        [HttpGet("resumen-aportaciones/socio/{idSocio}")]
        public async Task<ActionResult<IEnumerable<VistaResumenAportacione>>> GetResumenAportacionesBySocio(int idSocio)
        {
            return await _context.VistaResumenAportaciones
                .Where(r => r.IdSocio == idSocio)
                .OrderByDescending(r => r.TotalAportaciones)
                .ToListAsync();
        }

        // GET: api/Reportes/historial-ahorros
        [HttpGet("historial-ahorros")]
        public async Task<ActionResult<IEnumerable<VistaHistorialAhorro>>> GetHistorialAhorros()
        {
            return await _context.VistaHistorialAhorros
                .OrderByDescending(h => h.FechaMovimiento)
                .ToListAsync();
        }

        // GET: api/Reportes/historial-ahorros/cuenta/5
        [HttpGet("historial-ahorros/cuenta/{idCuenta}")]
        public async Task<ActionResult<IEnumerable<VistaHistorialAhorro>>> GetHistorialAhorrosByCuenta(int idCuenta)
        {
            return await _context.VistaHistorialAhorros
                .Where(h => h.IdCuentaAhorro == idCuenta)
                .OrderByDescending(h => h.FechaMovimiento)
                .ToListAsync();
        }

        // GET: api/Reportes/cartera-creditos
        [HttpGet("cartera-creditos")]
        public async Task<ActionResult<IEnumerable<VistaCarteraCredito>>> GetCarteraCreditos()
        {
            return await _context.VistaCarteraCreditos
                .OrderByDescending(c => c.SaldoPendiente)
                .ToListAsync();
        }

        // GET: api/Reportes/cartera-creditos/socio/5
        [HttpGet("cartera-creditos/socio/{idSocio}")]
        public async Task<ActionResult<IEnumerable<VistaCarteraCredito>>> GetCarteraCreditosBySocio(int idSocio)
        {
            return await _context.VistaCarteraCreditos
                .Where(c => c.IdSocio == idSocio)
                .OrderByDescending(c => c.SaldoPendiente)
                .ToListAsync();
        }

        // GET: api/Reportes/cartera-creditos/estado/activo
        [HttpGet("cartera-creditos/estado/{estado}")]
        public async Task<ActionResult<IEnumerable<VistaCarteraCredito>>> GetCarteraCreditosByEstado(string estado)
        {
            return await _context.VistaCarteraCreditos
                .Where(c => c.Estado == estado)
                .OrderByDescending(c => c.SaldoPendiente)
                .ToListAsync();
        }
    }
} 