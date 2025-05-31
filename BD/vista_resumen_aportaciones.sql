CREATE VIEW vista_resumen_aportaciones AS
SELECT 
    s.id_socio,
    s.numero_socio,
    CONCAT(s.nombres, ' ', s.apellidos) as nombre_socio,
    ta.nombre_tipo as tipo_aportacion,
    ta.es_obligatoria,
    COUNT(a.id_aportacion) as total_aportaciones,
    SUM(a.monto) as monto_total_aportado,
    MAX(a.fecha_aportacion) as ultima_aportacion,
    MIN(a.fecha_aportacion) as primera_aportacion
FROM socios s
LEFT JOIN aportaciones a ON s.id_socio = a.id_socio AND a.estado = 'confirmada'
LEFT JOIN tipos_aportacion ta ON a.id_tipo_aportacion = ta.id_tipo_aportacion
WHERE s.estado = 'activo'
GROUP BY s.id_socio, s.numero_socio, s.nombres, s.apellidos, ta.nombre_tipo, ta.es_obligatoria;
