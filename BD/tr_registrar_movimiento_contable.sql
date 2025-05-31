CREATE TRIGGER tr_registrar_movimiento_contable
ON movimientos_ahorro
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @v_numero_asiento NVARCHAR(20);
    DECLARE @v_id_asiento INT;
    
    -- Cursor para procesar cada registro insertado
    DECLARE cur_movimientos CURSOR FOR
    SELECT id_movimiento, tipo_movimiento, monto, fecha_movimiento, descripcion, usuario_registro
    FROM inserted;
    
    DECLARE @id_mov INT, 
            @tipo_mov NVARCHAR(20), 
            @monto DECIMAL(15,2), 
            @fecha_mov DATETIME2, 
            @desc NVARCHAR(MAX),
            @usuario INT;
    
    OPEN cur_movimientos;
    FETCH NEXT FROM cur_movimientos INTO @id_mov, @tipo_mov, @monto, @fecha_mov, @desc, @usuario;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Generar número de asiento
        SET @v_numero_asiento = 'AS-' + FORMAT(GETDATE(), 'yyyyMMdd') + '-' + RIGHT('000000' + CAST(@id_mov AS NVARCHAR), 6);
        
        -- Insertar encabezado del asiento
        INSERT INTO libro_diario (numero_asiento, fecha_asiento, concepto, total_debe, total_haber, usuario_registro)
        VALUES (@v_numero_asiento, CAST(@fecha_mov AS DATE), 'Movimiento de ahorro - ' + ISNULL(@desc, ''), @monto, @monto, @usuario);
        
        SET @v_id_asiento = SCOPE_IDENTITY();
        
        -- Insertar detalles según el tipo de movimiento
        IF @tipo_mov = 'deposito'
        BEGIN
            -- Débito a Caja, Crédito a Ahorros de Socios
            INSERT INTO detalle_libro_diario (id_asiento, id_cuenta_contable, concepto, debe, haber, orden_linea)
            VALUES (@v_id_asiento, 1, 'Ingreso por depósito', @monto, 0.00, 1),
                   (@v_id_asiento, 2, 'Aumento en ahorros', 0.00, @monto, 2);
        END
        ELSE IF @tipo_mov = 'retiro'
        BEGIN
            -- Débito a Ahorros de Socios, Crédito a Caja
            INSERT INTO detalle_libro_diario (id_asiento, id_cuenta_contable, concepto, debe, haber, orden_linea)
            VALUES (@v_id_asiento, 2, 'Disminución en ahorros', @monto, 0.00, 1),
                   (@v_id_asiento, 1, 'Salida por retiro', 0.00, @monto, 2);
        END;
        
        FETCH NEXT FROM cur_movimientos INTO @id_mov, @tipo_mov, @monto, @fecha_mov, @desc, @usuario;
    END;
    
    CLOSE cur_movimientos;
    DEALLOCATE cur_movimientos;
END;
