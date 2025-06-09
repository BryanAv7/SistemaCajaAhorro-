using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class PagosCredito
{
    public int IdPago { get; set; }

    public int IdCredito { get; set; }

    public int? IdCuota { get; set; }

    public string Estado { get; set; } = null!;

    public decimal MontoPago { get; set; }

    public decimal MontoCapital { get; set; }

    public decimal MontoInteres { get; set; }

    public decimal? MontoMora { get; set; }

    public DateOnly FechaPago { get; set; }

    public string? MetodoPago { get; set; }

    public string? NumeroComprobante { get; set; }

    public int UsuarioRegistro { get; set; }

    public string? Observaciones { get; set; }

    public virtual Credito IdCreditoNavigation { get; set; } = null!;

    public virtual TablaAmortizacion? IdCuotaNavigation { get; set; }

    public virtual Usuario UsuarioRegistroNavigation { get; set; } = null!;
}
