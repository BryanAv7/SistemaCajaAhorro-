CREATE TRIGGER tr_actualizar_saldo_ahorro
ON movimientos_ahorro
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE cuentas_ahorro 
    SET saldo_actual = i.saldo_nuevo,
        fecha_ultimo_movimiento = i.fecha_movimiento
    FROM cuentas_ahorro ca
    INNER JOIN inserted i ON ca.id_cuenta_ahorro = i.id_cuenta_ahorro;
END;