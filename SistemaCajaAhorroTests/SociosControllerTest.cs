using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Controllers;
using SistemaCajaAhorro.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SistemaCajaAhorroTests
{
    [TestClass]
    public class SociosTest
    {
        private DbContextOptions<PublicContext> _dbContextOptions;
        private Socio _testSocio;

        [TestInitialize]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<PublicContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // ahora sí compila
                .EnableSensitiveDataLogging()
                .Options;

            _testSocio = new Socio
            {
                IdSocio = 1,
                Cedula = "1234567890",
                Nombres = "Pedro",
                NumeroSocio = "SOC12345",
                Apellidos = "Lopez",
                FechaNacimiento = new DateOnly(1990, 1, 1),
                Estado = "Activo"
            };
        }

        [TestMethod]
        public async Task PostSocio_CreaNuevoSocio()
        {
            using var context = new PublicContext(_dbContextOptions);
            var controller = new SocioController(context);

            var result = await controller.PostSocio(_testSocio);

            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            var socioCreado = createdResult.Value as Socio;
            Assert.AreEqual("Pedro", socioCreado.Nombres);
            Assert.IsTrue(context.Socios.Any(s => s.Cedula == "1234567890"));
        }

        [TestMethod]
        public async Task PutSocio_ActualizaSocio()
        {
            using var context = new PublicContext(_dbContextOptions);
            context.Socios.Add(_testSocio);
            context.SaveChanges();

            var controller = new SocioController(context);
            _testSocio.Nombres = "Pedro Actualizado";

            var result = await controller.PutSocio(1, _testSocio);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            Assert.AreEqual("Pedro Actualizado", context.Socios.Find(1).Nombres);
        }

        [TestMethod]
        public async Task PutSocio_IdNoCoincide_BadRequest()
        {
            using var context = new PublicContext(_dbContextOptions);
            context.Socios.Add(_testSocio);
            context.SaveChanges();

            var controller = new SocioController(context);
            var result = await controller.PutSocio(2, _testSocio);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task DeleteSocio_EliminaSocio()
        {
            using var context = new PublicContext(_dbContextOptions);
            context.Socios.Add(_testSocio);
            context.SaveChanges();

            var controller = new SocioController(context);
            var result = await controller.DeleteSocio(1);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            Assert.IsFalse(context.Socios.Any(s => s.IdSocio == 1));
        }

        [TestMethod]
        public async Task DeleteSocio_SocioNoExiste_NotFound()
        {
            using var context = new PublicContext(_dbContextOptions);
            var controller = new SocioController(context);

            var result = await controller.DeleteSocio(999);

            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
    }
}
