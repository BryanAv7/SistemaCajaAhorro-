using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Controllers;
using SistemaCajaAhorro.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace SistemaCajaAhorroTests
{
    [TestClass]  
    public class CuentasAhorroControllerTests
    {
        
        private PublicContext _context = null!;

        // Controlador
        private CuentasAhorroController _controller = null!;

        // Método que se ejecuta antes de cada prueba para preparar el entorno
        [TestInitialize]
        public void Setup()
        {
            // Configura el DbContext para usar una base de datos en memoria
            // Se usa un nombre único para que cada prueba tenga su base aislada
            var options = new DbContextOptionsBuilder<PublicContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_CuentasAhorro_{System.Guid.NewGuid()}")
                .EnableSensitiveDataLogging()  // Para facilitar la depuración mostrando datos sensibles
                .Options;

            // Crear una nueva instancia 
            _context = new PublicContext(options);

            // "Sembrar" (seed) un socio necesario para que las cuentas puedan referenciarlo
            if (!_context.Socios.Any())
            {
                _context.Socios.Add(new Socio
                {
                    IdSocio = 1,
                    Nombres = "Juan",
                    Apellidos = "Pérez",
                    Cedula = "123456789",
                    NumeroSocio = "SOC12345"  
                });
                _context.SaveChanges();  // Guardar cambios en la base en memoria
            }

            // Crear instancia 
            _controller = new CuentasAhorroController(_context);
        }

        // Prueba para verificar que se puede crear una cuenta de ahorro correctamente
        [TestMethod]
        public async Task PostCuentaAhorro_DeberiaCrearCuenta()
        {
            // Arrange: preparar una nueva cuenta con IdSocio existente y tipo "ahorro"
            var nuevaCuenta = new CuentasAhorro
            {
                IdSocio = 1,
                TipoCuenta = "ahorro"
            };

            // Act: llamar al método POST del controlador para crear la cuenta
            var actionResult = await _controller.PostCuentaAhorro(nuevaCuenta);

            // Assert: comprobar que el resultado es un CreatedAtActionResult
            var createdResult = actionResult.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);

            // Extraer la cuenta creada y validar sus propiedades
            var cuentaCreada = createdResult.Value as CuentasAhorro;
            Assert.IsNotNull(cuentaCreada);
            Assert.AreEqual("activa", cuentaCreada.Estado);  // Estado inicial esperado
            Assert.AreEqual(0, cuentaCreada.SaldoActual);   // Saldo inicial esperado
            Assert.AreEqual("ahorro", cuentaCreada.TipoCuenta);
            Assert.AreEqual(1, cuentaCreada.IdSocio);
            Assert.IsFalse(string.IsNullOrEmpty(cuentaCreada.NumeroCuenta));  
        }

        // Prueba para verificar que se puede obtener una cuenta existente
        [TestMethod]
        public async Task GetCuentaAhorro_DeberiaRetornarCuentaExistente()
        {
            // Arrange: Crear primero una cuenta para luego obtenerla
            var cuenta = new CuentasAhorro
            {
                IdSocio = 1,
                TipoCuenta = "ahorro"
            };
            var postResult = await _controller.PostCuentaAhorro(cuenta);
            var created = (postResult.Result as CreatedAtActionResult)!.Value as CuentasAhorro;

            // Act: obtener la cuenta creada usando su Id
            var getResult = await _controller.GetCuentaAhorro(created!.IdCuentaAhorro);

            // Assert: validar que la cuenta obtenida es igual a la creada
            Assert.IsNotNull(getResult.Value);
            Assert.AreEqual(created.IdCuentaAhorro, getResult.Value.IdCuentaAhorro);
            Assert.AreEqual(created.NumeroCuenta, getResult.Value.NumeroCuenta);
        }

        // Prueba para verificar que se puede actualizar una cuenta existente
        [TestMethod]
        public async Task PutCuentaAhorro_DeberiaActualizarCuenta()
        {
            // Arrange: Crear cuenta para actualizarla luego
            var cuenta = new CuentasAhorro
            {
                IdSocio = 1,
                TipoCuenta = "ahorro"
            };
            var postResult = await _controller.PostCuentaAhorro(cuenta);
            var created = (postResult.Result as CreatedAtActionResult)!.Value as CuentasAhorro;

            // Modificar un campo de la cuenta, por ejemplo su estado
            created!.Estado = "inactiva";

            // Act: llamar al método PUT para actualizar la cuenta
            var putResult = await _controller.PutCuentaAhorro(created.IdCuentaAhorro, created);

            // Assert: verificar que el resultado es NoContent (actualización exitosa)
            Assert.IsInstanceOfType(putResult, typeof(NoContentResult));

            // Consultar la cuenta actualizada en el contexto y validar el cambio
            var cuentaActualizada = await _context.CuentasAhorros.FindAsync(created.IdCuentaAhorro);
            Assert.IsNotNull(cuentaActualizada);
            Assert.AreEqual("inactiva", cuentaActualizada!.Estado);
        }

        // Método que se ejecuta después de cada prueba para limpiar recursos
        [TestCleanup]
        public void Cleanup()
        {
            // Eliminar la base de datos en memoria para evitar contaminación entre pruebas
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
