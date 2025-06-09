using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class Aportacione
{
    public int IdAportacion { get; set; }

    public int IdSocio { get; set; }

    public int IdTipoAportacion { get; set; }

    public string? NumeroComprobante { get; set; }

    public decimal Monto { get; set; }

    public DateOnly FechaAportacion { get; set; }

    public string? Motivo { get; set; }

    public string? MetodoPago { get; set; }

    public string? Estado { get; set; }

    public int UsuarioRegistro { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public string? Observaciones { get; set; }

    public virtual Socio IdSocioNavigation { get; set; } = null!;

    public virtual TiposAportacion IdTipoAportacionNavigation { get; set; } = null!;

    public virtual Usuario UsuarioRegistroNavigation { get; set; } = null!;
}
