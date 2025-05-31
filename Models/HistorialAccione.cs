using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class HistorialAccione
{
    public int IdHistorial { get; set; }

    public int IdUsuario { get; set; }

    public string Accion { get; set; } = null!;

    public string TablaAfectada { get; set; } = null!;

    public int? IdRegistroAfectado { get; set; }

    public string? ValoresAnteriores { get; set; }

    public string? ValoresNuevos { get; set; }

    public string? IpUsuario { get; set; }

    public string? UserAgent { get; set; }

    public DateTime? FechaAccion { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
