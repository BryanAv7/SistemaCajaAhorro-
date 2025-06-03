using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditosController : ControllerBase
    {
        private readonly PublicContext _context;

        public CreditosController(PublicContext context)
        {
            _context = context;
        }

        // GET: api/Creditos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Credito>>> GetCreditos()
        {
            return await _context.Creditos
                .Include(c => c.IdSocioNavigation)
                .Include(c => c.UsuarioRegistroNavigation)
                .OrderByDescending(c => c.FechaDesembolso)
                .ToListAsync();
        }

        // GET: api/Creditos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Credito>> GetCredito(int id)
        {
            var credito = await _context.Creditos
                .Include(c => c.IdSocioNavigation)
                .Include(c => c.UsuarioRegistroNavigation)
                .Include(c => c.TablaAmortizacions)
                .FirstOrDefaultAsync(c => c.IdCredito == id);

            if (credito == null)
            {
                return NotFound(new { mensaje = "Crédito no encontrado." });
            }

            return credito;
        }

        // GET: api/Creditos/socio/5
        [HttpGet("socio/{idSocio}")]
        public async Task<ActionResult<IEnumerable<Credito>>> GetCreditosBySocio(int idSocio)
        {
            return await _context.Creditos
                .Include(c => c.UsuarioRegistroNavigation)
                .Where(c => c.IdSocio == idSocio)
                .OrderByDescending(c => c.FechaDesembolso)
                .ToListAsync();
        }

        // GET: api/Creditos/estado/activo
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<Credito>>> GetCreditosByEstado(string estado)
        {
            return await _context.Creditos
                .Include(c => c.IdSocioNavigation)
                .Include(c => c.UsuarioRegistroNavigation)
                .Where(c => c.Estado == estado)
                .OrderByDescending(c => c.FechaDesembolso)
                .ToListAsync();
        }

        // POST: api/Creditos
        [HttpPost]
        public async Task<ActionResult<Credito>> PostCredito(Credito credito)
        {
            // Validar que el socio existe y está activo
            var socio = await _context.Socios.FindAsync(credito.IdSocio);
            if (socio == null)
            {
                return BadRequest(new { mensaje = "El socio no existe." });
            }
            if (socio.Estado != "activo")
            {
                return BadRequest(new { mensaje = "El socio no está activo." });
            }

            // Validar que el usuario existe
            var usuario = await _context.Usuarios.FindAsync(credito.UsuarioRegistro);
            if (usuario == null)
            {
                return BadRequest(new { mensaje = "El usuario no existe." });
            }

            // Validar que existe una solicitud aprobada
            var solicitud = await _context.SolicitudesCreditos
                .FirstOrDefaultAsync(s => s.IdSocio == credito.IdSocio && 
                                        s.Estado == "aprobada" &&
                                        s.MontoSolicitado == credito.MontoCredito);
            if (solicitud == null)
            {
                return BadRequest(new { mensaje = "No existe una solicitud aprobada para este monto." });
            }

            // Generar número de crédito único
            credito.NumeroCredito = await GenerarNumeroCredito();
            credito.Estado = "activo";
            credito.FechaDesembolso = DateOnly.FromDateTime(DateTime.Now);
            credito.FechaRegistro = DateTime.Now;

            // Calcular tabla de amortización
            var tablaAmortizacion = CalcularTablaAmortizacion(credito);
            credito.TablaAmortizacions = tablaAmortizacion;

            _context.Creditos.Add(credito);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCredito), new { id = credito.IdCredito }, credito);
        }

        // PUT: api/Creditos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCredito(int id, Credito credito)
        {
            if (id != credito.IdCredito)
            {
                return BadRequest(new { mensaje = "El ID del crédito no coincide." });
            }

            var creditoExistente = await _context.Creditos.FindAsync(id);
            if (creditoExistente == null)
            {
                return NotFound(new { mensaje = "Crédito no encontrado." });
            }

            // No permitir modificar ciertos campos
            credito.NumeroCredito = creditoExistente.NumeroCredito;
            credito.FechaDesembolso = creditoExistente.FechaDesembolso;
            credito.FechaRegistro = creditoExistente.FechaRegistro;
            credito.IdSocio = creditoExistente.IdSocio;
            credito.MontoCredito = creditoExistente.MontoCredito;
            credito.PlazoMeses = creditoExistente.PlazoMeses;
            credito.TasaInteres = creditoExistente.TasaInteres;

            _context.Entry(credito).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CreditoExists(id))
                {
                    return NotFound(new { mensaje = "Crédito no encontrado." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/Creditos/5/estado
        [HttpPut("{id}/estado")]
        public async Task<IActionResult> CambiarEstadoCredito(int id, [FromBody] CambioEstadoCredito cambioEstado)
        {
            var credito = await _context.Creditos.FindAsync(id);
            if (credito == null)
            {
                return NotFound(new { mensaje = "Crédito no encontrado." });
            }

            // Validar que el usuario existe
            var usuario = await _context.Usuarios.FindAsync(cambioEstado.UsuarioRegistro);
            if (usuario == null)
            {
                return BadRequest(new { mensaje = "El usuario no existe." });
            }

            credito.Estado = cambioEstado.NuevoEstado;
            credito.Observaciones = cambioEstado.Observaciones;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CreditoExists(id))
                {
                    return NotFound(new { mensaje = "Crédito no encontrado." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool CreditoExists(int id)
        {
            return _context.Creditos.Any(e => e.IdCredito == id);
        }

        private async Task<string> GenerarNumeroCredito()
        {
            // Obtener el último número de crédito
            var ultimoCredito = await _context.Creditos
                .OrderByDescending(c => c.NumeroCredito)
                .FirstOrDefaultAsync();

            int numero = 1;
            if (ultimoCredito != null)
            {
                // Extraer el número del último crédito y sumar 1
                string ultimoNumero = ultimoCredito.NumeroCredito.Substring(3);
                if (int.TryParse(ultimoNumero, out int ultimoNumeroInt))
                {
                    numero = ultimoNumeroInt + 1;
                }
            }

            // Formato: CRD-00000001
            return $"CRD-{numero:D8}";
        }

        private List<TablaAmortizacion> CalcularTablaAmortizacion(Credito credito)
        {
            var tablaAmortizacion = new List<TablaAmortizacion>();
            decimal saldoInicial = credito.MontoCredito;
            decimal tasaMensual = credito.TasaInteres / 100 / 12;
            decimal cuotaMensual = credito.MontoCredito * (tasaMensual * (decimal)Math.Pow(1 + (double)tasaMensual, credito.PlazoMeses)) / 
                                  ((decimal)Math.Pow(1 + (double)tasaMensual, credito.PlazoMeses) - 1);

            for (int i = 1; i <= credito.PlazoMeses; i++)
            {
                decimal interes = saldoInicial * tasaMensual;
                decimal capital = cuotaMensual - interes;
                decimal saldoFinal = saldoInicial - capital;

                tablaAmortizacion.Add(new TablaAmortizacion
                {
                    NumeroCuota = i,
                    FechaVencimiento = credito.FechaDesembolso.AddMonths(i),
                    SaldoInicial = saldoInicial,
                    CuotaMensual = cuotaMensual,
                    Interes = interes,
                    Capital = capital,
                    SaldoFinal = saldoFinal,
                    Estado = "pendiente"
                });

                saldoInicial = saldoFinal;
            }

            return tablaAmortizacion;
        }
    }

    public class CambioEstadoCredito
    {
        public string NuevoEstado { get; set; } = null!;
        public string? Observaciones { get; set; }
        public int UsuarioRegistro { get; set; }
    }
} 