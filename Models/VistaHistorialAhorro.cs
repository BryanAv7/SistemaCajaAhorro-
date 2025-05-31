using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class VistaHistorialAhorro
{
    public int IdSocio { get; set; }

    public string NumeroSocio { get; set; } = null!;

    public string NombreCompleto { get; set; } = null!;

    public string NumeroCuenta { get; set; } = null!;

    public string TipoCuenta { get; set; } = null!;

    public string TipoMovimiento { get; set; } = null!;

    public decimal Monto { get; set; }

    public decimal SaldoAnterior { get; set; }

    public decimal SaldoNuevo { get; set; }

    public string? Descripcion { get; set; }

    public DateTime? FechaMovimiento { get; set; }

    public string? NumeroComprobante { get; set; }
}
