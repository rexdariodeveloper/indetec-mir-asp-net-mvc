ALTER TABLE ArtblInvitacionCompraProveedorCotizacion	ALTER COLUMN CotizacionId UNIQUEIDENTIFIER NULL
GO

SET IDENTITY_INSERT GRtblControlMaestroOrigenArchivo ON
GO
INSERT INTO GRtblControlMaestroOrigenArchivo (
	OrigenArchivoId,
	Descripcion,
	Vigente,
	FechaCreacion,
	CreadoPorId
)
VALUES
(4, 'Invitaci�n Compra Proveedor Cotizaci�n', 1, GETDATE(), 1)
GO
SET IDENTITY_INSERT GRtblControlMaestroOrigenArchivo OFF
GO


SET IDENTITY_INSERT GRtblArchivoEstructuraCarpeta ON
GO
INSERT INTO GRtblArchivoEstructuraCarpeta (
	EstructuraCarpetaId,
	NombreCarpeta,
	DescripcionCorta,
	EstructuraReferenciaId,
	OrigenArchivoId,
	NombreCalculado,
	TipoCalculo,
	Vigente,
	FechaCreacion,
	CreadoPorId
)
VALUES
(8, 'Invitaciones de Compra', 'Carpeta raiz de Invitaciones Compra', NULL, NULL, 0, NULL, 1, GETDATE(), 1),
(9, NULL, 'Carpeta de la Invitaci�n de Compra', 8, NULL, 1, 17, 1, GETDATE(), 1),
(10, NULL, 'Carpeta del Proveedor', 9, NULL, 1, 17, 1, GETDATE(), 1),
(11, 'Cotizaciones', 'Cotizaciones del proveedor', 10, 4, 0, NULL, 1, GETDATE(), 1)
GO
SET IDENTITY_INSERT GRtblArchivoEstructuraCarpeta OFF
GO