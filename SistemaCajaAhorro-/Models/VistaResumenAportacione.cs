using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class VistaResumenAportacione
{
    public int IdSocio { get; set; }

    public string NumeroSocio { get; set; } = null!;

    public string NombreSocio { get; set; } = null!;

    public string? TipoAportacion { get; set; }

    public bool? EsObligatoria { get; set; }

    public int? TotalAportaciones { get; set; }

    public decimal? MontoTotalAportado { get; set; }

    public DateOnly? UltimaAportacion { get; set; }

    public DateOnly? PrimeraAportacion { get; set; }
}
