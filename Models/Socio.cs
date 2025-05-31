using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class Socio
{
    public int IdSocio { get; set; }

    public string NumeroSocio { get; set; } = null!;

    public string Cedula { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public string? Ciudad { get; set; }

    public DateOnly FechaIngreso { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public string? Observaciones { get; set; }

    public virtual ICollection<Aportacione> Aportaciones { get; set; } = new List<Aportacione>();

    public virtual ICollection<Credito> Creditos { get; set; } = new List<Credito>();

    public virtual ICollection<CuentasAhorro> CuentasAhorros { get; set; } = new List<CuentasAhorro>();

    public virtual ICollection<SolicitudesCredito> SolicitudesCreditos { get; set; } = new List<SolicitudesCredito>();
}
