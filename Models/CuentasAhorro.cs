using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class CuentasAhorro
{
    public int IdCuentaAhorro { get; set; }

    public int IdSocio { get; set; }

    public string NumeroCuenta { get; set; } = null!;

    public string TipoCuenta { get; set; } = null!;

    public decimal? SaldoActual { get; set; }

    public decimal? TasaInteres { get; set; }

    public DateOnly FechaApertura { get; set; }

    public string? Estado { get; set; }

    public decimal? MontoMinimo { get; set; }

    public DateTime? FechaUltimoMovimiento { get; set; }

    public virtual Socio IdSocioNavigation { get; set; } = null!;

    public virtual ICollection<MovimientosAhorro> MovimientosAhorros { get; set; } = new List<MovimientosAhorro>();
}
