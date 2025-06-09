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
using System.Threading;
using System.Threading.Tasks;

namespace SistemaCajaAhorroTests
{
    [TestClass]
    public class AportacionesControllerTests
    {
        private Mock<PublicContext> _mockContext = null!;
        private AportacionesController _controller = null!;
        private Mock<DbSet<Aportacione>> _mockAportacionesDbSet = null!;
        private Mock<DbSet<Socio>> _mockSociosDbSet = null!;
        private Mock<DbSet<TiposAportacion>> _mockTiposAportacionDbSet = null!;
        private Mock<DbSet<Usuario>> _mockUsuariosDbSet = null!;

        public AportacionesControllerTests()
        {
            _mockContext = new Mock<PublicContext>();
            _mockAportacionesDbSet = new Mock<DbSet<Aportacione>>();
            _mockSociosDbSet = new Mock<DbSet<Socio>>();
            _mockTiposAportacionDbSet = new Mock<DbSet<TiposAportacion>>();
            _mockUsuariosDbSet = new Mock<DbSet<Usuario>>();
        }

        [TestInitialize]
        public void Setup()
        {
            var aportaciones = new List<Aportacione>
            {
                new Aportacione { Id = 1, Monto = 100, Fecha = DateTime.Now, UsuarioId = 1 },
                new Aportacione { Id = 2, Monto = 200, Fecha = DateTime.Now, UsuarioId = 1 }
            };

            _mockAportacionesDbSet = MockDbSetHelper.CreateMockDbSet(aportaciones);
            _mockContext.Setup(c => c.Aportaciones).Returns(_mockAportacionesDbSet.Object);

            var socios = new List<Socio>
            {
                new Socio { IdSocio = 1, Nombres = "Socio1" }
            }.AsQueryable();

            var tiposAportacion = new List<TiposAportacion>
            {
                new TiposAportacion { IdTipoAportacion = 1, NombreTipo = "Tipo1" }
            }.AsQueryable();

            var usuarios = new List<Usuario>
            {
                new Usuario { IdUsuario = 1, Nombres = "Usuario1" }
            }.AsQueryable();

            // Configuración para operaciones asíncronas
            var asyncProvider = new Mock<IAsyncQueryProvider>();
            asyncProvider
                .Setup(x => x.ExecuteAsync<Aportacione>(
                    It.IsAny<Expression>(),
                    It.IsAny<CancellationToken>()))
                .Returns(aportaciones.First());

            _mockAportacionesDbSet.As<IQueryable<Aportacione>>().Setup(m => m.Provider).Returns(asyncProvider.Object);
            _mockAportacionesDbSet.As<IQueryable<Aportacione>>().Setup(m => m.Expression).Returns(aportaciones.AsQueryable().Expression);
            _mockAportacionesDbSet.As<IQueryable<Aportacione>>().Setup(m => m.ElementType).Returns(aportaciones.AsQueryable().ElementType);
            _mockAportacionesDbSet.As<IQueryable<Aportacione>>().Setup(m => m.GetEnumerator()).Returns(aportaciones.GetEnumerator());

            _mockAportacionesDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((object[] ids, CancellationToken token) => aportaciones.FirstOrDefault(a => a.IdAportacion == (int)ids[0])!);

            _mockSociosDbSet.As<IQueryable<Socio>>().Setup(m => m.Provider).Returns(asyncProvider.Object);
            _mockSociosDbSet.As<IQueryable<Socio>>().Setup(m => m.Expression).Returns(socios.Expression);
            _mockSociosDbSet.As<IQueryable<Socio>>().Setup(m => m.ElementType).Returns(socios.ElementType);
            _mockSociosDbSet.As<IQueryable<Socio>>().Setup(m => m.GetEnumerator()).Returns(socios.GetEnumerator());

            _mockTiposAportacionDbSet.As<IQueryable<TiposAportacion>>().Setup(m => m.Provider).Returns(asyncProvider.Object);
            _mockTiposAportacionDbSet.As<IQueryable<TiposAportacion>>().Setup(m => m.Expression).Returns(tiposAportacion.Expression);
            _mockTiposAportacionDbSet.As<IQueryable<TiposAportacion>>().Setup(m => m.ElementType).Returns(tiposAportacion.ElementType);
            _mockTiposAportacionDbSet.As<IQueryable<TiposAportacion>>().Setup(m => m.GetEnumerator()).Returns(tiposAportacion.GetEnumerator());

            _mockUsuariosDbSet.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(asyncProvider.Object);
            _mockUsuariosDbSet.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(usuarios.Expression);
            _mockUsuariosDbSet.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(usuarios.ElementType);
            _mockUsuariosDbSet.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(usuarios.GetEnumerator());

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _controller = new AportacionesController(_mockContext.Object);
        }

        [TestMethod]
        public async Task GetAportaciones_ReturnsAllAportaciones()
        {
            // Act
            var result = await _controller.GetAportaciones();

            // Assert
            Assert.IsNotNull(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var aportaciones = okResult.Value as IEnumerable<Aportacione>;
            Assert.IsNotNull(aportaciones);
            Assert.AreEqual(2, aportaciones.Count());
        }

        [TestMethod]
        public async Task GetAportacion_WithValidId_ReturnsAportacion()
        {
            // Act
            var result = await _controller.GetAportacion(1);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var aportacion = okResult.Value as Aportacione;
            Assert.IsNotNull(aportacion);
            Assert.AreEqual(1, aportacion.IdAportacion);
        }

        [TestMethod]
        public async Task GetAportacion_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetAportacion(999);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task PostAportacion_WithValidData_ReturnsCreatedResult()
        {
            // Arrange
            var newAportacion = new Aportacione
            {
                IdSocio = 1,
                IdTipoAportacion = 1,
                Monto = 200,
                FechaAportacion = DateOnly.FromDateTime(DateTime.Now),
                Estado = "registrada",
                UsuarioRegistro = 1
            };

            _mockAportacionesDbSet.Setup(d => d.Add(It.IsAny<Aportacione>()))
                .Callback<Aportacione>(a => a.IdAportacion = 3);

            // Act
            var result = await _controller.PostAportacion(newAportacion);

            // Assert
            Assert.IsNotNull(result);
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            var aportacion = createdResult.Value as Aportacione;
            Assert.IsNotNull(aportacion);
            Assert.AreEqual(3, aportacion.IdAportacion);
        }

        [TestMethod]
        public async Task PutAportacion_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var existingAportacion = new Aportacione
            {
                IdAportacion = 1,
                IdSocio = 1,
                IdTipoAportacion = 1,
                Monto = 100,
                FechaAportacion = DateOnly.FromDateTime(DateTime.Now),
                Estado = "registrada",
                UsuarioRegistro = 1
            };

            _mockAportacionesDbSet.Setup(d => d.FindAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAportacion);

            var updatedAportacion = new Aportacione
            {
                IdAportacion = 1,
                IdSocio = 1,
                IdTipoAportacion = 1,
                Monto = 150,
                FechaAportacion = DateOnly.FromDateTime(DateTime.Now),
                Estado = "registrada",
                UsuarioRegistro = 1
            };

            // Act
            var result = await _controller.PutAportacion(1, updatedAportacion);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task PutAportacion_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var aportacion = new Aportacione
            {
                IdAportacion = 1,
                IdSocio = 1,
                IdTipoAportacion = 1,
                Monto = 100,
                FechaAportacion = DateOnly.FromDateTime(DateTime.Now),
                Estado = "registrada",
                UsuarioRegistro = 1
            };

            // Act
            var result = await _controller.PutAportacion(2, aportacion);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task DeleteAportacion_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var existingAportacion = new Aportacione
            {
                IdAportacion = 1,
                IdSocio = 1,
                IdTipoAportacion = 1,
                Monto = 100,
                FechaAportacion = DateOnly.FromDateTime(DateTime.Now),
                Estado = "registrada",
                UsuarioRegistro = 1
            };

            _mockAportacionesDbSet.Setup(d => d.FindAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAportacion);

            // Act
            var result = await _controller.DeleteAportacion(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeleteAportacion_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.DeleteAportacion(999);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
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
    }
}