using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class ParametrosGenerale
{
    public int IdParametro { get; set; }

    public string NombreParametro { get; set; } = null!;

    public string ValorParametro { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? TipoDato { get; set; }

    public string? Modulo { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public int? UsuarioActualizacion { get; set; }

    public virtual Usuario? UsuarioActualizacionNavigation { get; set; }
}
