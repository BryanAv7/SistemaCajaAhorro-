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
    public class AportacionesControllerTests
    {
        private DbContextOptions<PublicContext> _dbContextOptions;
        private Aportacione _testAportacion1;
        private Aportacione _testAportacion2;
        private Socio _testSocio;
        private TiposAportacion _testTipoAportacion;
        private Usuario _testUsuario;

        [TestInitialize]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<PublicContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            _testSocio = new Socio
            {
                IdSocio = 1,
                Nombres = "Test",
                Apellidos = "User",
                Cedula = "1234567890",
                Estado = "Activo",
                NumeroSocio = "SOC12345"
            };

            _testTipoAportacion = new TiposAportacion
            {
                IdTipoAportacion = 1,
                NombreTipo = "Test",
                MontoMinimo = 50
            };

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

            _testAportacion1 = new Aportacione
            {
                IdAportacion = 1,
                IdSocio = 1,
                IdTipoAportacion = 1,
                Monto = 100,
                FechaAportacion = DateOnly.FromDateTime(DateTime.Now),
                UsuarioRegistro = 1,
                Estado = "registrada"
            };

            _testAportacion2 = new Aportacione
            {
                IdAportacion = 2,
                IdSocio = 1,
                IdTipoAportacion = 1,
                Monto = 200,
                FechaAportacion = DateOnly.FromDateTime(DateTime.Now),
                UsuarioRegistro = 1,
                Estado = "registrada"
            };
        }

        [TestMethod]
        public async Task GetAportaciones_ReturnsAllAportaciones()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.Socios.Add(_testSocio);
                context.TiposAportacions.Add(_testTipoAportacion);
                context.Usuarios.Add(_testUsuario);
                context.Aportaciones.Add(_testAportacion1);
                context.Aportaciones.Add(_testAportacion2);
                context.SaveChanges();

                var controller = new AportacionesController(context);
                var result = await controller.GetAportaciones();

                Assert.IsNotNull(result.Value);
                Assert.AreEqual(2, result.Value.Count());
            }
        }

        [TestMethod]
        public async Task GetAportacion_WithValidId_ReturnsAportacion()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.Socios.Add(_testSocio);
                context.TiposAportacions.Add(_testTipoAportacion);
                context.Usuarios.Add(_testUsuario);
                context.Aportaciones.Add(_testAportacion1);
                context.SaveChanges();

                var controller = new AportacionesController(context);
                var result = await controller.GetAportacion(1);

                Assert.IsNotNull(result.Value);
                Assert.AreEqual(1, result.Value.IdAportacion);
            }
        }

        [TestMethod]
        public async Task GetAportacion_WithInvalidId_ReturnsNotFound()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                var controller = new AportacionesController(context);
                var result = await controller.GetAportacion(999);

                Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            }
        }

        [TestMethod]
        public async Task PostAportacion_WithValidData_ReturnsCreatedResult()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.Socios.Add(_testSocio);
                context.TiposAportacions.Add(_testTipoAportacion);
                context.Usuarios.Add(_testUsuario);
                context.SaveChanges();

                var controller = new AportacionesController(context);
                var result = await controller.PostAportacion(_testAportacion1);

                Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
                Assert.IsTrue(context.Aportaciones.Any(a => a.IdAportacion == 1));
            }
        }

        [TestMethod]
        public async Task PostAportacion_WithInvalidSocio_ReturnsBadRequest()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.TiposAportacions.Add(_testTipoAportacion);
                context.Usuarios.Add(_testUsuario);
                context.SaveChanges();

                var controller = new AportacionesController(context);
                var result = await controller.PostAportacion(_testAportacion1);

                Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            }
        }

        [TestMethod]
        public async Task PostAportacion_WithInvalidTipoAportacion_ReturnsBadRequest()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.Socios.Add(_testSocio);
                context.Usuarios.Add(_testUsuario);
                context.SaveChanges();

                var controller = new AportacionesController(context);
                var result = await controller.PostAportacion(_testAportacion1);

                Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            }
        }

        [TestMethod]
        public async Task PostAportacion_WithInvalidUsuario_ReturnsBadRequest()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.Socios.Add(_testSocio);
                context.TiposAportacions.Add(_testTipoAportacion);
                context.SaveChanges();

                var controller = new AportacionesController(context);
                var result = await controller.PostAportacion(_testAportacion1);

                Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            }
        }

        [TestMethod]
        public async Task PostAportacion_WithMontoBelowMinimo_ReturnsBadRequest()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.Socios.Add(_testSocio);
                context.TiposAportacions.Add(_testTipoAportacion);
                context.Usuarios.Add(_testUsuario);
                context.SaveChanges();

                var aportacion = new Aportacione
                {
                    IdSocio = 1,
                    IdTipoAportacion = 1,
                    Monto = 25, // Monto menor al mínimo (50)
                    FechaAportacion = DateOnly.FromDateTime(DateTime.Now),
                    UsuarioRegistro = 1,
                    Estado = "registrada"
                };

                var controller = new AportacionesController(context);
                var result = await controller.PostAportacion(aportacion);

                Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            }
        }
    }
}