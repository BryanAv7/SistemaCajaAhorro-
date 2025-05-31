using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class DetalleLibroDiario
{
    public int IdDetalle { get; set; }

    public int IdAsiento { get; set; }

    public int IdCuentaContable { get; set; }

    public string? Concepto { get; set; }

    public decimal? Debe { get; set; }

    public decimal? Haber { get; set; }

    public int? OrdenLinea { get; set; }

    public virtual LibroDiario IdAsientoNavigation { get; set; } = null!;

    public virtual PlanCuenta IdCuentaContableNavigation { get; set; } = null!;
}
