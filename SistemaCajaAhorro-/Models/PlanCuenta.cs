using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class PlanCuenta
{
    public int IdCuenta { get; set; }

    public string CodigoCuenta { get; set; } = null!;

    public string NombreCategoria { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string TipoCuenta { get; set; } = null!;

    public int? CuentaPadre { get; set; }

    public int? Nivel { get; set; }

    public decimal? ValorMonetario { get; set; }

    public bool? Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual PlanCuenta? CuentaPadreNavigation { get; set; }

    public virtual ICollection<DetalleLibroDiario> DetalleLibroDiarios { get; set; } = new List<DetalleLibroDiario>();

    public virtual ICollection<PlanCuenta> InverseCuentaPadreNavigation { get; set; } = new List<PlanCuenta>();

    public virtual ICollection<MovimientosAhorro> MovimientosAhorros { get; set; } = new List<MovimientosAhorro>();
}
