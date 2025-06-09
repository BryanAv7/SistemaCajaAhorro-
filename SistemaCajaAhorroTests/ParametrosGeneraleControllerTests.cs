using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Controllers;
using SistemaCajaAhorro.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace SistemaCajaAhorroTests
{
    [TestClass]
    public class ParametrosGeneraleControllerTests
    {
        private DbContextOptions<PublicContext> _options;

        [TestInitialize]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<PublicContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new PublicContext(_options);
            context.ParametrosGenerales.Add(new ParametrosGenerale
            {
                IdParametro = 1,
                NombreParametro = "Interés",
                ValorParametro = "5.0",
                FechaActualizacion = DateTime.Now
            });
            context.SaveChanges();
        }

        [TestMethod]
        public async Task GetParametrosGenerales_ReturnsAllItems()
        {
            using var context = new PublicContext(_options);
            var controller = new ParametrosGeneraleController(context);

            var result = await controller.GetParametrosGenerales();

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, result.Value.Count());
        }


        [TestMethod]
        public async Task GetParametrosGenerale_ReturnsItem()
        {
            using var context = new PublicContext(_options);
            var controller = new ParametrosGeneraleController(context);

            var result = await controller.GetParametrosGenerale(1);

            Assert.IsNotNull(result.Value);
            Assert.AreEqual("Interés", result.Value.NombreParametro);
        }

        [TestMethod]
        public async Task GetParametrosGenerale_ReturnsNotFound()
        {
            using var context = new PublicContext(_options);
            var controller = new ParametrosGeneraleController(context);

            var result = await controller.GetParametrosGenerale(99);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task PostParametrosGenerale_CreatesItem()
        {
            using var context = new PublicContext(_options);
            var controller = new ParametrosGeneraleController(context);

            var newParam = new ParametrosGenerale
            {
                NombreParametro = "Mora",
                ValorParametro = "2.0"
            };

            var result = await controller.PostParametrosGenerale(newParam);

            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            Assert.AreEqual(2, context.ParametrosGenerales.Count());
        }

        [TestMethod]
        public async Task PutParametrosGenerale_UpdatesItem()
        {
            using var context = new PublicContext(_options);
            var controller = new ParametrosGeneraleController(context);

            var updateParam = context.ParametrosGenerales.First();
            updateParam.ValorParametro = "6.0";

            var result = await controller.PutParametrosGenerale(updateParam.IdParametro, updateParam);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            Assert.AreEqual("6.0", context.ParametrosGenerales.Find(1)!.ValorParametro);
        }

        [TestMethod]
        public async Task PutParametrosGenerale_ReturnsBadRequest()
        {
            using var context = new PublicContext(_options);
            var controller = new ParametrosGeneraleController(context);

            var updateParam = context.ParametrosGenerales.First();
            var result = await controller.PutParametrosGenerale(999, updateParam);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task DeleteParametrosGenerale_RemovesItem()
        {
            using var context = new PublicContext(_options);
            var controller = new ParametrosGeneraleController(context);

            var result = await controller.DeleteParametrosGenerale(1);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            Assert.AreEqual(0, context.ParametrosGenerales.Count());
        }

        [TestMethod]
        public async Task DeleteParametrosGenerale_ReturnsNotFound()
        {
            using var context = new PublicContext(_options);
            var controller = new ParametrosGeneraleController(context);

            var result = await controller.DeleteParametrosGenerale(99);

            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
    }
}
