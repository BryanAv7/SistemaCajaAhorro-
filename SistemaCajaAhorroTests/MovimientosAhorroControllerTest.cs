using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaCajaAhorro.Controllers;
using SistemaCajaAhorro.Models;

namespace SistemaCajaAhorroTests
{
    [TestClass]
    public class MovimientosAhorroControllerTest
    {
        private DbContextOptions<PublicContext> _dbContextOptions;
        private Usuario _testUser;

        [TestInitialize]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<PublicContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            _testUser = new Usuario
            {
                IdUsuario = 1,
                Nombres = "Test",
                Apellidos = "User",
                Cedula = "1234567890",
                Correo = "",
                Contrasena = "password",
                PerfilAcceso = "admin",
                Estado = true,
                FechaCreacion = DateTime.Now
            };

        }

        [TestMethod]
        public async Task GetMovimientosAhorro_ReturnsAllMovimientos()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.MovimientosAhorros.Add(new MovimientosAhorro { IdMovimiento = 1, Monto = 100, TipoMovimiento = "deposito", FechaMovimiento = DateTime.Now });
                context.SaveChanges();

                var controller = new MovimientosAhorroController(context);
                var result = await controller.GetMovimientosAhorro();

                Assert.IsInstanceOfType(result.Value, typeof(List<MovimientosAhorro>));
                Assert.AreEqual(1, result.Value.Count());
            }
        }

        [TestMethod]
        public async Task GetMovimiento_ReturnsNotFound_WhenNotExists()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                var controller = new MovimientosAhorroController(context);
                var result = await controller.GetMovimiento(999);

                Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            }
        }

        [TestMethod]
        public async Task PostMovimiento_ReturnsBadRequest_WhenCuentaNotExists()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                var controller = new MovimientosAhorroController(context);
                var movimiento = new MovimientosAhorro { IdCuentaAhorro = 1, UsuarioRegistro = 1, TipoMovimiento = "deposito", Monto = 100 };
                var result = await controller.PostMovimiento(movimiento);

                Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            }
        }

        [TestMethod]
        public async Task PostMovimiento_ReturnsBadRequest_WhenUsuarioNotExists()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.CuentasAhorros.Add(new CuentasAhorro { IdCuentaAhorro = 1, SaldoActual = 100, NumeroCuenta = "01", TipoCuenta = "ahorro" });
                context.SaveChanges();

                var controller = new MovimientosAhorroController(context);
                var movimiento = new MovimientosAhorro { IdCuentaAhorro = 1, UsuarioRegistro = 99, TipoMovimiento = "deposito", Monto = 100 };
                var result = await controller.PostMovimiento(movimiento);

                Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            }
        }

        [TestMethod]
        public async Task PostMovimiento_ReturnsBadRequest_WhenTipoMovimientoInvalido()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.CuentasAhorros.Add(new CuentasAhorro { IdCuentaAhorro = 1, SaldoActual = 100, NumeroCuenta = "01", TipoCuenta = "ahorro" });
                context.Usuarios.Add(_testUser);
                context.SaveChanges();

                var controller = new MovimientosAhorroController(context);
                var movimiento = new MovimientosAhorro { IdCuentaAhorro = 1, UsuarioRegistro = 1, TipoMovimiento = "otro", Monto = 100 };
                var result = await controller.PostMovimiento(movimiento);

                Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            }
        }

        [TestMethod]
        public async Task PostMovimiento_ReturnsBadRequest_WhenMontoMenorIgualCero()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.CuentasAhorros.Add(new CuentasAhorro { IdCuentaAhorro = 1, SaldoActual = 100, TipoCuenta = "ahorro", NumeroCuenta = "01" });
                context.Usuarios.Add(_testUser);
                context.SaveChanges();

                var controller = new MovimientosAhorroController(context);
                var movimiento = new MovimientosAhorro { IdCuentaAhorro = 1, UsuarioRegistro = 1, TipoMovimiento = "deposito", Monto = 0 };
                var result = await controller.PostMovimiento(movimiento);

                Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            }
        }

        [TestMethod]
        public async Task PostMovimiento_ReturnsBadRequest_WhenSaldoInsuficienteParaRetiro()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.CuentasAhorros.Add(new CuentasAhorro { IdCuentaAhorro = 1, SaldoActual = 50, TipoCuenta = "ahorro", NumeroCuenta = "01" });
                context.Usuarios.Add(_testUser);
                context.SaveChanges();

                var controller = new MovimientosAhorroController(context);
                var movimiento = new MovimientosAhorro { IdCuentaAhorro = 1, UsuarioRegistro = 1, TipoMovimiento = "retiro", Monto = 100 };
                var result = await controller.PostMovimiento(movimiento);

                Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            }
        }

        [TestMethod]
        public async Task PostMovimiento_CreatesMovimiento_WhenValid()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.CuentasAhorros.Add(new CuentasAhorro { IdCuentaAhorro = 1, SaldoActual = 100, TipoCuenta = "ahorro", NumeroCuenta = "01" });
                context.Usuarios.Add(_testUser);
                context.SaveChanges();

                var controller = new MovimientosAhorroController(context);
                var movimiento = new MovimientosAhorro { IdCuentaAhorro = 1, UsuarioRegistro = 1, TipoMovimiento = "deposito", Monto = 50 };
                var result = await controller.PostMovimiento(movimiento);

                Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            }
        }

        [TestMethod]
        public async Task DeleteMovimiento_ReturnsBadRequest_WhenFechaMovimientoNull()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.MovimientosAhorros.Add(new MovimientosAhorro { IdMovimiento = 1, FechaMovimiento = null });
                context.SaveChanges();

                var controller = new MovimientosAhorroController(context);
                var result = await controller.DeleteMovimiento(1);

                Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            }
        }

        [TestMethod]
        public async Task DeleteMovimiento_ReturnsBadRequest_WhenMovimientoAntiguo()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.MovimientosAhorros.Add(new MovimientosAhorro { IdMovimiento = 1, FechaMovimiento = DateTime.Now.AddDays(-31) });
                context.SaveChanges();

                var controller = new MovimientosAhorroController(context);
                var result = await controller.DeleteMovimiento(1);

                Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            }
        }

        [TestMethod]
        public async Task DeleteMovimiento_DeletesMovimiento_WhenValid()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.MovimientosAhorros.Add(new MovimientosAhorro { IdMovimiento = 1, FechaMovimiento = DateTime.Now, TipoMovimiento = "" });
                context.SaveChanges();

                var controller = new MovimientosAhorroController(context);
                var result = await controller.DeleteMovimiento(1);

                Assert.IsInstanceOfType(result, typeof(NoContentResult));
            }
        }
    }
}
