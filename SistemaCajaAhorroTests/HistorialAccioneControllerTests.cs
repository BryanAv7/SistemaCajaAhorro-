using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SistemaCajaAhorro.Controllers;
using SistemaCajaAhorro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaCajaAhorroTests
{
    [TestClass]
    public class HistorialAccioneControllerTests
    {
        private DbContextOptions<PublicContext> _dbContextOptions;
        private HistorialAccione _testHistorial1;
        private HistorialAccione _testHistorial2;
        private Usuario _testUsuario;

        [TestInitialize]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<PublicContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            _testUsuario = new Usuario
            {
                IdUsuario = 1,
                Nombres = "Test",
                Apellidos = "User",
                Cedula = "1234567890",
                Correo = "test@test.com",
                Contrasena = "password",
                PerfilAcceso = "admin",
                Estado = true
            };

            _testHistorial1 = new HistorialAccione
            {
                IdHistorial = 1,
                IdUsuario = 1,
                Accion = "Crear",
                TablaAfectada = "Socios",
                IdRegistroAfectado = 1,
                ValoresAnteriores = null,
                ValoresNuevos = "Nuevo socio creado",
                FechaAccion = DateTime.Now
            };

            _testHistorial2 = new HistorialAccione
            {
                IdHistorial = 2,
                IdUsuario = 1,
                Accion = "Actualizar",
                TablaAfectada = "Socios",
                IdRegistroAfectado = 1,
                ValoresAnteriores = "Nombre: Juan",
                ValoresNuevos = "Nombre: Juan Pérez",
                FechaAccion = DateTime.Now
            };
        }

        [TestMethod]
        public async Task GetHistorialAcciones_ReturnsAllHistoriales()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.Usuarios.Add(_testUsuario);
                context.HistorialAcciones.Add(_testHistorial1);
                context.HistorialAcciones.Add(_testHistorial2);
                context.SaveChanges();

                var controller = new HistorialAccioneController(context);
                var result = await controller.GetHistorialAcciones();

                Assert.IsNotNull(result.Value);
                Assert.AreEqual(2, result.Value.Count());
            }
        }

        [TestMethod]
        public async Task GetHistorialAccione_WithValidId_ReturnsHistorial()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.Usuarios.Add(_testUsuario);
                context.HistorialAcciones.Add(_testHistorial1);
                context.SaveChanges();

                var controller = new HistorialAccioneController(context);
                var result = await controller.GetHistorialAccione(1);

                Assert.IsNotNull(result.Value);
                Assert.AreEqual(1, result.Value.IdHistorial);
            }
        }

        [TestMethod]
        public async Task GetHistorialAccione_WithInvalidId_ReturnsNotFound()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                var controller = new HistorialAccioneController(context);
                var result = await controller.GetHistorialAccione(999);

                Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            }
        }

        [TestMethod]
        public async Task PostHistorialAccione_WithValidData_ReturnsCreatedResult()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.Usuarios.Add(_testUsuario);
                context.SaveChanges();

                var controller = new HistorialAccioneController(context);
                var result = await controller.PostHistorialAccione(_testHistorial1);

                Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
                Assert.IsTrue(context.HistorialAcciones.Any(h => h.IdHistorial == 1));
            }
        }

        [TestMethod]
        public async Task PostHistorialAccione_WithInvalidUsuario_ReturnsBadRequest()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                var controller = new HistorialAccioneController(context);
                var historial = new HistorialAccione
                {
                    IdUsuario = 999, // ID de usuario inválido
                    Accion = "Crear",
                    TablaAfectada = "Socios",
                    IdRegistroAfectado = 2,
                    ValoresAnteriores = null,
                    ValoresNuevos = "Nuevo socio creado",
                    FechaAccion = DateTime.Now
                };

                var result = await controller.PostHistorialAccione(historial);

                Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            }
        }

        [TestMethod]
        public async Task PutHistorialAccione_WithValidData_ReturnsNoContent()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.Usuarios.Add(_testUsuario);
                context.HistorialAcciones.Add(_testHistorial1);
                context.SaveChanges();

                var controller = new HistorialAccioneController(context);
                _testHistorial1.Accion = "Updated Test Action";
                _testHistorial1.ValoresNuevos = "Updated Value";

                var result = await controller.PutHistorialAccione(1, _testHistorial1);

                Assert.IsInstanceOfType(result, typeof(NoContentResult));
            }
        }

        [TestMethod]
        public async Task PutHistorialAccione_IdMismatch_ReturnsBadRequest()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.Usuarios.Add(_testUsuario);
                context.HistorialAcciones.Add(_testHistorial1);
                context.SaveChanges();

                var controller = new HistorialAccioneController(context);
                var result = await controller.PutHistorialAccione(2, _testHistorial1);

                Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            }
        }

        [TestMethod]
        public async Task DeleteHistorialAccione_WithValidId_ReturnsNoContent()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.Usuarios.Add(_testUsuario);
                context.HistorialAcciones.Add(_testHistorial1);
                context.SaveChanges();

                var controller = new HistorialAccioneController(context);
                var result = await controller.DeleteHistorialAccione(1);

                Assert.IsInstanceOfType(result, typeof(NoContentResult));
            }
        }

        [TestMethod]
        public async Task DeleteHistorialAccione_WithInvalidId_ReturnsNotFound()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                var controller = new HistorialAccioneController(context);
                var result = await controller.DeleteHistorialAccione(999);

                Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            }
        }
    }
}