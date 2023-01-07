SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ****************************************************************
 * RHspConsultaEmpleadoLigado
 * ****************************************************************
 * Descripción: Consulta para obtener el empelado cuando hay ligado por 1 empleado
 * Autor: Rene Carrillo
 * Fecha: 04.11.2021
 * Fecha de Modificación: 24.01.2022
 * Versión: 1.0.1
 *****************************************************************
 * PARAMETROS DE ENTRADA: empleado.*
 * PARAMETROS DE SALIDA: *
 *****************************************************************
*/

ALTER PROCEDURE [dbo].[RHspConsultaEmpleadoLigado]
	@UsuarioId INT
AS
SELECT * 
FROM (SELECT empleado.*
		FROM RHtblEmpleado empleado
			LEFT JOIN GRtblUsuario usuario ON empleado.EmpleadoId = usuario.EmpleadoId AND usuario.Activo = 1
		WHERE ISNULL(empleado.EmpleadoId, 0) != ISNULL(usuario.EmpleadoId, 0) AND empleado.EstatusId = 1
		UNION ALL
		SELECT empleado.*
		FROM RHtblEmpleado empleado
			INNER JOIN GRtblUsuario usuario ON empleado.EmpleadoId = usuario.EmpleadoId AND usuario.Activo = 1
		WHERE usuario.UsuarioId = @UsuarioId AND empleado.EstatusId = 1) AS Empleado
ORDER BY Empleado.EmpleadoId ASC