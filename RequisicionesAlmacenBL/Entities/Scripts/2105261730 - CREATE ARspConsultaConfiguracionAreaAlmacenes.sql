SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[ARspConsultaConfiguracionAreaAlmacenes]
	@configuracionAreaId INT
AS
/* ****************************************************************
 * ARspConsultaConfiguracionAreaAlmacenes
 * ****************************************************************
 * Descripci�n: Procedimiento para Obtener los registros que se van a
 *						 agregar a los Almacenes de la Ficha Configuraci�n de �reas.
 *
 * autor: 	Javier El�as
 * Fecha: 	26.05.2021
 * Versi�n: 1.0.0
 *****************************************************************
 * PARAMETROS DE ENTRADA: ConfiguracionAreaId
 * PARAMETROS DE SALIDA:
 *****************************************************************
*/ 
SELECT almacen.AlmacenId,
       almacen.Nombre,
       CAST(CASE WHEN ConfiguracionAreaAlmacenId IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS Seleccionado
FROM tblAlmacen AS almacen
     LEFT JOIN ARtblControlMaestroConfiguracionAreaAlmacen AS configuracionAlmacen ON almacen.AlmacenId = configuracionAlmacen.AlmacenId
                                                                                      AND ConfiguracionAreaId = @configuracionAreaId
                                                                                      AND Borrado = 0
ORDER BY almacen.Nombre