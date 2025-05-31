using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class SolicitudesCredito
{
    public int IdSolicitud { get; set; }

    public int IdSocio { get; set; }

    public decimal MontoSolicitado { get; set; }

    public int PlazoMeses { get; set; }

    public decimal TasaInteres { get; set; }

    public string DestinoCredito { get; set; } = null!;

    public decimal? IngresosMensuales { get; set; }

    public decimal? GastosMensuales { get; set; }

    public DateOnly FechaSolicitud { get; set; }

    public string? Estado { get; set; }

    public string? MotivoRechazo { get; set; }

    public int? UsuarioEvaluador { get; set; }

    public DateOnly? FechaEvaluacion { get; set; }

    public string? Observaciones { get; set; }

    public virtual ICollection<Credito> Creditos { get; set; } = new List<Credito>();

    public virtual Socio IdSocioNavigation { get; set; } = null!;

    public virtual Usuario? UsuarioEvaluadorNavigation { get; set; }
}
