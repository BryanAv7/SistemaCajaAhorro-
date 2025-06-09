using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class LibroDiario
{
    public int IdAsiento { get; set; }

    public string NumeroAsiento { get; set; } = null!;

    public DateOnly FechaAsiento { get; set; }

    public string Concepto { get; set; } = null!;

    public decimal TotalDebe { get; set; }

    public decimal TotalHaber { get; set; }

    public int UsuarioRegistro { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public string? Estado { get; set; }

    public virtual ICollection<DetalleLibroDiario> DetalleLibroDiarios { get; set; } = new List<DetalleLibroDiario>();

    public virtual Usuario UsuarioRegistroNavigation { get; set; } = null!;
}
