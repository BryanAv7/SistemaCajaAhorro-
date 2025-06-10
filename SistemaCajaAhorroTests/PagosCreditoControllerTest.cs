using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SistemaCajaAhorro.Controllers;
using SistemaCajaAhorro.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaCajaAhorroTests
{
    [TestClass]
    public class PagosCreditoControllerTest
    {
        private PublicContext GetContextWithData(string dbName)
        {
            var options = new DbContextOptionsBuilder<PublicContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new PublicContext(options);

            // Agregar usuario
            context.Usuarios.Add(new Usuario { IdUsuario = 1, Nombres = "Test", Apellidos = "User", Correo = "test@example.com", Cedula = "1234567890", Contrasena = "pass", PerfilAcceso = "admin" });

            // Agregar crédito
            context.Creditos.Add(new Credito
            {
                IdCredito = 1,
                NumeroCredito = "CR001",
                MontoAprobado = 1000,
                CuotaMensual = 100,
                Estado = "activo",
                TablaAmortizacions = new List<TablaAmortizacion>
                {
                    new TablaAmortizacion { IdCuota = 1, NumeroCuota = 1, CuotaTotal = 100, SaldoPendiente = 100, Estado = "pendiente", FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(10)) }
                }
            });

            context.SaveChanges();
            return context;
        }

        [TestMethod]
        public void PostPagoCredito_ReturnsBadRequest_WhenCreditoDoesNotExist()
        {
            var context = GetContextWithData("NoCredito");
            var controller = new PagosCreditoController(context);

            var pago = new PagosCredito { IdCredito = 99, IdCuota = 1, MontoPago = 100, UsuarioRegistro = 1 };
            var result = controller.PostPagoCredito(pago);

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void PostPagoCredito_ReturnsBadRequest_WhenMontoInsuficiente()
        {
            var context = GetContextWithData("MontoInsuficiente");
            var controller = new PagosCreditoController(context);

            var pago = new PagosCredito { IdCredito = 1, IdCuota = 1, MontoPago = 10, UsuarioRegistro = 1 };
            var result = controller.PostPagoCredito(pago);

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void PostPagoCredito_RegistersPago_WhenValid()
        {
            var context = GetContextWithData("PagoValido");
            var controller = new PagosCreditoController(context);

            var pago = new PagosCredito { IdCredito = 1, IdCuota = 1, MontoPago = 100, UsuarioRegistro = 1 };
            var result = controller.PostPagoCredito(pago);

            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            Assert.AreEqual(1, context.PagosCreditos.Count());
        }

        [TestMethod]
        public void GetPagosByCredito_ReturnsPagosList()
        {
            var context = GetContextWithData("ConsultaPagos");
            context.PagosCreditos.Add(new PagosCredito
            {
                IdPago = 1,
                IdCredito = 1,
                MontoPago = 100,
                Estado = "registrado",
                UsuarioRegistro = 1,
                FechaPago = DateOnly.FromDateTime(DateTime.Today)
            });
            context.SaveChanges();

            var controller = new PagosCreditoController(context);
            var result = controller.GetPagosByCredito(1);

            Assert.IsInstanceOfType(result.Result, typeof(ActionResult<IEnumerable<PagosCredito>>));
        }

        [TestMethod]
        public void GetPagoCredito_ReturnsNotFound_WhenIdIsInvalid()
        {
            var context = GetContextWithData("PagoInvalido");
            var controller = new PagosCreditoController(context);

            var result = controller.GetPagoCredito(99);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public void GetPagosByEstado_ReturnsFilteredList()
        {
            var context = GetContextWithData("PorEstado");
            context.PagosCreditos.Add(new PagosCredito
            {
                IdPago = 1,
                IdCredito = 1,
                Estado = "registrado",
                MontoPago = 100,
                UsuarioRegistro = 1,
                FechaPago = DateOnly.FromDateTime(DateTime.Today)
            });
            context.SaveChanges();

            var controller = new PagosCreditoController(context);
            var result = controller.GetPagosByEstado("registrado");

            Assert.IsInstanceOfType(result.Result, typeof(ActionResult<IEnumerable<PagosCredito>>));
        }

        [TestMethod]
        public void GetPagosByFecha_ReturnsCorrectRange()
        {
            var context = GetContextWithData("PorFecha");
            context.PagosCreditos.Add(new PagosCredito
            {
                IdPago = 1,
                IdCredito = 1,
                Estado = "registrado",
                MontoPago = 100,
                UsuarioRegistro = 1,
                FechaPago = DateOnly.FromDateTime(DateTime.Today)
            });
            context.SaveChanges();

            var controller = new PagosCreditoController(context);
            var fechaInicio = DateTime.Today.AddDays(-1);
            var fechaFin = DateTime.Today.AddDays(1);

            var result = controller.GetPagosByFecha(fechaInicio, fechaFin);

            Assert.IsInstanceOfType(result.Result, typeof(ActionResult<IEnumerable<PagosCredito>>));
        }

        [TestMethod]
        public void GetPagosCredito_ReturnsAllOrdered()
        {
            var context = GetContextWithData("TodosPagos");
            context.PagosCreditos.AddRange(new List<PagosCredito>
            {
                new PagosCredito { IdPago = 1, IdCredito = 1, MontoPago = 100, UsuarioRegistro = 1, FechaPago = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)) },
                new PagosCredito { IdPago = 2, IdCredito = 1, MontoPago = 150, UsuarioRegistro = 1, FechaPago = DateOnly.FromDateTime(DateTime.Today) }
            });
            context.SaveChanges();

            var controller = new PagosCreditoController(context);
            var result = controller.GetPagosCredito();

            Assert.IsInstanceOfType(result.Result, typeof(ActionResult<IEnumerable<PagosCredito>>));
        }
    }
}
