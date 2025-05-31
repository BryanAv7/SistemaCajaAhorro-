using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class TiposAportacion
{
    public int IdTipoAportacion { get; set; }

    public string NombreTipo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal? MontoMinimo { get; set; }

    public bool? EsObligatoria { get; set; }

    public string? Frecuencia { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<Aportacione> Aportaciones { get; set; } = new List<Aportacione>();
}
