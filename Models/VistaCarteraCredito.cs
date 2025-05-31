using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class VistaCarteraCredito
{
    public int IdCredito { get; set; }

    public string NumeroCredito { get; set; } = null!;

    public string NumeroSocio { get; set; } = null!;

    public string NombreSocio { get; set; } = null!;

    public decimal MontoAprobado { get; set; }

    public decimal SaldoCapital { get; set; }

    public decimal TasaInteres { get; set; }

    public decimal CuotaMensual { get; set; }

    public DateOnly? FechaDesembolso { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public string? Estado { get; set; }

    public int? DiasVencimiento { get; set; }

    public string EstadoCartera { get; set; } = null!;
}
