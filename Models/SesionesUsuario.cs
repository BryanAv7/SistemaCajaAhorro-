using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class SesionesUsuario
{
    public int IdSesion { get; set; }

    public int IdUsuario { get; set; }

    public string TokenSesion { get; set; } = null!;

    public string? IpAcceso { get; set; }

    public string? UserAgent { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaUltimoAcceso { get; set; }

    public string? Estado { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
