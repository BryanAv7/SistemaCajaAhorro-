using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SistemaCajaAhorro.Controllers;
using SistemaCajaAhorro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaCajaAhorroTests
{
    [TestClass]
    public class HistorialAccioneControllerTests
    {
        private Mock<PublicContext> _mockContext = null!;
        private HistorialAccioneController _controller = null!;
        private Mock<DbSet<HistorialAccione>> _mockHistorialAccionesDbSet = null!;
        private Mock<DbSet<Usuario>> _mockUsuariosDbSet = null!;

        public HistorialAccioneControllerTests()
        {
            _mockContext = new Mock<PublicContext>();
            _mockHistorialAccionesDbSet = new Mock<DbSet<HistorialAccione>>();
            _mockUsuariosDbSet = new Mock<DbSet<Usuario>>();
        }

        [TestInitialize]
        public void Setup()
        {
            var historiales = new List<HistorialAccione>
            {
                new HistorialAccione { Id = 1, Accion = "Test", Fecha = DateTime.Now, UsuarioId = 1 },
                new HistorialAccione { Id = 2, Accion = "Test2", Fecha = DateTime.Now, UsuarioId = 1 }
            };

            _mockHistorialAccionesDbSet = MockDbSetHelper.CreateMockDbSet(historiales);
            _mockContext.Setup(c => c.HistorialAcciones).Returns(_mockHistorialAccionesDbSet.Object);

            var usuarios = new List<Usuario>
            {
                new Usuario { IdUsuario = 1, Nombres = "Usuario1" }
            }.AsQueryable();

            // Configuración para operaciones asíncronas
            var asyncProvider = new Mock<IAsyncQueryProvider>();
            asyncProvider
                .Setup(x => x.ExecuteAsync<HistorialAccione>(
                    It.IsAny<Expression>(),
                    It.IsAny<CancellationToken>()))
                .Returns(historiales.First());

            _mockHistorialAccionesDbSet.As<IQueryable<HistorialAccione>>().Setup(m => m.Provider).Returns(asyncProvider.Object);
            _mockHistorialAccionesDbSet.As<IQueryable<HistorialAccione>>().Setup(m => m.Expression).Returns(historiales.AsQueryable().Expression);
            _mockHistorialAccionesDbSet.As<IQueryable<HistorialAccione>>().Setup(m => m.ElementType).Returns(historiales.AsQueryable().ElementType);
            _mockHistorialAccionesDbSet.As<IQueryable<HistorialAccione>>().Setup(m => m.GetEnumerator()).Returns(historiales.GetEnumerator());

            _mockHistorialAccionesDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((object[] ids, CancellationToken token) => historiales.FirstOrDefault(h => h.IdHistorial == (int)ids[0])!);

            _mockUsuariosDbSet.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(asyncProvider.Object);
            _mockUsuariosDbSet.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(usuarios.Expression);
            _mockUsuariosDbSet.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(usuarios.ElementType);
            _mockUsuariosDbSet.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(usuarios.GetEnumerator());

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _controller = new HistorialAccioneController(_mockContext.Object);
        }

        private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public T Current => _inner.Current;

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_inner.MoveNext());
            }

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return new ValueTask();
            }
        }

        [TestMethod]
        public async Task GetHistorialAcciones_ReturnsAllHistorial()
        {
            // Act
            var result = await _controller.GetHistorialAcciones();

            // Assert
            Assert.IsNotNull(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var historiales = okResult.Value as IEnumerable<HistorialAccione>;
            Assert.IsNotNull(historiales);
            Assert.AreEqual(2, historiales.Count());
        }

        [TestMethod]
        public async Task GetHistorialAccione_WithValidId_ReturnsHistorial()
        {
            // Act
            var result = await _controller.GetHistorialAccione(1);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var historial = okResult.Value as HistorialAccione;
            Assert.IsNotNull(historial);
            Assert.AreEqual(1, historial.IdHistorial);
        }

        [TestMethod]
        public async Task GetHistorialAccione_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetHistorialAccione(999);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task PostHistorialAccione_WithValidData_ReturnsCreatedResult()
        {
            // Arrange
            var newHistorial = new HistorialAccione
            {
                IdUsuario = 1,
                Accion = "New Test Action",
                TablaAfectada = "TestTable",
                IdRegistroAfectado = 1,
                ValoresAnteriores = "Old Value",
                ValoresNuevos = "New Value",
                FechaAccion = DateTime.Now
            };

            _mockHistorialAccionesDbSet.Setup(d => d.Add(It.IsAny<HistorialAccione>()))
                .Callback<HistorialAccione>(h => h.IdHistorial = 3);

            // Act
            var result = await _controller.PostHistorialAccione(newHistorial);

            // Assert
            Assert.IsNotNull(result);
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            var historial = createdResult.Value as HistorialAccione;
            Assert.IsNotNull(historial);
            Assert.AreEqual(3, historial.IdHistorial);
        }

        [TestMethod]
        public async Task PutHistorialAccione_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var existingHistorial = new HistorialAccione
            {
                IdHistorial = 1,
                IdUsuario = 1,
                Accion = "Test Action",
                TablaAfectada = "TestTable",
                IdRegistroAfectado = 1,
                ValoresAnteriores = "Old Value",
                ValoresNuevos = "New Value",
                FechaAccion = DateTime.Now
            };

            _mockHistorialAccionesDbSet.Setup(d => d.FindAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingHistorial);

            var updatedHistorial = new HistorialAccione
            {
                IdHistorial = 1,
                IdUsuario = 1,
                Accion = "Updated Test Action",
                TablaAfectada = "TestTable",
                IdRegistroAfectado = 1,
                ValoresAnteriores = "Old Value",
                ValoresNuevos = "Updated Value",
                FechaAccion = DateTime.Now
            };

            // Act
            var result = await _controller.PutHistorialAccione(1, updatedHistorial);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task PutHistorialAccione_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var historial = new HistorialAccione
            {
                IdHistorial = 1,
                IdUsuario = 1,
                Accion = "Test Action",
                TablaAfectada = "TestTable",
                IdRegistroAfectado = 1,
                ValoresAnteriores = "Old Value",
                ValoresNuevos = "New Value",
                FechaAccion = DateTime.Now
            };

            // Act
            var result = await _controller.PutHistorialAccione(2, historial);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task DeleteHistorialAccione_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var existingHistorial = new HistorialAccione
            {
                IdHistorial = 1,
                IdUsuario = 1,
                Accion = "Test Action",
                TablaAfectada = "TestTable",
                IdRegistroAfectado = 1,
                ValoresAnteriores = "Old Value",
                ValoresNuevos = "New Value",
                FechaAccion = DateTime.Now
            };

            _mockHistorialAccionesDbSet.Setup(d => d.FindAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingHistorial);

            // Act
            var result = await _controller.DeleteHistorialAccione(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeleteHistorialAccione_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.DeleteHistorialAccione(999);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
    }
}