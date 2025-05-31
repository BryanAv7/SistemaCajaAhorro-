CREATE VIEW vista_historial_ahorros AS
SELECT 
    s.id_socio,
    s.numero_socio,
    CONCAT(s.nombres, ' ', s.apellidos) as nombre_completo,
    ca.numero_cuenta,
    ca.tipo_cuenta,
    ma.tipo_movimiento,
    ma.monto,
    ma.saldo_anterior,
    ma.saldo_nuevo,
    ma.descripcion,
    ma.fecha_movimiento,
    ma.numero_comprobante
FROM socios s
JOIN cuentas_ahorro ca ON s.id_socio = ca.id_socio
JOIN movimientos_ahorro ma ON ca.id_cuenta_ahorro = ma.id_cuenta_ahorro
WHERE s.estado = 'activo' AND ca.estado = 'activa';