using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class MovimientosAhorro
{
    public int IdMovimiento { get; set; }

    public int IdCuentaAhorro { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public decimal Monto { get; set; }

    public decimal SaldoAnterior { get; set; }

    public decimal SaldoNuevo { get; set; }

    public string? Descripcion { get; set; }

    public DateTime? FechaMovimiento { get; set; }

    public string? NumeroComprobante { get; set; }

    public int UsuarioRegistro { get; set; }

    public int? IdCuentaContable { get; set; }

    public virtual CuentasAhorro IdCuentaAhorroNavigation { get; set; } = null!;

    public virtual PlanCuenta? IdCuentaContableNavigation { get; set; }

    public virtual Usuario UsuarioRegistroNavigation { get; set; } = null!;
}
