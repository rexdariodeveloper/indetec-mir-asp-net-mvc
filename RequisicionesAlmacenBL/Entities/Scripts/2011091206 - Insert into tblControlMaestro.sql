SET IDENTITY_INSERT tblControlMaestro ON

-- Activo, Inactivo, Borrado, Compra directa y Invitaci�n compra

INSERT INTO tblControlMaestro(
	ControlId,
	Control,
	Valor,
    Sistema,
	Activo,
	ControlSencillo,
    CreadoPorId
) VALUES(
	6, -- ControlId
	'TipoNodoMenu', -- Control
	'M�dulo', -- Valor
	1, -- Sistema
	1, -- Activo
	0, -- ControlSencillo
	1 -- CreadoPorId
), (
	7, -- ControlId
	'TipoNodoMenu', -- Control
	'Ficha', -- Valor
	1, -- Sistema
	1, -- Activo
	0, -- ControlSencillo
	1 -- CreadoPorId
), (
	8, -- ControlId
	'SistemaAcceso', -- Control
	'WEB', -- Valor
	1, -- Sistema
	1, -- Activo
	0, -- ControlSencillo
	1 -- CreadoPorId
)

SET IDENTITY_INSERT tblControlMaestro OFF


