using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCajaAhorro.Controllers;
using SistemaCajaAhorro.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace SistemaCajaAhorroTests
{
    [TestClass]
    public sealed class UsuariosTest
    {
        private DbContextOptions<PublicContext> _dbContextOptions;
        private Usuario _testUser1;
        private Usuario _testUser2;

        [TestInitialize]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<PublicContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _testUser1 = new Usuario
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
            _testUser2 = new Usuario
            {
                IdUsuario = 2,
                Nombres = "Test2",
                Apellidos = "User2",
                Cedula = "0987654321",
                Correo = "",
                Contrasena = "password2",
                PerfilAcceso = "user",
                Estado = true,
                FechaCreacion = DateTime.Now
            };
        }

        [TestMethod]
        public async Task GetUsuarios_ReturnsAllUsuarios()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.Usuarios.Add(_testUser1);
                context.Usuarios.Add(_testUser2);
                context.SaveChanges();

                var controller = new UsuariosController(context);

                var result = await controller.GetUsuarios();

                Assert.AreEqual(2, result.Value.Count());
            }
        }

        [TestMethod]
        public async Task GetUsuario_ReturnsUsuario_WhenExists()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                context.Usuarios.Add(_testUser1);
                context.SaveChanges();

                var controller = new UsuariosController(context);

                var result = await controller.GetUsuario(1);

                Assert.IsNotNull(result.Value);
                Assert.AreEqual("Test", result.Value.Nombres);
            }
        }

        [TestMethod]
        public async Task GetUsuario_ReturnsNotFound_WhenNotExists()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                var controller = new UsuariosController(context);

                var result = await controller.GetUsuario(99);

                Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            }
        }

        [TestMethod]
        public async Task PostUsuario_CreatesUsuario()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                var controller = new UsuariosController(context);
                var usuario = _testUser2;

                var result = await controller.PostUsuario(usuario);

                var createdResult = result.Result as CreatedAtActionResult;
                Assert.IsNotNull(createdResult);
                var createdUsuario = createdResult.Value as Usuario;
                Assert.AreEqual("Test2", createdUsuario.Nombres);
                Assert.IsTrue(context.Usuarios.Any(u => u.Nombres == "Test2"));
            }
        }

        [TestMethod]
        public async Task PutUsuario_UpdatesUsuario()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                var usuario = _testUser1;
                context.Usuarios.Add(usuario);
                context.SaveChanges();

                var controller = new UsuariosController(context);
                usuario.Nombres = "Maria Actualizada";

                var result = await controller.PutUsuario(1, usuario);

                Assert.IsInstanceOfType(result, typeof(NoContentResult));
                Assert.AreEqual("Maria Actualizada", context.Usuarios.Find(1).Nombres);
            }
        }

        [TestMethod]
        public async Task PutUsuario_ReturnsBadRequest_WhenIdMismatch()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                var usuario = _testUser1;
            ;
                context.Usuarios.Add(usuario);
                context.SaveChanges();

                var controller = new UsuariosController(context);

                var result = await controller.PutUsuario(2, usuario);

                Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            }
        }

        [TestMethod]
        public async Task DeleteUsuario_RemovesUsuario()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                var usuario = _testUser1;
                context.Usuarios.Add(usuario);
                context.SaveChanges();

                var controller = new UsuariosController(context);

                var result = await controller.DeleteUsuario(1);

                Assert.IsInstanceOfType(result, typeof(NoContentResult));
                Assert.IsFalse(context.Usuarios.Any(u => u.IdUsuario == 1));
            }
        }

        [TestMethod]
        public async Task DeleteUsuario_ReturnsNotFound_WhenNotExists()
        {
            using (var context = new PublicContext(_dbContextOptions))
            {
                var controller = new UsuariosController(context);

                var result = await controller.DeleteUsuario(99);

                Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            }
        }
    }
}
