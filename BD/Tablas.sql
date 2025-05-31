-- =====================================================
-- SISTEMA DE CAJAS DE AHORRO - DISE�O DE BASE DE DATOS
-- ADAPTADO PARA SQL SERVER
-- =====================================================

-- =====================================================
-- TABLAS DE CONFIGURACI�N Y USUARIOS DEL SISTEMA
-- =====================================================

-- Tabla de Usuarios del Sistema (Personal que opera)
CREATE TABLE usuarios (
    id_usuario INT IDENTITY(1,1) PRIMARY KEY,
    cedula NVARCHAR(20) NOT NULL UNIQUE,
    nombres NVARCHAR(100) NOT NULL,
    apellidos NVARCHAR(100) NOT NULL,
    direccion NVARCHAR(200),
    correo NVARCHAR(100) NOT NULL UNIQUE,
    fecha_nacimiento DATE,
    ciudad NVARCHAR(100),
    contrasena NVARCHAR(255) NOT NULL,
    perfil_acceso NVARCHAR(20) NOT NULL CHECK (perfil_acceso IN ('administrador', 'cajero', 'contador', 'supervisor')),
    estado BIT DEFAULT 1,
    fecha_creacion DATETIME2 DEFAULT GETDATE(),
    ultimo_acceso DATETIME2 NULL
);

-- Indices para usuarios
CREATE INDEX IX_usuarios_cedula ON usuarios(cedula);
CREATE INDEX IX_usuarios_correo ON usuarios(correo);
CREATE INDEX IX_usuarios_perfil ON usuarios(perfil_acceso);

-- Tabla de Plan de Cuentas Contables
CREATE TABLE plan_cuentas (
    id_cuenta INT IDENTITY(1,1) PRIMARY KEY,
    codigo_cuenta NVARCHAR(20) NOT NULL UNIQUE,
    nombre_categoria NVARCHAR(150) NOT NULL,
    descripcion NVARCHAR(MAX),
    tipo_cuenta NVARCHAR(20) NOT NULL CHECK (tipo_cuenta IN ('activo', 'pasivo', 'patrimonio', 'ingreso', 'gasto')),
    cuenta_padre INT NULL,
    nivel INT DEFAULT 1,
    valor_monetario DECIMAL(15,2) DEFAULT 0.00,
    estado BIT DEFAULT 1,
    fecha_creacion DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT FK_plan_cuentas_padre FOREIGN KEY (cuenta_padre) REFERENCES plan_cuentas(id_cuenta)
);

-- Indices para plan de cuentas
CREATE INDEX IX_plan_cuentas_codigo ON plan_cuentas(codigo_cuenta);
CREATE INDEX IX_plan_cuentas_tipo ON plan_cuentas(tipo_cuenta);

-- Tabla de Parametros Generales del Sistema
CREATE TABLE parametros_generales (
    id_parametro INT IDENTITY(1,1) PRIMARY KEY,
    nombre_parametro NVARCHAR(100) NOT NULL UNIQUE,
    valor_parametro NVARCHAR(MAX) NOT NULL,
    descripcion NVARCHAR(MAX),
    tipo_dato NVARCHAR(20) DEFAULT 'string' CHECK (tipo_dato IN ('string', 'number', 'boolean', 'date', 'json')),
    modulo NVARCHAR(50), -- Para agrupar parametros por modulo
    fecha_actualizacion DATETIME2 DEFAULT GETDATE(),
    usuario_actualizacion INT,
    
    CONSTRAINT FK_parametros_usuario FOREIGN KEY (usuario_actualizacion) REFERENCES usuarios(id_usuario)
);

-- indice para parametros
CREATE INDEX IX_parametros_modulo ON parametros_generales(modulo);

-- =====================================================
-- TABLAS DE SOCIOS Y CUENTAS
-- =====================================================

-- Tabla de Socios (Clientes de la Caja de Ahorro)
CREATE TABLE socios (
    id_socio INT IDENTITY(1,1) PRIMARY KEY,
    numero_socio NVARCHAR(20) NOT NULL UNIQUE,
    cedula NVARCHAR(20) NOT NULL UNIQUE,
    nombres NVARCHAR(100) NOT NULL,
    apellidos NVARCHAR(100) NOT NULL,
    correo NVARCHAR(100) UNIQUE,
    telefono NVARCHAR(20),
    direccion NVARCHAR(200),
    fecha_nacimiento DATE,
    ciudad NVARCHAR(100),
    fecha_ingreso DATE NOT NULL,
    estado NVARCHAR(20) DEFAULT 'activo' CHECK (estado IN ('activo', 'inactivo', 'suspendido')),
    fecha_creacion DATETIME2 DEFAULT GETDATE(),
    observaciones NVARCHAR(MAX)
);

-- indices para socios
CREATE INDEX IX_socios_numero ON socios(numero_socio);
CREATE INDEX IX_socios_cedula ON socios(cedula);
CREATE INDEX IX_socios_estado ON socios(estado);

-- Tabla de Cuentas de Ahorro de los Socios
CREATE TABLE cuentas_ahorro (
    id_cuenta_ahorro INT IDENTITY(1,1) PRIMARY KEY,
    id_socio INT NOT NULL,
    numero_cuenta NVARCHAR(30) NOT NULL UNIQUE,
    tipo_cuenta NVARCHAR(30) NOT NULL CHECK (tipo_cuenta IN ('ahorro_programado', 'ahorro_libre', 'ahorro_navide�o')),
    saldo_actual DECIMAL(15,2) DEFAULT 0.00,
    tasa_interes DECIMAL(5,4) DEFAULT 0.0000, -- Porcentaje anual
    fecha_apertura DATE NOT NULL,
    estado NVARCHAR(20) DEFAULT 'activa' CHECK (estado IN ('activa', 'cerrada', 'bloqueada')),
    monto_minimo DECIMAL(15,2) DEFAULT 0.00,
    fecha_ultimo_movimiento DATETIME2 NULL,
    
    CONSTRAINT FK_cuentas_socio FOREIGN KEY (id_socio) REFERENCES socios(id_socio)
);

-- indices para cuentas de ahorro
CREATE INDEX IX_cuentas_socio ON cuentas_ahorro(id_socio);
CREATE INDEX IX_cuentas_numero ON cuentas_ahorro(numero_cuenta);
CREATE INDEX IX_cuentas_estado ON cuentas_ahorro(estado);

-- =====================================================
-- TABLAS DE APORTACIONES
-- =====================================================

-- Tabla de Tipos de Aportaciones
CREATE TABLE tipos_aportacion (
    id_tipo_aportacion INT IDENTITY(1,1) PRIMARY KEY,
    nombre_tipo NVARCHAR(100) NOT NULL,
    descripcion NVARCHAR(MAX),
    monto_minimo DECIMAL(15,2) DEFAULT 0.00,
    es_obligatoria BIT DEFAULT 0,
    frecuencia NVARCHAR(20) DEFAULT 'unica' CHECK (frecuencia IN ('unica', 'mensual', 'trimestral', 'anual')),
    estado BIT DEFAULT 1
);

-- indice para tipos de aportacion
CREATE INDEX IX_tipos_aportacion_nombre ON tipos_aportacion(nombre_tipo);

-- Tabla de Aportaciones de los Socios
CREATE TABLE aportaciones (
    id_aportacion INT IDENTITY(1,1) PRIMARY KEY,
    id_socio INT NOT NULL,
    id_tipo_aportacion INT NOT NULL,
    numero_comprobante NVARCHAR(50) UNIQUE,
    monto DECIMAL(15,2) NOT NULL,
    fecha_aportacion DATE NOT NULL,
    motivo NVARCHAR(MAX),
    metodo_pago NVARCHAR(20) DEFAULT 'efectivo' CHECK (metodo_pago IN ('efectivo', 'transferencia', 'cheque')),
    estado NVARCHAR(20) DEFAULT 'registrada' CHECK (estado IN ('registrada', 'confirmada', 'anulada')),
    usuario_registro INT NOT NULL,
    fecha_registro DATETIME2 DEFAULT GETDATE(),
    observaciones NVARCHAR(MAX),
    
    CONSTRAINT FK_aportaciones_socio FOREIGN KEY (id_socio) REFERENCES socios(id_socio),
    CONSTRAINT FK_aportaciones_tipo FOREIGN KEY (id_tipo_aportacion) REFERENCES tipos_aportacion(id_tipo_aportacion),
    CONSTRAINT FK_aportaciones_usuario FOREIGN KEY (usuario_registro) REFERENCES usuarios(id_usuario)
);

-- indices para aportaciones
CREATE INDEX IX_aportaciones_socio ON aportaciones(id_socio);
CREATE INDEX IX_aportaciones_fecha ON aportaciones(fecha_aportacion);
CREATE INDEX IX_aportaciones_estado ON aportaciones(estado);

-- =====================================================
-- TABLAS DE CREDITOS
-- =====================================================

-- Tabla de Solicitudes de Credito
CREATE TABLE solicitudes_credito (
    id_solicitud INT IDENTITY(1,1) PRIMARY KEY,
    id_socio INT NOT NULL,
    monto_solicitado DECIMAL(15,2) NOT NULL,
    plazo_meses INT NOT NULL,
    tasa_interes DECIMAL(5,4) NOT NULL,
    destino_credito NVARCHAR(MAX) NOT NULL,
    ingresos_mensuales DECIMAL(15,2),
    gastos_mensuales DECIMAL(15,2),
    fecha_solicitud DATE NOT NULL,
    estado NVARCHAR(20) DEFAULT 'pendiente' CHECK (estado IN ('pendiente', 'aprobado', 'rechazado', 'desembolsado')),
    motivo_rechazo NVARCHAR(MAX) NULL,
    usuario_evaluador INT NULL,
    fecha_evaluacion DATE NULL,
    observaciones NVARCHAR(MAX),
    
    CONSTRAINT FK_solicitudes_socio FOREIGN KEY (id_socio) REFERENCES socios(id_socio),
    CONSTRAINT FK_solicitudes_evaluador FOREIGN KEY (usuario_evaluador) REFERENCES usuarios(id_usuario)
);

-- Indices para solicitudes de credito
CREATE INDEX IX_solicitudes_socio ON solicitudes_credito(id_socio);
CREATE INDEX IX_solicitudes_estado ON solicitudes_credito(estado);
CREATE INDEX IX_solicitudes_fecha ON solicitudes_credito(fecha_solicitud);

-- Tabla de Creditos Aprobados
CREATE TABLE creditos (
    id_credito INT IDENTITY(1,1) PRIMARY KEY,
    id_solicitud INT NOT NULL,
    id_socio INT NOT NULL,
    numero_credito NVARCHAR(30) NOT NULL UNIQUE,
    monto_aprobado DECIMAL(15,2) NOT NULL,
    tasa_interes DECIMAL(5,4) NOT NULL,
    plazo_meses INT NOT NULL,
    cuota_mensual DECIMAL(15,2) NOT NULL,
    saldo_capital DECIMAL(15,2) NOT NULL,
    fecha_aprobacion DATE NOT NULL,
    fecha_desembolso DATE NULL,
    fecha_vencimiento DATE NOT NULL,
    estado NVARCHAR(20) DEFAULT 'aprobado' CHECK (estado IN ('aprobado', 'desembolsado', 'vigente', 'cancelado', 'vencido')),
    usuario_aprobacion INT NOT NULL,
    
    CONSTRAINT FK_creditos_solicitud FOREIGN KEY (id_solicitud) REFERENCES solicitudes_credito(id_solicitud),
    CONSTRAINT FK_creditos_socio FOREIGN KEY (id_socio) REFERENCES socios(id_socio),
    CONSTRAINT FK_creditos_usuario FOREIGN KEY (usuario_aprobacion) REFERENCES usuarios(id_usuario)
);

-- Indices para creditos
CREATE INDEX IX_creditos_numero ON creditos(numero_credito);
CREATE INDEX IX_creditos_socio ON creditos(id_socio);
CREATE INDEX IX_creditos_estado ON creditos(estado);

-- Tabla de Amortizacion de Creditos
CREATE TABLE tabla_amortizacion (
    id_cuota INT IDENTITY(1,1) PRIMARY KEY,
    id_credito INT NOT NULL,
    numero_cuota INT NOT NULL,
    fecha_vencimiento DATE NOT NULL,
    cuota_capital DECIMAL(15,2) NOT NULL,
    cuota_interes DECIMAL(15,2) NOT NULL,
    cuota_total DECIMAL(15,2) NOT NULL,
    saldo_pendiente DECIMAL(15,2) NOT NULL,
    estado NVARCHAR(20) DEFAULT 'pendiente' CHECK (estado IN ('pendiente', 'pagada', 'vencida')),
    
    CONSTRAINT FK_amortizacion_credito FOREIGN KEY (id_credito) REFERENCES creditos(id_credito) ON DELETE CASCADE,
    CONSTRAINT UK_credito_cuota UNIQUE (id_credito, numero_cuota)
);

-- Indices para tabla de amortizacion
CREATE INDEX IX_amortizacion_credito ON tabla_amortizacion(id_credito);
CREATE INDEX IX_amortizacion_fecha ON tabla_amortizacion(fecha_vencimiento);

-- Tabla de Pagos de Creditos
CREATE TABLE pagos_credito (
    id_pago INT IDENTITY(1,1) PRIMARY KEY,
    id_credito INT NOT NULL,
    id_cuota INT NULL, -- NULL para pagos anticipados o extraordinarios
    monto_pago DECIMAL(15,2) NOT NULL,
    monto_capital DECIMAL(15,2) NOT NULL,
    monto_interes DECIMAL(15,2) NOT NULL,
    monto_mora DECIMAL(15,2) DEFAULT 0.00,
    fecha_pago DATE NOT NULL,
    metodo_pago NVARCHAR(20) DEFAULT 'efectivo' CHECK (metodo_pago IN ('efectivo', 'transferencia', 'cheque')),
    numero_comprobante NVARCHAR(50),
    usuario_registro INT NOT NULL,
    observaciones NVARCHAR(MAX),
    
    CONSTRAINT FK_pagos_credito FOREIGN KEY (id_credito) REFERENCES creditos(id_credito),
    CONSTRAINT FK_pagos_cuota FOREIGN KEY (id_cuota) REFERENCES tabla_amortizacion(id_cuota),
    CONSTRAINT FK_pagos_usuario FOREIGN KEY (usuario_registro) REFERENCES usuarios(id_usuario)
);

-- Indices para pagos de credito
CREATE INDEX IX_pagos_credito ON pagos_credito(id_credito);
CREATE INDEX IX_pagos_fecha ON pagos_credito(fecha_pago);

-- =====================================================
-- TABLAS DE MOVIMIENTOS Y TRANSACCIONES
-- =====================================================

-- Tabla de Movimientos de Ahorro (Depositos y Retiros)
CREATE TABLE movimientos_ahorro (
    id_movimiento INT IDENTITY(1,1) PRIMARY KEY,
    id_cuenta_ahorro INT NOT NULL,
    tipo_movimiento NVARCHAR(20) NOT NULL CHECK (tipo_movimiento IN ('deposito', 'retiro', 'interes', 'comision', 'ajuste')),
    monto DECIMAL(15,2) NOT NULL,
    saldo_anterior DECIMAL(15,2) NOT NULL,
    saldo_nuevo DECIMAL(15,2) NOT NULL,
    descripcion NVARCHAR(MAX),
    fecha_movimiento DATETIME2 DEFAULT GETDATE(),
    numero_comprobante NVARCHAR(50),
    usuario_registro INT NOT NULL,
    id_cuenta_contable INT,
    
    CONSTRAINT FK_movimientos_cuenta FOREIGN KEY (id_cuenta_ahorro) REFERENCES cuentas_ahorro(id_cuenta_ahorro),
    CONSTRAINT FK_movimientos_usuario FOREIGN KEY (usuario_registro) REFERENCES usuarios(id_usuario),
    CONSTRAINT FK_movimientos_cuenta_contable FOREIGN KEY (id_cuenta_contable) REFERENCES plan_cuentas(id_cuenta)
);

-- Indices para movimientos de ahorro
CREATE INDEX IX_movimientos_cuenta ON movimientos_ahorro(id_cuenta_ahorro);
CREATE INDEX IX_movimientos_fecha ON movimientos_ahorro(fecha_movimiento);
CREATE INDEX IX_movimientos_tipo ON movimientos_ahorro(tipo_movimiento);

-- Tabla de Libro Diario (Registro Contable)
CREATE TABLE libro_diario (
    id_asiento INT IDENTITY(1,1) PRIMARY KEY,
    numero_asiento NVARCHAR(20) NOT NULL UNIQUE,
    fecha_asiento DATE NOT NULL,
    concepto NVARCHAR(MAX) NOT NULL,
    total_debe DECIMAL(15,2) NOT NULL,
    total_haber DECIMAL(15,2) NOT NULL,
    usuario_registro INT NOT NULL,
    fecha_registro DATETIME2 DEFAULT GETDATE(),
    estado NVARCHAR(20) DEFAULT 'confirmado' CHECK (estado IN ('borrador', 'confirmado', 'anulado')),
    
    CONSTRAINT FK_libro_usuario FOREIGN KEY (usuario_registro) REFERENCES usuarios(id_usuario)
);

-- indices para libro diario
CREATE INDEX IX_libro_fecha ON libro_diario(fecha_asiento);
CREATE INDEX IX_libro_numero ON libro_diario(numero_asiento);

-- Tabla de Detalles del Libro Diario
CREATE TABLE detalle_libro_diario (
    id_detalle INT IDENTITY(1,1) PRIMARY KEY,
    id_asiento INT NOT NULL,
    id_cuenta_contable INT NOT NULL,
    concepto NVARCHAR(MAX),
    debe DECIMAL(15,2) DEFAULT 0.00,
    haber DECIMAL(15,2) DEFAULT 0.00,
    orden_linea INT DEFAULT 1,
    
    CONSTRAINT FK_detalle_asiento FOREIGN KEY (id_asiento) REFERENCES libro_diario(id_asiento) ON DELETE CASCADE,
    CONSTRAINT FK_detalle_cuenta FOREIGN KEY (id_cuenta_contable) REFERENCES plan_cuentas(id_cuenta)
);

-- indices para detalle libro diario
CREATE INDEX IX_detalle_asiento ON detalle_libro_diario(id_asiento);
CREATE INDEX IX_detalle_cuenta ON detalle_libro_diario(id_cuenta_contable);

-- =====================================================
-- TABLAS DE AUDITORIA Y SEGUIMIENTO
-- =====================================================

-- Tabla de Historial de Acciones (Auditoria)
CREATE TABLE historial_acciones (
    id_historial INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario INT NOT NULL,
    accion NVARCHAR(100) NOT NULL,
    tabla_afectada NVARCHAR(50) NOT NULL,
    id_registro_afectado INT,
    valores_anteriores NVARCHAR(MAX), -- JSON en SQL Server
    valores_nuevos NVARCHAR(MAX),     -- JSON en SQL Server
    ip_usuario NVARCHAR(45),
    user_agent NVARCHAR(MAX),
    fecha_accion DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT FK_historial_usuario FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
);

-- Indices para historial de acciones
CREATE INDEX IX_historial_usuario ON historial_acciones(id_usuario);
CREATE INDEX IX_historial_fecha ON historial_acciones(fecha_accion);
CREATE INDEX IX_historial_tabla ON historial_acciones(tabla_afectada);

-- Tabla de Sesiones de Usuario
CREATE TABLE sesiones_usuario (
    id_sesion INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario INT NOT NULL,
    token_sesion NVARCHAR(255) NOT NULL UNIQUE,
    ip_acceso NVARCHAR(45),
    user_agent NVARCHAR(MAX),
    fecha_inicio DATETIME2 DEFAULT GETDATE(),
    fecha_ultimo_acceso DATETIME2 DEFAULT GETDATE(),
    estado NVARCHAR(20) DEFAULT 'activa' CHECK (estado IN ('activa', 'cerrada', 'expirada')),
    
    CONSTRAINT FK_sesiones_usuario FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario) ON DELETE CASCADE
);

-- Indices para sesiones
CREATE INDEX IX_sesiones_token ON sesiones_usuario(token_sesion);
CREATE INDEX IX_sesiones_usuario ON sesiones_usuario(id_usuario);