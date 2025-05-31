using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class TablaAmortizacion
{
    public int IdCuota { get; set; }

    public int IdCredito { get; set; }

    public int NumeroCuota { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public decimal CuotaCapital { get; set; }

    public decimal CuotaInteres { get; set; }

    public decimal CuotaTotal { get; set; }

    public decimal SaldoPendiente { get; set; }

    public string? Estado { get; set; }

    public virtual Credito IdCreditoNavigation { get; set; } = null!;

    public virtual ICollection<PagosCredito> PagosCreditos { get; set; } = new List<PagosCredito>();
}
