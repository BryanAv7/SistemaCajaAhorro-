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
    public class PlanCuentaControllerTests
    {
        // Contexto de base de datos en memoria para las pruebas
        private PublicContext _context = null!;

        // Controlador que se va a probar
        private PlanCuentaController _controller = null!;

        // Método que se ejecuta antes de cada prueba para preparar el entorno
        [TestInitialize]
        public void Setup()
        {
            // Configura DbContext para usar una base de datos en memoria con un nombre único por prueba
            var options = new DbContextOptionsBuilder<PublicContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Base aislada por prueba
                .Options;

            // Crear nueva instancia del contexto con esas opciones
            _context = new PublicContext(options);

            // Insertar un registro inicial de PlanCuenta para usar en las pruebas
            _context.PlanCuentas.Add(new PlanCuenta
            {
                IdCuenta = 1,
                CodigoCuenta = "1001",
                NombreCategoria = "Caja Principal",
                TipoCuenta = "Activo",
                Estado = true,
                FechaCreacion = DateTime.Now
            });

            // Guardar los cambios en la base de datos en memoria
            _context.SaveChanges();

            // Crear instancia del controlador con el contexto preparado
            _controller = new PlanCuentaController(_context);
        }

        // Prueba para verificar que se obtiene una lista de PlanCuentas
        [TestMethod]
        public async Task GetPlanCuentas_ReturnsList()
        {
            var result = await _controller.GetPlanCuentas();

            // Validar que el resultado no sea nulo y contenga 1 elemento (el inicial)
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, result.Value.Count());
        }

        // Prueba para verificar que se puede obtener un PlanCuenta específico por Id
        [TestMethod]
        public async Task GetPlanCuenta_ReturnsCorrectItem()
        {
            var result = await _controller.GetPlanCuenta(1);

            // Validar que el resultado no sea nulo y que el nombre sea el esperado
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("Caja Principal", result.Value.NombreCategoria);
        }

        // Prueba para verificar que se puede crear un nuevo PlanCuenta
        [TestMethod]
        public async Task PostPlanCuenta_CreatesNewItem()
        {
            // Arrange: nuevo objeto PlanCuenta
            var newPlan = new PlanCuenta
            {
                CodigoCuenta = "2001",
                NombreCategoria = "Bancos",
                TipoCuenta = "Activo",
                Estado = true
            };

            // Act: llamar al método POST del controlador para crear
            var result = await _controller.PostPlanCuenta(newPlan);

            // Assert: validar que la respuesta sea CreatedAtActionResult y el objeto creado tenga datos correctos
            var createdResult = result.Result as CreatedAtActionResult;

            Assert.IsNotNull(createdResult);
            var createdItem = createdResult.Value as PlanCuenta;
            Assert.IsNotNull(createdItem);
            Assert.AreEqual("Bancos", createdItem.NombreCategoria);
        }

        // Prueba para verificar que se puede actualizar un PlanCuenta existente
        [TestMethod]
        public async Task PutPlanCuenta_UpdatesExistingItem()
        {
            // Arrange: obtener un plan existente y cambiar su nombre
            var existingPlan = _context.PlanCuentas.First();
            existingPlan.NombreCategoria = "Caja Actualizada";

            // Act: llamar al método PUT para actualizar el registro
            var updateResult = await _controller.PutPlanCuenta(existingPlan.IdCuenta, existingPlan);

            // Assert: validar que la respuesta sea NoContent (actualización exitosa)
            Assert.IsInstanceOfType(updateResult, typeof(NoContentResult));

            // Consultar de nuevo el registro actualizado y validar el cambio
            var updatedPlan = await _context.PlanCuentas.FindAsync(existingPlan.IdCuenta);

            Assert.IsNotNull(updatedPlan);
            Assert.AreEqual("Caja Actualizada", updatedPlan.NombreCategoria);
        }

        // Prueba para verificar que se puede eliminar un PlanCuenta
        [TestMethod]
        public async Task DeletePlanCuenta_RemovesItem()
        {
            // Act: eliminar la cuenta con Id = 1
            var result = await _controller.DeletePlanCuenta(1);

            // Assert: validar que la respuesta sea NoContent (eliminación exitosa)
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            
            var deleted = await _controller.GetPlanCuenta(1);
            Assert.IsInstanceOfType(deleted.Result, typeof(NotFoundObjectResult));
        }
    }
}
