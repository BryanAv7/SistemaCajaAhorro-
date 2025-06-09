CREATE VIEW vista_cartera_creditos AS
SELECT 
    c.id_credito,
    c.numero_credito,
    s.numero_socio,
    CONCAT(s.nombres, ' ', s.apellidos) as nombre_socio,
    c.monto_aprobado,
    c.saldo_capital,
    c.tasa_interes,
    c.cuota_mensual,
    c.fecha_desembolso,
    c.fecha_vencimiento,
    c.estado,
    DATEDIFF(DAY, c.fecha_vencimiento, GETDATE()) as dias_vencimiento,
    CASE 
        WHEN c.estado = 'vencido' THEN 'VENCIDO'
        WHEN DATEDIFF(DAY, c.fecha_vencimiento, GETDATE()) > 0 THEN 'EN MORA'
        WHEN DATEDIFF(DAY, GETDATE(), c.fecha_vencimiento) <= 30 THEN 'POR VENCER'
        ELSE 'AL DIA'
    END as estado_cartera
FROM creditos c
JOIN socios s ON c.id_socio = s.id_socio
WHERE c.estado IN ('desembolsado', 'vigente', 'vencido');