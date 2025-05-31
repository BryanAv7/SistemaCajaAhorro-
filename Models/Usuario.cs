using System;
using System.Collections.Generic;

namespace SistemaCajaAhorro.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Cedula { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string? Direccion { get; set; }

    public string Correo { get; set; } = null!;

    public DateOnly? FechaNacimiento { get; set; }

    public string? Ciudad { get; set; }

    public string Contrasena { get; set; } = null!;

    public string PerfilAcceso { get; set; } = null!;

    public bool? Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? UltimoAcceso { get; set; }

    public virtual ICollection<Aportacione> Aportaciones { get; set; } = new List<Aportacione>();

    public virtual ICollection<Credito> Creditos { get; set; } = new List<Credito>();

    public virtual ICollection<HistorialAccione> HistorialAcciones { get; set; } = new List<HistorialAccione>();

    public virtual ICollection<LibroDiario> LibroDiarios { get; set; } = new List<LibroDiario>();

    public virtual ICollection<MovimientosAhorro> MovimientosAhorros { get; set; } = new List<MovimientosAhorro>();

    public virtual ICollection<PagosCredito> PagosCreditos { get; set; } = new List<PagosCredito>();

    public virtual ICollection<ParametrosGenerale> ParametrosGenerales { get; set; } = new List<ParametrosGenerale>();

    public virtual ICollection<SesionesUsuario> SesionesUsuarios { get; set; } = new List<SesionesUsuario>();

    public virtual ICollection<SolicitudesCredito> SolicitudesCreditos { get; set; } = new List<SolicitudesCredito>();
}
