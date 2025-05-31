-- =====================================================
-- DATOS INICIALES
-- =====================================================

-- Insertar plan de cuentas basico
INSERT INTO plan_cuentas (codigo_cuenta, nombre_categoria, tipo_cuenta, nivel) VALUES
('1', 'ACTIVOS', 'activo', 1),
('11', 'ACTIVO CORRIENTE', 'activo', 2),
('1101', 'Caja General', 'activo', 3),
('1102', 'Bancos', 'activo', 3),
('1201', 'Cartera de Creditos', 'activo', 3),
('2', 'PASIVOS', 'pasivo', 1),
('21', 'PASIVO CORRIENTE', 'pasivo', 2),
('2101', 'Ahorros de Socios', 'pasivo', 3),
('3', 'PATRIMONIO', 'patrimonio', 1),
('3101', 'Aportes de Socios', 'patrimonio', 3),
('4', 'INGRESOS', 'ingreso', 1),
('4101', 'Intereses por Creditos', 'ingreso', 3),
('5', 'GASTOS', 'gasto', 1),
('5101', 'Gastos Administrativos', 'gasto', 3);

-- Insertar tipos de aportacion basicos
INSERT INTO tipos_aportacion (nombre_tipo, descripcion, monto_minimo, es_obligatoria) VALUES
('Aportacion Inicial', 'Aportacion obligatoria al ingresar como socio', 100.00, 1),
('Aportacion Mensual', 'Aportacion mensual obligatoria', 25.00, 1),
('Aportacion Voluntaria', 'Aportaciones adicionales voluntarias', 10.00, 0),
('Aportacion Extraordinaria', 'Para eventos especiales o emergencias', 50.00, 0);

-- Insertar parametros generales
INSERT INTO parametros_generales (nombre_parametro, valor_parametro, descripcion, tipo_dato, modulo) VALUES
('TASA_INTERES_AHORRO', '0.0350', 'Tasa de interés anual para cuentas de ahorro', 'number', 'ahorro'),
('TASA_INTERES_CREDITO', '0.1200', 'Tasa de interés anual base para creditos', 'number', 'credito'),
('MONTO_MINIMO_CREDITO', '500.00', 'Monto manimo para solicitar credito', 'number', 'credito'),
('MESES_MINIMOS_SOCIO', '6', 'Meses minimos como socio para solicitar credito', 'number', 'credito'),
('PENALIZACION_MORA', '0.0500', 'Tasa de penalizacion por mora mensual', 'number', 'credito');

-- Insertar usuario administrador inicial
INSERT INTO usuarios (cedula, nombres, apellidos, correo, ciudad, contrasena, perfil_acceso) VALUES
('0000000001', 'Administrador', 'Sistema', 'admin@cajadeahorro.com', 'Cuenca', 
'$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'administrador');