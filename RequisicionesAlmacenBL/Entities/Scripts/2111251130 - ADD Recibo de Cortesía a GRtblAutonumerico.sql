INSERT INTO GRtblAutonumerico
(
    --AutonumericoId - this column value is auto-generated
    Nombre,
    Prefijo,
    Siguiente,
    Ceros,
    Ejercicio,
    Activo,
    FechaCreacion,
    CreadoPorId
)
VALUES
(
    -- AutonumericoId - tinyint
    'Recibo de Cortes�a', -- Nombre - nvarchar
    'RC', -- Prefijo - nvarchar
    1, -- Siguiente - bigint
    6, -- Ceros - int
    NULL, -- Ejercicio - int
    1, -- Activo - bit
    GETDATE(), -- FechaCreacion - datetime
    NULL -- CreadoPorId - INT 
)