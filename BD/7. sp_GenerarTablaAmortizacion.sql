-- Procedimiento para calcular tabla de amortizacion
CREATE PROCEDURE sp_GenerarTablaAmortizacion
    @p_id_credito INT,
    @p_monto DECIMAL(15,2),
    @p_tasa_anual DECIMAL(5,4),
    @p_plazo_meses INT
AS
BEGIN
    DECLARE @v_cuota_mensual DECIMAL(15,2);
    DECLARE @v_tasa_mensual DECIMAL(8,6);
    DECLARE @v_saldo_pendiente DECIMAL(15,2);
    DECLARE @v_cuota_capital DECIMAL(15,2);
    DECLARE @v_cuota_interes DECIMAL(15,2);
    DECLARE @v_contador INT = 1;
    DECLARE @v_fecha_vencimiento DATE;
    
    SET @v_tasa_mensual = @p_tasa_anual / 12 / 100;
    SET @v_cuota_mensual = @p_monto * (@v_tasa_mensual * POWER(1 + @v_tasa_mensual, @p_plazo_meses)) / 
                          (POWER(1 + @v_tasa_mensual, @p_plazo_meses) - 1);
    SET @v_saldo_pendiente = @p_monto;
    
    -- Limpiar tabla anterior si existe
    DELETE FROM tabla_amortizacion WHERE id_credito = @p_id_credito;
    
    WHILE @v_contador <= @p_plazo_meses
    BEGIN
        SET @v_cuota_interes = @v_saldo_pendiente * @v_tasa_mensual;
        SET @v_cuota_capital = @v_cuota_mensual - @v_cuota_interes;
        SET @v_saldo_pendiente = @v_saldo_pendiente - @v_cuota_capital;
        SET @v_fecha_vencimiento = DATEADD(MONTH, @v_contador, GETDATE());
        
        INSERT INTO tabla_amortizacion (
            id_credito, numero_cuota, fecha_vencimiento, 
            cuota_capital, cuota_interes, cuota_total, saldo_pendiente
        ) VALUES (
            @p_id_credito, @v_contador, @v_fecha_vencimiento,
            @v_cuota_capital, @v_cuota_interes, @v_cuota_mensual, @v_saldo_pendiente
        );
        
        SET @v_contador = @v_contador + 1;
    END;
END;