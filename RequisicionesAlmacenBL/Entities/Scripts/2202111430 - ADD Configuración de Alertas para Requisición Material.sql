SET IDENTITY_INSERT GRtblControlMaestro ON
INSERT INTO GRtblControlMaestro
(
    ControlId,
    Control,
    Valor,
    Sistema,
    Activo,
    ControlSencillo,
    FechaCreacion
)
VALUES
(126, 'AlertaEtapa', 'Requisici�n Material. Nuevo', 1, 1, 0, GETDATE()),
(127, 'AlertaEtapa', 'Requisici�n Material. Validaci�', 1, 1, 0, GETDATE()),
(128, 'AlertaEtapaAccio', 'Enviar', 1, 1, 0, GETDATE()),
(129, 'AlertaEtapaAccio', 'Autorizar', 1, 1, 0, GETDATE()),
(130, 'AlertaEtapaAccio', 'Rechazar', 1, 1, 0, GETDATE()),
(131, 'AlertaEtapaAccio', 'Revisi�', 1, 1, 0, GETDATE())
SET IDENTITY_INSERT GRtblControlMaestro OFF
GO

SET IDENTITY_INSERT GRtblAlertaEtapaAccion ON
INSERT INTO GRtblAlertaEtapaAccion
(
    AlertaEtapaAccionId,
    EtapaId,
    AccionId,
    PermiteAutorizacion,
    PermiteNotificacion,
    Timestamp
)
VALUES
(
    1, -- AlertaEtapaAccionId - int
    126, -- EtapaId - int
    128, -- AccionId - int
    1, -- PermiteAutorizacion - bit
    1, -- PermiteNotificacion - bit
    NULL -- Timestamp - timestamp
),
(
    2, -- AlertaEtapaAccionId - int
    127, -- EtapaId - int
    129, -- AccionId - int
    0, -- PermiteAutorizacion - bit
    1, -- PermiteNotificacion - bit
    NULL -- Timestamp - timestamp
),
(
    3, -- AlertaEtapaAccionId - int
    127, -- EtapaId - int
    130, -- AccionId - int
    0, -- PermiteAutorizacion - bit
    1, -- PermiteNotificacion - bit
    NULL -- Timestamp - timestamp
),
(
    4, -- AlertaEtapaAccionId - int
    127, -- EtapaId - int
    131, -- AccionId - int
    0, -- PermiteAutorizacion - bit
    1, -- PermiteNotificacion - bit
    NULL -- Timestamp - timestamp
)
SET IDENTITY_INSERT GRtblAlertaEtapaAccion OFF
GO

SET IDENTITY_INSERT GRtblAlertaDefinicion ON
INSERT INTO GRtblAlertaDefinicion
(
    AlertaDefinicionId, -- this column value is auto-generated
    NombreCorto,
    Descripcion,
    AlertaEtapaAccionId,
    NodoMenuId,
    RutaAccion,
    TablaReferencia,
    CampoId,
    CampoCodigo,
    CampoEstadoRegistro,
    NuevoEstado,
    CambiarEstatusATramite,
    EtapaAccionAlAutorizarId,
    EtapaAccionAlRechazarId,
    EtapaAccionAlRevisionId,
    Borrado,
    Timestamp,
    RutaAccionNodoMenuId
)
VALUES
(
    1, -- AlertaDefinicionId - int
    'Requisici�n Material - Autorizaci�n', -- NombreCorto - nvarchar
    'Requisici�n Material - Etapa Nueva - Acci�n Enviar - Autorizaci�n', -- Descripcion - nvarchar
    1, -- AlertaEtapaAccionId - int
    8, -- NodoMenuId - int
    'compras/requisiciones/requisicionmaterial/editar/{id}', -- RutaAccion - nvarchar
    'ARtblRequisicionMaterial', -- TablaReferencia - nvarchar
    'RequisicionMaterialId', -- CampoId - nvarchar
    'CodigoRequisicion', -- CampoCodigo - nvarchar
    'EstatusId', -- CampoEstadoRegistro - nvarchar
    69, -- NuevoEstado - int
    1, -- CambiarEstatusATramite - bit
    2, -- EtapaAccionAlAutorizarId - int
    3, -- EtapaAccionAlRechazarId - int
    4, -- EtapaAccionAlRevisionId - int
    0, -- Borrado - bit
    NULL, -- Timestamp - timestamp
    8 -- RutaAccionNodoMenuId - int
),
(
    2, -- AlertaDefinicionId - int
    'Requisici�n Material - Autorizada', -- NombreCorto - nvarchar
    'Requisici�n Material - Etapa Validaci�n - Acci�n Autorizar', -- Descripcion - nvarchar
    2, -- AlertaEtapaAccionId - int
    8, -- NodoMenuId - int
    'compras/requisiciones/requisicionmaterial/editar/{id}', -- RutaAccion - nvarchar
    'ARtblRequisicionMaterial', -- TablaReferencia - nvarchar
    'RequisicionMaterialId', -- CampoId - nvarchar
    'CodigoRequisicion', -- CampoCodigo - nvarchar
    'EstatusId', -- CampoEstadoRegistro - nvarchar
    64, -- NuevoEstado - int
    1, -- CambiarEstatusATramite - bit
    NULL, -- EtapaAccionAlAutorizarId - int
    NULL, -- EtapaAccionAlRechazarId - int
    NULL, -- EtapaAccionAlRevisionId - int
    0, -- Borrado - bit
    NULL, -- Timestamp - timestamp
    8 -- RutaAccionNodoMenuId - int
),
(
    3, -- AlertaDefinicionId - int
    'Requisici�n Material - Rechazada', -- NombreCorto - nvarchar
    'Requisici�n Material - Etapa Validaci�n - Acci�n Rechazar', -- Descripcion - nvarchar
    3, -- AlertaEtapaAccionId - int
    8, -- NodoMenuId - int
    'compras/requisiciones/requisicionmaterial/editar/{id}', -- RutaAccion - nvarchar
    'ARtblRequisicionMaterial', -- TablaReferencia - nvarchar
    'RequisicionMaterialId', -- CampoId - nvarchar
    'CodigoRequisicion', -- CampoCodigo - nvarchar
    'EstatusId', -- CampoEstadoRegistro - nvarchar
    74, -- NuevoEstado - int
    1, -- CambiarEstatusATramite - bit
    NULL, -- EtapaAccionAlAutorizarId - int
    NULL, -- EtapaAccionAlRechazarId - int
    NULL, -- EtapaAccionAlRevisionId - int
    0, -- Borrado - bit
    NULL, -- Timestamp - timestamp
    8 -- RutaAccionNodoMenuId - int
),
(
    4, -- AlertaDefinicionId - int
    'Requisici�n Material - En Revisi�n', -- NombreCorto - nvarchar
    'Requisici�n Material - Etapa Validaci�n - Acci�n Revisi�n', -- Descripcion - nvarchar
    4, -- AlertaEtapaAccionId - int
    8, -- NodoMenuId - int
    'compras/requisiciones/requisicionmaterial/editar/{id}', -- RutaAccion - nvarchar
    'ARtblRequisicionMaterial', -- TablaReferencia - nvarchar
    'RequisicionMaterialId', -- CampoId - nvarchar
    'CodigoRequisicion', -- CampoCodigo - nvarchar
    'EstatusId', -- CampoEstadoRegistro - nvarchar
    76, -- NuevoEstado - int
    1, -- CambiarEstatusATramite - bit
    NULL, -- EtapaAccionAlAutorizarId - int
    NULL, -- EtapaAccionAlRechazarId - int
    NULL, -- EtapaAccionAlRevisionId - int
    0, -- Borrado - bit
    NULL, -- Timestamp - timestamp
    8 -- RutaAccionNodoMenuId - int
)
SET IDENTITY_INSERT GRtblAlertaDefinicion OFF
GO