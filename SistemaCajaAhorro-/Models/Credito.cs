using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class Credito
{
    public int IdCredito { get; set; }

    public int IdSolicitud { get; set; }

    public int IdSocio { get; set; }

    public string NumeroCredito { get; set; } = null!;

    public decimal MontoAprobado { get; set; }

    public decimal TasaInteres { get; set; }

    public int PlazoMeses { get; set; }

    public decimal CuotaMensual { get; set; }

    public decimal SaldoCapital { get; set; }

    public DateOnly FechaAprobacion { get; set; }

    public DateOnly? FechaDesembolso { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public string? Estado { get; set; }

    public int UsuarioAprobacion { get; set; }

    public virtual Socio IdSocioNavigation { get; set; } = null!;

    public virtual SolicitudesCredito IdSolicitudNavigation { get; set; } = null!;

    public virtual ICollection<PagosCredito> PagosCreditos { get; set; } = new List<PagosCredito>();

    public virtual ICollection<TablaAmortizacion> TablaAmortizacions { get; set; } = new List<TablaAmortizacion>();

    public virtual Usuario UsuarioAprobacionNavigation { get; set; } = null!;
}
