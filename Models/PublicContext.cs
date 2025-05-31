using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SistemaCajaAhorro.Models;

public partial class PublicContext : DbContext
{
    public PublicContext()
    {
    }

    public PublicContext(DbContextOptions<PublicContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aportacione> Aportaciones { get; set; }

    public virtual DbSet<Credito> Creditos { get; set; }

    public virtual DbSet<CuentasAhorro> CuentasAhorros { get; set; }

    public virtual DbSet<DetalleLibroDiario> DetalleLibroDiarios { get; set; }

    public virtual DbSet<HistorialAccione> HistorialAcciones { get; set; }

    public virtual DbSet<LibroDiario> LibroDiarios { get; set; }

    public virtual DbSet<MovimientosAhorro> MovimientosAhorros { get; set; }

    public virtual DbSet<PagosCredito> PagosCreditos { get; set; }

    public virtual DbSet<ParametrosGenerale> ParametrosGenerales { get; set; }

    public virtual DbSet<PlanCuenta> PlanCuentas { get; set; }

    public virtual DbSet<SesionesUsuario> SesionesUsuarios { get; set; }

    public virtual DbSet<Socio> Socios { get; set; }

    public virtual DbSet<SolicitudesCredito> SolicitudesCreditos { get; set; }

    public virtual DbSet<TablaAmortizacion> TablaAmortizacions { get; set; }

    public virtual DbSet<TiposAportacion> TiposAportacions { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<VistaCarteraCredito> VistaCarteraCreditos { get; set; }

    public virtual DbSet<VistaHistorialAhorro> VistaHistorialAhorros { get; set; }

    public virtual DbSet<VistaResumenAportacione> VistaResumenAportaciones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=public;Trusted_Connection=true;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aportacione>(entity =>
        {
            entity.HasKey(e => e.IdAportacion).HasName("PK__aportaci__837B0AE1B6E620C7");

            entity.ToTable("aportaciones");

            entity.HasIndex(e => e.Estado, "IX_aportaciones_estado");

            entity.HasIndex(e => e.FechaAportacion, "IX_aportaciones_fecha");

            entity.HasIndex(e => e.IdSocio, "IX_aportaciones_socio");

            entity.HasIndex(e => e.NumeroComprobante, "UQ__aportaci__1850D80DBC75C48F").IsUnique();

            entity.Property(e => e.IdAportacion).HasColumnName("id_aportacion");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("registrada")
                .HasColumnName("estado");
            entity.Property(e => e.FechaAportacion).HasColumnName("fecha_aportacion");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdSocio).HasColumnName("id_socio");
            entity.Property(e => e.IdTipoAportacion).HasColumnName("id_tipo_aportacion");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(20)
                .HasDefaultValue("efectivo")
                .HasColumnName("metodo_pago");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.Motivo).HasColumnName("motivo");
            entity.Property(e => e.NumeroComprobante)
                .HasMaxLength(50)
                .HasColumnName("numero_comprobante");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.UsuarioRegistro).HasColumnName("usuario_registro");

            entity.HasOne(d => d.IdSocioNavigation).WithMany(p => p.Aportaciones)
                .HasForeignKey(d => d.IdSocio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_aportaciones_socio");

            entity.HasOne(d => d.IdTipoAportacionNavigation).WithMany(p => p.Aportaciones)
                .HasForeignKey(d => d.IdTipoAportacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_aportaciones_tipo");

            entity.HasOne(d => d.UsuarioRegistroNavigation).WithMany(p => p.Aportaciones)
                .HasForeignKey(d => d.UsuarioRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_aportaciones_usuario");
        });

        modelBuilder.Entity<Credito>(entity =>
        {
            entity.HasKey(e => e.IdCredito).HasName("PK__creditos__7A60C097B9DFD6B4");

            entity.ToTable("creditos");

            entity.HasIndex(e => e.Estado, "IX_creditos_estado");

            entity.HasIndex(e => e.NumeroCredito, "IX_creditos_numero");

            entity.HasIndex(e => e.IdSocio, "IX_creditos_socio");

            entity.HasIndex(e => e.NumeroCredito, "UQ__creditos__542BD6212A0E051B").IsUnique();

            entity.Property(e => e.IdCredito).HasColumnName("id_credito");
            entity.Property(e => e.CuotaMensual)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("cuota_mensual");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("aprobado")
                .HasColumnName("estado");
            entity.Property(e => e.FechaAprobacion).HasColumnName("fecha_aprobacion");
            entity.Property(e => e.FechaDesembolso).HasColumnName("fecha_desembolso");
            entity.Property(e => e.FechaVencimiento).HasColumnName("fecha_vencimiento");
            entity.Property(e => e.IdSocio).HasColumnName("id_socio");
            entity.Property(e => e.IdSolicitud).HasColumnName("id_solicitud");
            entity.Property(e => e.MontoAprobado)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("monto_aprobado");
            entity.Property(e => e.NumeroCredito)
                .HasMaxLength(30)
                .HasColumnName("numero_credito");
            entity.Property(e => e.PlazoMeses).HasColumnName("plazo_meses");
            entity.Property(e => e.SaldoCapital)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("saldo_capital");
            entity.Property(e => e.TasaInteres)
                .HasColumnType("decimal(5, 4)")
                .HasColumnName("tasa_interes");
            entity.Property(e => e.UsuarioAprobacion).HasColumnName("usuario_aprobacion");

            entity.HasOne(d => d.IdSocioNavigation).WithMany(p => p.Creditos)
                .HasForeignKey(d => d.IdSocio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_creditos_socio");

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.Creditos)
                .HasForeignKey(d => d.IdSolicitud)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_creditos_solicitud");

            entity.HasOne(d => d.UsuarioAprobacionNavigation).WithMany(p => p.Creditos)
                .HasForeignKey(d => d.UsuarioAprobacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_creditos_usuario");
        });

        modelBuilder.Entity<CuentasAhorro>(entity =>
        {
            entity.HasKey(e => e.IdCuentaAhorro).HasName("PK__cuentas___5BB6F3629C431D13");

            entity.ToTable("cuentas_ahorro");

            entity.HasIndex(e => e.Estado, "IX_cuentas_estado");

            entity.HasIndex(e => e.NumeroCuenta, "IX_cuentas_numero");

            entity.HasIndex(e => e.IdSocio, "IX_cuentas_socio");

            entity.HasIndex(e => e.NumeroCuenta, "UQ__cuentas___C6B74B885B7149A5").IsUnique();

            entity.Property(e => e.IdCuentaAhorro).HasColumnName("id_cuenta_ahorro");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("activa")
                .HasColumnName("estado");
            entity.Property(e => e.FechaApertura).HasColumnName("fecha_apertura");
            entity.Property(e => e.FechaUltimoMovimiento).HasColumnName("fecha_ultimo_movimiento");
            entity.Property(e => e.IdSocio).HasColumnName("id_socio");
            entity.Property(e => e.MontoMinimo)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("monto_minimo");
            entity.Property(e => e.NumeroCuenta)
                .HasMaxLength(30)
                .HasColumnName("numero_cuenta");
            entity.Property(e => e.SaldoActual)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("saldo_actual");
            entity.Property(e => e.TasaInteres)
                .HasDefaultValue(0.0000m)
                .HasColumnType("decimal(5, 4)")
                .HasColumnName("tasa_interes");
            entity.Property(e => e.TipoCuenta)
                .HasMaxLength(30)
                .HasColumnName("tipo_cuenta");

            entity.HasOne(d => d.IdSocioNavigation).WithMany(p => p.CuentasAhorros)
                .HasForeignKey(d => d.IdSocio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_cuentas_socio");
        });

        modelBuilder.Entity<DetalleLibroDiario>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__detalle___4F1332DEED2A33A1");

            entity.ToTable("detalle_libro_diario");

            entity.HasIndex(e => e.IdAsiento, "IX_detalle_asiento");

            entity.HasIndex(e => e.IdCuentaContable, "IX_detalle_cuenta");

            entity.Property(e => e.IdDetalle).HasColumnName("id_detalle");
            entity.Property(e => e.Concepto).HasColumnName("concepto");
            entity.Property(e => e.Debe)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("debe");
            entity.Property(e => e.Haber)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("haber");
            entity.Property(e => e.IdAsiento).HasColumnName("id_asiento");
            entity.Property(e => e.IdCuentaContable).HasColumnName("id_cuenta_contable");
            entity.Property(e => e.OrdenLinea)
                .HasDefaultValue(1)
                .HasColumnName("orden_linea");

            entity.HasOne(d => d.IdAsientoNavigation).WithMany(p => p.DetalleLibroDiarios)
                .HasForeignKey(d => d.IdAsiento)
                .HasConstraintName("FK_detalle_asiento");

            entity.HasOne(d => d.IdCuentaContableNavigation).WithMany(p => p.DetalleLibroDiarios)
                .HasForeignKey(d => d.IdCuentaContable)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_detalle_cuenta");
        });

        modelBuilder.Entity<HistorialAccione>(entity =>
        {
            entity.HasKey(e => e.IdHistorial).HasName("PK__historia__76E6C50207354288");

            entity.ToTable("historial_acciones");

            entity.HasIndex(e => e.FechaAccion, "IX_historial_fecha");

            entity.HasIndex(e => e.TablaAfectada, "IX_historial_tabla");

            entity.HasIndex(e => e.IdUsuario, "IX_historial_usuario");

            entity.Property(e => e.IdHistorial).HasColumnName("id_historial");
            entity.Property(e => e.Accion)
                .HasMaxLength(100)
                .HasColumnName("accion");
            entity.Property(e => e.FechaAccion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_accion");
            entity.Property(e => e.IdRegistroAfectado).HasColumnName("id_registro_afectado");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IpUsuario)
                .HasMaxLength(45)
                .HasColumnName("ip_usuario");
            entity.Property(e => e.TablaAfectada)
                .HasMaxLength(50)
                .HasColumnName("tabla_afectada");
            entity.Property(e => e.UserAgent).HasColumnName("user_agent");
            entity.Property(e => e.ValoresAnteriores).HasColumnName("valores_anteriores");
            entity.Property(e => e.ValoresNuevos).HasColumnName("valores_nuevos");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.HistorialAcciones)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_historial_usuario");
        });

        modelBuilder.Entity<LibroDiario>(entity =>
        {
            entity.HasKey(e => e.IdAsiento).HasName("PK__libro_di__14864B6758AA7DAB");

            entity.ToTable("libro_diario");

            entity.HasIndex(e => e.FechaAsiento, "IX_libro_fecha");

            entity.HasIndex(e => e.NumeroAsiento, "IX_libro_numero");

            entity.HasIndex(e => e.NumeroAsiento, "UQ__libro_di__58AA2A7F9876B044").IsUnique();

            entity.Property(e => e.IdAsiento).HasColumnName("id_asiento");
            entity.Property(e => e.Concepto).HasColumnName("concepto");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("confirmado")
                .HasColumnName("estado");
            entity.Property(e => e.FechaAsiento).HasColumnName("fecha_asiento");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.NumeroAsiento)
                .HasMaxLength(20)
                .HasColumnName("numero_asiento");
            entity.Property(e => e.TotalDebe)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("total_debe");
            entity.Property(e => e.TotalHaber)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("total_haber");
            entity.Property(e => e.UsuarioRegistro).HasColumnName("usuario_registro");

            entity.HasOne(d => d.UsuarioRegistroNavigation).WithMany(p => p.LibroDiarios)
                .HasForeignKey(d => d.UsuarioRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_libro_usuario");
        });

        modelBuilder.Entity<MovimientosAhorro>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento).HasName("PK__movimien__2A071C240A74FF19");

            entity.ToTable("movimientos_ahorro", tb =>
                {
                    tb.HasTrigger("tr_actualizar_saldo_ahorro");
                    tb.HasTrigger("tr_registrar_movimiento_contable");
                });

            entity.HasIndex(e => e.IdCuentaAhorro, "IX_movimientos_cuenta");

            entity.HasIndex(e => e.FechaMovimiento, "IX_movimientos_fecha");

            entity.HasIndex(e => e.TipoMovimiento, "IX_movimientos_tipo");

            entity.Property(e => e.IdMovimiento).HasColumnName("id_movimiento");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.FechaMovimiento)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_movimiento");
            entity.Property(e => e.IdCuentaAhorro).HasColumnName("id_cuenta_ahorro");
            entity.Property(e => e.IdCuentaContable).HasColumnName("id_cuenta_contable");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.NumeroComprobante)
                .HasMaxLength(50)
                .HasColumnName("numero_comprobante");
            entity.Property(e => e.SaldoAnterior)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("saldo_anterior");
            entity.Property(e => e.SaldoNuevo)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("saldo_nuevo");
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(20)
                .HasColumnName("tipo_movimiento");
            entity.Property(e => e.UsuarioRegistro).HasColumnName("usuario_registro");

            entity.HasOne(d => d.IdCuentaAhorroNavigation).WithMany(p => p.MovimientosAhorros)
                .HasForeignKey(d => d.IdCuentaAhorro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_movimientos_cuenta");

            entity.HasOne(d => d.IdCuentaContableNavigation).WithMany(p => p.MovimientosAhorros)
                .HasForeignKey(d => d.IdCuentaContable)
                .HasConstraintName("FK_movimientos_cuenta_contable");

            entity.HasOne(d => d.UsuarioRegistroNavigation).WithMany(p => p.MovimientosAhorros)
                .HasForeignKey(d => d.UsuarioRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_movimientos_usuario");
        });

        modelBuilder.Entity<PagosCredito>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK__pagos_cr__0941B074CD3AFC5D");

            entity.ToTable("pagos_credito");

            entity.HasIndex(e => e.IdCredito, "IX_pagos_credito");

            entity.HasIndex(e => e.FechaPago, "IX_pagos_fecha");

            entity.Property(e => e.IdPago).HasColumnName("id_pago");
            entity.Property(e => e.FechaPago).HasColumnName("fecha_pago");
            entity.Property(e => e.IdCredito).HasColumnName("id_credito");
            entity.Property(e => e.IdCuota).HasColumnName("id_cuota");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(20)
                .HasDefaultValue("efectivo")
                .HasColumnName("metodo_pago");
            entity.Property(e => e.MontoCapital)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("monto_capital");
            entity.Property(e => e.MontoInteres)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("monto_interes");
            entity.Property(e => e.MontoMora)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("monto_mora");
            entity.Property(e => e.MontoPago)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("monto_pago");
            entity.Property(e => e.NumeroComprobante)
                .HasMaxLength(50)
                .HasColumnName("numero_comprobante");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.UsuarioRegistro).HasColumnName("usuario_registro");

            entity.HasOne(d => d.IdCreditoNavigation).WithMany(p => p.PagosCreditos)
                .HasForeignKey(d => d.IdCredito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pagos_credito");

            entity.HasOne(d => d.IdCuotaNavigation).WithMany(p => p.PagosCreditos)
                .HasForeignKey(d => d.IdCuota)
                .HasConstraintName("FK_pagos_cuota");

            entity.HasOne(d => d.UsuarioRegistroNavigation).WithMany(p => p.PagosCreditos)
                .HasForeignKey(d => d.UsuarioRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pagos_usuario");
        });

        modelBuilder.Entity<ParametrosGenerale>(entity =>
        {
            entity.HasKey(e => e.IdParametro).HasName("PK__parametr__3D24E325D48334E6");

            entity.ToTable("parametros_generales");

            entity.HasIndex(e => e.Modulo, "IX_parametros_modulo");

            entity.HasIndex(e => e.NombreParametro, "UQ__parametr__6F443E34462933CB").IsUnique();

            entity.Property(e => e.IdParametro).HasColumnName("id_parametro");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_actualizacion");
            entity.Property(e => e.Modulo)
                .HasMaxLength(50)
                .HasColumnName("modulo");
            entity.Property(e => e.NombreParametro)
                .HasMaxLength(100)
                .HasColumnName("nombre_parametro");
            entity.Property(e => e.TipoDato)
                .HasMaxLength(20)
                .HasDefaultValue("string")
                .HasColumnName("tipo_dato");
            entity.Property(e => e.UsuarioActualizacion).HasColumnName("usuario_actualizacion");
            entity.Property(e => e.ValorParametro).HasColumnName("valor_parametro");

            entity.HasOne(d => d.UsuarioActualizacionNavigation).WithMany(p => p.ParametrosGenerales)
                .HasForeignKey(d => d.UsuarioActualizacion)
                .HasConstraintName("FK_parametros_usuario");
        });

        modelBuilder.Entity<PlanCuenta>(entity =>
        {
            entity.HasKey(e => e.IdCuenta).HasName("PK__plan_cue__C7E286853CDDFF68");

            entity.ToTable("plan_cuentas");

            entity.HasIndex(e => e.CodigoCuenta, "IX_plan_cuentas_codigo");

            entity.HasIndex(e => e.TipoCuenta, "IX_plan_cuentas_tipo");

            entity.HasIndex(e => e.CodigoCuenta, "UQ__plan_cue__B4E54EABF529C6F7").IsUnique();

            entity.Property(e => e.IdCuenta).HasColumnName("id_cuenta");
            entity.Property(e => e.CodigoCuenta)
                .HasMaxLength(20)
                .HasColumnName("codigo_cuenta");
            entity.Property(e => e.CuentaPadre).HasColumnName("cuenta_padre");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.Nivel)
                .HasDefaultValue(1)
                .HasColumnName("nivel");
            entity.Property(e => e.NombreCategoria)
                .HasMaxLength(150)
                .HasColumnName("nombre_categoria");
            entity.Property(e => e.TipoCuenta)
                .HasMaxLength(20)
                .HasColumnName("tipo_cuenta");
            entity.Property(e => e.ValorMonetario)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("valor_monetario");

            entity.HasOne(d => d.CuentaPadreNavigation).WithMany(p => p.InverseCuentaPadreNavigation)
                .HasForeignKey(d => d.CuentaPadre)
                .HasConstraintName("FK_plan_cuentas_padre");
        });

        modelBuilder.Entity<SesionesUsuario>(entity =>
        {
            entity.HasKey(e => e.IdSesion).HasName("PK__sesiones__8D3F9DFE19A6DC00");

            entity.ToTable("sesiones_usuario");

            entity.HasIndex(e => e.TokenSesion, "IX_sesiones_token");

            entity.HasIndex(e => e.IdUsuario, "IX_sesiones_usuario");

            entity.HasIndex(e => e.TokenSesion, "UQ__sesiones__F572AF3A6A9CF768").IsUnique();

            entity.Property(e => e.IdSesion).HasColumnName("id_sesion");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("activa")
                .HasColumnName("estado");
            entity.Property(e => e.FechaInicio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_inicio");
            entity.Property(e => e.FechaUltimoAcceso)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_ultimo_acceso");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IpAcceso)
                .HasMaxLength(45)
                .HasColumnName("ip_acceso");
            entity.Property(e => e.TokenSesion)
                .HasMaxLength(255)
                .HasColumnName("token_sesion");
            entity.Property(e => e.UserAgent).HasColumnName("user_agent");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.SesionesUsuarios)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_sesiones_usuario");
        });

        modelBuilder.Entity<Socio>(entity =>
        {
            entity.HasKey(e => e.IdSocio).HasName("PK__socios__3718538331EFF0DE");

            entity.ToTable("socios");

            entity.HasIndex(e => e.Cedula, "IX_socios_cedula");

            entity.HasIndex(e => e.Estado, "IX_socios_estado");

            entity.HasIndex(e => e.NumeroSocio, "IX_socios_numero");

            entity.HasIndex(e => e.Correo, "UQ__socios__2A586E0B6BDA498C").IsUnique();

            entity.HasIndex(e => e.Cedula, "UQ__socios__415B7BE5F05DF3AC").IsUnique();

            entity.HasIndex(e => e.NumeroSocio, "UQ__socios__9E02E04D51296723").IsUnique();

            entity.Property(e => e.IdSocio).HasColumnName("id_socio");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .HasColumnName("apellidos");
            entity.Property(e => e.Cedula)
                .HasMaxLength(20)
                .HasColumnName("cedula");
            entity.Property(e => e.Ciudad)
                .HasMaxLength(100)
                .HasColumnName("ciudad");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(200)
                .HasColumnName("direccion");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("activo")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaIngreso).HasColumnName("fecha_ingreso");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.Nombres)
                .HasMaxLength(100)
                .HasColumnName("nombres");
            entity.Property(e => e.NumeroSocio)
                .HasMaxLength(20)
                .HasColumnName("numero_socio");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<SolicitudesCredito>(entity =>
        {
            entity.HasKey(e => e.IdSolicitud).HasName("PK__solicitu__5C0C31F37C58CA3C");

            entity.ToTable("solicitudes_credito");

            entity.HasIndex(e => e.Estado, "IX_solicitudes_estado");

            entity.HasIndex(e => e.FechaSolicitud, "IX_solicitudes_fecha");

            entity.HasIndex(e => e.IdSocio, "IX_solicitudes_socio");

            entity.Property(e => e.IdSolicitud).HasColumnName("id_solicitud");
            entity.Property(e => e.DestinoCredito).HasColumnName("destino_credito");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("pendiente")
                .HasColumnName("estado");
            entity.Property(e => e.FechaEvaluacion).HasColumnName("fecha_evaluacion");
            entity.Property(e => e.FechaSolicitud).HasColumnName("fecha_solicitud");
            entity.Property(e => e.GastosMensuales)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("gastos_mensuales");
            entity.Property(e => e.IdSocio).HasColumnName("id_socio");
            entity.Property(e => e.IngresosMensuales)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("ingresos_mensuales");
            entity.Property(e => e.MontoSolicitado)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("monto_solicitado");
            entity.Property(e => e.MotivoRechazo).HasColumnName("motivo_rechazo");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.PlazoMeses).HasColumnName("plazo_meses");
            entity.Property(e => e.TasaInteres)
                .HasColumnType("decimal(5, 4)")
                .HasColumnName("tasa_interes");
            entity.Property(e => e.UsuarioEvaluador).HasColumnName("usuario_evaluador");

            entity.HasOne(d => d.IdSocioNavigation).WithMany(p => p.SolicitudesCreditos)
                .HasForeignKey(d => d.IdSocio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_solicitudes_socio");

            entity.HasOne(d => d.UsuarioEvaluadorNavigation).WithMany(p => p.SolicitudesCreditos)
                .HasForeignKey(d => d.UsuarioEvaluador)
                .HasConstraintName("FK_solicitudes_evaluador");
        });

        modelBuilder.Entity<TablaAmortizacion>(entity =>
        {
            entity.HasKey(e => e.IdCuota).HasName("PK__tabla_am__5EFF6F7E21CCD75B");

            entity.ToTable("tabla_amortizacion");

            entity.HasIndex(e => e.IdCredito, "IX_amortizacion_credito");

            entity.HasIndex(e => e.FechaVencimiento, "IX_amortizacion_fecha");

            entity.HasIndex(e => new { e.IdCredito, e.NumeroCuota }, "UK_credito_cuota").IsUnique();

            entity.Property(e => e.IdCuota).HasColumnName("id_cuota");
            entity.Property(e => e.CuotaCapital)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("cuota_capital");
            entity.Property(e => e.CuotaInteres)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("cuota_interes");
            entity.Property(e => e.CuotaTotal)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("cuota_total");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("pendiente")
                .HasColumnName("estado");
            entity.Property(e => e.FechaVencimiento).HasColumnName("fecha_vencimiento");
            entity.Property(e => e.IdCredito).HasColumnName("id_credito");
            entity.Property(e => e.NumeroCuota).HasColumnName("numero_cuota");
            entity.Property(e => e.SaldoPendiente)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("saldo_pendiente");

            entity.HasOne(d => d.IdCreditoNavigation).WithMany(p => p.TablaAmortizacions)
                .HasForeignKey(d => d.IdCredito)
                .HasConstraintName("FK_amortizacion_credito");
        });

        modelBuilder.Entity<TiposAportacion>(entity =>
        {
            entity.HasKey(e => e.IdTipoAportacion).HasName("PK__tipos_ap__3BC0A6576082FE2D");

            entity.ToTable("tipos_aportacion");

            entity.HasIndex(e => e.NombreTipo, "IX_tipos_aportacion_nombre");

            entity.Property(e => e.IdTipoAportacion).HasColumnName("id_tipo_aportacion");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.EsObligatoria)
                .HasDefaultValue(false)
                .HasColumnName("es_obligatoria");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.Frecuencia)
                .HasMaxLength(20)
                .HasDefaultValue("unica")
                .HasColumnName("frecuencia");
            entity.Property(e => e.MontoMinimo)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("monto_minimo");
            entity.Property(e => e.NombreTipo)
                .HasMaxLength(100)
                .HasColumnName("nombre_tipo");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__usuarios__4E3E04ADEFD6803B");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Cedula, "IX_usuarios_cedula");

            entity.HasIndex(e => e.Correo, "IX_usuarios_correo");

            entity.HasIndex(e => e.PerfilAcceso, "IX_usuarios_perfil");

            entity.HasIndex(e => e.Correo, "UQ__usuarios__2A586E0B63B274BE").IsUnique();

            entity.HasIndex(e => e.Cedula, "UQ__usuarios__415B7BE57C29EFBF").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .HasColumnName("apellidos");
            entity.Property(e => e.Cedula)
                .HasMaxLength(20)
                .HasColumnName("cedula");
            entity.Property(e => e.Ciudad)
                .HasMaxLength(100)
                .HasColumnName("ciudad");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(255)
                .HasColumnName("contrasena");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(200)
                .HasColumnName("direccion");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.Nombres)
                .HasMaxLength(100)
                .HasColumnName("nombres");
            entity.Property(e => e.PerfilAcceso)
                .HasMaxLength(20)
                .HasColumnName("perfil_acceso");
            entity.Property(e => e.UltimoAcceso).HasColumnName("ultimo_acceso");
        });

        modelBuilder.Entity<VistaCarteraCredito>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vista_cartera_creditos");

            entity.Property(e => e.CuotaMensual)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("cuota_mensual");
            entity.Property(e => e.DiasVencimiento).HasColumnName("dias_vencimiento");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasColumnName("estado");
            entity.Property(e => e.EstadoCartera)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("estado_cartera");
            entity.Property(e => e.FechaDesembolso).HasColumnName("fecha_desembolso");
            entity.Property(e => e.FechaVencimiento).HasColumnName("fecha_vencimiento");
            entity.Property(e => e.IdCredito).HasColumnName("id_credito");
            entity.Property(e => e.MontoAprobado)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("monto_aprobado");
            entity.Property(e => e.NombreSocio)
                .HasMaxLength(201)
                .HasColumnName("nombre_socio");
            entity.Property(e => e.NumeroCredito)
                .HasMaxLength(30)
                .HasColumnName("numero_credito");
            entity.Property(e => e.NumeroSocio)
                .HasMaxLength(20)
                .HasColumnName("numero_socio");
            entity.Property(e => e.SaldoCapital)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("saldo_capital");
            entity.Property(e => e.TasaInteres)
                .HasColumnType("decimal(5, 4)")
                .HasColumnName("tasa_interes");
        });

        modelBuilder.Entity<VistaHistorialAhorro>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vista_historial_ahorros");

            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.FechaMovimiento).HasColumnName("fecha_movimiento");
            entity.Property(e => e.IdSocio).HasColumnName("id_socio");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(201)
                .HasColumnName("nombre_completo");
            entity.Property(e => e.NumeroComprobante)
                .HasMaxLength(50)
                .HasColumnName("numero_comprobante");
            entity.Property(e => e.NumeroCuenta)
                .HasMaxLength(30)
                .HasColumnName("numero_cuenta");
            entity.Property(e => e.NumeroSocio)
                .HasMaxLength(20)
                .HasColumnName("numero_socio");
            entity.Property(e => e.SaldoAnterior)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("saldo_anterior");
            entity.Property(e => e.SaldoNuevo)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("saldo_nuevo");
            entity.Property(e => e.TipoCuenta)
                .HasMaxLength(30)
                .HasColumnName("tipo_cuenta");
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(20)
                .HasColumnName("tipo_movimiento");
        });

        modelBuilder.Entity<VistaResumenAportacione>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vista_resumen_aportaciones");

            entity.Property(e => e.EsObligatoria).HasColumnName("es_obligatoria");
            entity.Property(e => e.IdSocio).HasColumnName("id_socio");
            entity.Property(e => e.MontoTotalAportado)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("monto_total_aportado");
            entity.Property(e => e.NombreSocio)
                .HasMaxLength(201)
                .HasColumnName("nombre_socio");
            entity.Property(e => e.NumeroSocio)
                .HasMaxLength(20)
                .HasColumnName("numero_socio");
            entity.Property(e => e.PrimeraAportacion).HasColumnName("primera_aportacion");
            entity.Property(e => e.TipoAportacion)
                .HasMaxLength(100)
                .HasColumnName("tipo_aportacion");
            entity.Property(e => e.TotalAportaciones).HasColumnName("total_aportaciones");
            entity.Property(e => e.UltimaAportacion).HasColumnName("ultima_aportacion");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
