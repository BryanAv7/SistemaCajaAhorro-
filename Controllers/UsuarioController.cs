// Librerias
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Models;


namespace SistemaCajaAhorro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly SistemaCajaAhorroContext _context;

        public UsuarioController(SistemaCajaAhorroContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios --Lista a todos los usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/id  -- Obtengo el usuario por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no existe." }); ;
            }

            return usuario;
        }

        // POST: api/Usuarios -- Crear  un nuevo usuario
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            usuario.FechaCreacion = DateTime.Now;
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, usuario);
        }

        // PUT: api/Usuarios/id  -- Actualizar un usuario 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound(new { mensaje = "Usuario no encontrado." }); ;
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Usuarios/id  -- Eliminar un usuario por ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado." });
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Método auxiliar para verificar si un usuario existe
        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}