//Modales
var modalComentario;
var modalDetalle;
var modalConfirmaDeshacer;
var modalMotivo;

//Botones
var dxButtonCancelar;
var dxButtonDeshacer;
var dxButtonGuardaCambios;

//Forms
var dxForm;
var dxFormModal;
var dxGridDetalles;
var dxGridExistencias;
var dxFormModalComentario;
var dxFormModalMotivo;

//Variables Globales
var rowEditar;
var registrosPorSurtir;

//Variables de Control
var requisicionId;
var contadorRegistrosNuevos;
var ignoraEventos;
var cambios;
var eventoRegresar;

var API_FICHA = "/compras/requisiciones/requisicionporsurtir/";

$(document).ready(function () {
    //Inicializamos las variables para la Ficha
    inicializaVariables();

    //Respaldamos el modelo del Form
    getForm();

    //Deshabilitamos los botones de acciones
    habilitaComponentes();
});

var inicializaVariables = function () {
    modalComentario = $('#modalComentario');
    modalDetalle = $('#modalDetalle');
    modalConfirmaDeshacer = $('#modalConfirmaDeshacer');
    modalMotivo = $('#modalMotivo');

    dxButtonCancelar = $('#dxButtonCancelar').dxButton("instance");
    dxButtonDeshacer = $('#dxButtonDeshacer').dxButton("instance");
    dxButtonGuardaCambios = $('#dxButtonGuardaCambios').dxButton("instance");

    dxForm = $("#dxForm").dxForm("instance");
    dxFormModal = $("#dxFormModal").dxForm("instance");
    dxGridDetalles = $("#dxGridDetalles").dxDataGrid("instance");
    dxGridExistencias = $("#dxGridExistencias").dxDataGrid("instance");
    dxFormModalComentario = $("#dxFormModalComentario").dxForm("instance");
    dxFormModalMotivo = $("#dxFormModalMotivo").dxForm("instance");

    registrosPorSurtir = [];

    contadorRegistrosNuevos = -1;
    ignoraEventos = false;
    cambios = false;
    eventoRegresar = true;
}

var getForm = function () {
    var modelo = $.extend(true, {}, dxForm.option('formData'));
    requisicionId = modelo.RequisicionMaterialId;

    return modelo;
}

var habilitaComponentes = function () {
    //dxButtonCancelar.option("visible", !cambios);
    dxButtonDeshacer.option("disabled", !cambios);
}

var setCambios = function () {
    cambios = true;
    habilitaComponentes();
}

var validaDeshacer = function (regresar) {
    eventoRegresar = regresar;

    if (cambios) {
        //Mostramos el modal de confirmacion
        modalConfirmaDeshacer.modal('show');
    } else {
        cancelar();
    }
}

var cancelar = function () {
    if (eventoRegresar) {
        //Regresamos al listado
        regresarListado();
    } else {
        //Recargamos la ficha
        recargarFicha();
    }
}

var permiteSurtirCantidad = function (estatusId) {
    var AccionSurtirCantidad = [
        ControlMaestroMapeo.AREstatusRequisicionDetalle.POR_SURTIR,
        ControlMaestroMapeo.AREstatusRequisicionDetalle.SURTIDO_PARCIAL,
        ControlMaestroMapeo.AREstatusRequisicionDetalle.EN_ALMACEN
    ]

    return AccionSurtirCantidad.includes(estatusId);
}

var editaCantidadSurtir = function (event) {
    //Obtenemos una copia del objeto a modificar
    var modelo = $.extend(true, {}, event.row.data);

    $("#dxTxtProducto").dxTextBox("instance").option("value", modelo.ProductoId + ' - ' + modelo.Producto);
    $("#dxTxtTipoGasto").dxTextBox("instance").option("value", modelo.TipoGastoId + ' - ' + modelo.TipoGasto);
    $("#dxTxtProyecto").dxTextBox("instance").option("value", modelo.ProyectoId + ' - ' + modelo.Proyecto);
    $("#dxTxtUnidadAdministrativa").dxTextBox("instance").option("value", modelo.UnidadAdministrativaId + ' - ' + modelo.UnidadAdministrativa);

    //Le pasamos el objeto al Form para que cargue sus valores
    dxFormModal.option("formData", modelo);

    //Limpiamos la tabla de existencias
    var dataSource = dxGridExistencias.getDataSource();
    dataSource.store().clear();
    dataSource.reload();

    var filtrarExistencias = [];

    _listExistencias.forEach(m => {
        if (m.AlmacenId == modelo.AlmacenId
            && m.ProductoId == modelo.ProductoId
            && m.UnidadAdministrativaId == modelo.UnidadAdministrativaId
            && m.ProyectoId == modelo.ProyectoId
            && m.TipoGastoId == modelo.TipoGastoId) {

            filtrarExistencias.push(m);
        }
    });

    filtrarExistencias.forEach(m => {
        var encontrado = registrosPorSurtir.find(x => x.AlmacenProductoId == m.AlmacenProductoId);

        if (encontrado) {
            m.CantidadSurtir = encontrado.CantidadSurtir;
        }
    });

    //Agregamos los registros a la tabla
    dxGridExistencias.option("dataSource", filtrarExistencias);

    //Recargamos la informacion de la tabla
    dxGridExistencias.getDataSource().reload();

    //Mostramos el modal
    modalDetalle.modal('show');
}

var editaComentario = function (event) {
    //Obtenemos una copia del objeto a modificar
    var modelo = $.extend(true, {}, event.row.data);

    //Le pasamos el objeto al Form para que cargue sus valores
    dxFormModalComentario.option("formData", modelo);

    //Mostramos el modal
    modalComentario.modal('show');
}

var editaMotivo = function (event) {
    //Obtenemos una copia del objeto a modificar
    var modelo = $.extend(true, {}, rowEditar || event.row.data);

    //Le pasamos el objeto al Form para que cargue sus valores
    dxFormModalMotivo.option("formData", modelo);

    //Mostramos el modal
    modalMotivo.modal('show');
}

var guardaCambiosModal = function () {
    //Obtenemos el Objeto que se esta creando/editando en el Form 
    var modelo = dxFormModal.option("formData");

    var existencias;

    //Obtenemos todos los registros que hay en el dxGridDetalles
    dxGridExistencias.getDataSource().store().load().done((res) => { existencias = res; });

    //Obtenemos la cantidad que se va a surtir
    var cantidad = 0;

    existencias.forEach(m => {
        cantidad += m.CantidadSurtir ? round(m.CantidadSurtir, 4) : 0;
    });

    //Validamos que la cantidad por surtir sea correcta
    if (cantidad <= 0) {
        toast("Favor de completar la Cantidad por Surtir (" + modelo.PorSurtir + ").", 'error');

        return;
    } else if (cantidad > modelo.PorSurtir) {
        toast("La Cantidad por Surtir no puede ser mayor a " + modelo.PorSurtir + ".", 'error');

        return;
    } else {
        //Agregamos la cantidad por surtir al modelo
        modelo.CantidadSurtir = cantidad;
        modelo.Revision = false;
        modelo.PorComprar = false;
        modelo.Motivo = null;

        //Obtenemos la instancia store del DataSource
        var store = dxGridDetalles.getDataSource().store();

        store.update(modelo.RequisicionMaterialDetalleId, modelo)
            .done(function () {
                //Respaldamos los registros a procesar
                existencias.forEach(m => {
                    var cantidadSurtir = m.CantidadSurtir ? round(m.CantidadSurtir, 4) : null;
                    var encontrado = registrosPorSurtir.find(x => x.AlmacenProductoId == m.AlmacenProductoId);

                    if (encontrado) {
                        encontrado.CantidadSurtir = cantidadSurtir;
                    } else {
                        var registro = {
                            AlmacenProductoId: m.AlmacenProductoId,
                            RequisicionMaterialDetalleId: modelo.RequisicionMaterialDetalleId,
                            CantidadSurtir: cantidadSurtir
                        }

                        registrosPorSurtir.push(registro);
                    }
                });

                //Recargamos la informacion de la tabla
                dxGridDetalles.getDataSource().reload();

                setCambios();

                //Ocultamos el modal
                modalDetalle.modal('hide');
            })
            .fail(function () {
                toast("No se pudo actualizar el registro en la tabla.", "error");
            });
    }
}

var guardaCambios = function () {
    //Obtenemos todos los registros que hay en el dxGridDetalles
    var detalles;

    dxGridDetalles.getDataSource().store().load().done((res) => { detalles = res; });

    //Quitamos los registros que se enviarán a Revisión o Por Comprar
    var detallesRevision = [];

    detalles.forEach(m => {
        if (permiteSurtirCantidad(m.EstatusId) && (m.Revision || m.PorComprar)) {
            registrosPorSurtir.forEach(x => {
                if (x.RequisicionMaterialDetalleId == m.RequisicionMaterialDetalleId) {
                    m.CantidadSurtir = null;
                }
            });

            detallesRevision.push(m);
        }
    });

    //Quitamos los requistros que no tengan cantidad por surtir
    var movimientos = [];

    registrosPorSurtir.forEach(m => {
        if (m.CantidadSurtir && m.CantidadSurtir > 0) {
            movimientos.push(m);
        }
    });

    //Validamos que la informacion requerida del Formulario este completa
    if ((!movimientos || !movimientos.length) && (!detallesRevision || !detallesRevision.length)) {
        toast("Favor de agregar artículos para surtir.", 'error');

        return;
    }

    //Mostramos Loader
    dxLoaderPanel.show();

    $.ajax({
        type: "POST",
        url: API_FICHA + "guardaCambios",
        data: { requisicion: getForm(), movimientos: movimientos, detallesRevision: detallesRevision },
        success: function (response) {
            // Ocultamos Loader
            dxLoaderPanel.hide();

            //Mostramos mensaje de Exito
            toast("Registro guardado con exito!", 'success');

            //Mostramos el reporte
            window.open(API_FICHA + "rptSurtidoSolicitud/" + requisicionId);

            //Regresamos al listado
            regresarListado();
        },
        error: function (response, status, error) {
            // Ocultamos Loader
            dxLoaderPanel.hide();

            //Mostramos mensaje de error
            toast("Error al guadar:\n" + response.responseText, 'error');
        }
    });
}

var guardaCambiosMotivo = function () {
    //Obtenemos el Objeto que se esta creando/editando en el Form 
    var modelo = dxFormModalMotivo.option("formData");

    //Validamos que la informacion requerida del Formulario este completa
    if (!dxFormModalMotivo.validate().isValid) {
        toast("Favor de completar los campos requeridos.", 'error');

        return;
    }

    //Obtenemos la instancia store del DataSource
    var store = dxGridDetalles.getDataSource().store();

    store.update(modelo.RequisicionMaterialDetalleId, modelo)
        .done(function () {
            //Recargamos la informacion de la tabla
            dxGridDetalles.getDataSource().reload();

            setCambios();

            rowEditar = null;

            //Ocultamos el modal
            modalMotivo.modal('hide');
        })
        .fail(function () {
            toast("No se pudo actualizar el registro en la tabla.", "error");
        });
}

var round = function (num, scale) {
    if (!("" + num).includes("e")) {
        return +(Math.round(num + "e+" + scale) + "e-" + scale);
    } else {
        var arr = ("" + num).split("e");
        var sig = "";

        if (+arr[1] + scale > 0) {
            sig = "+";
        }

        return +(Math.round(+arr[0] + "e" + sig + (+arr[1] + scale)) + "e-" + scale);
    }
}

var onEditingStart = function (event) {
    if (!permiteSurtirCantidad(event.data.EstatusId)) {
        event.cancel = true;
    }
}

var onDetallesChange = function (e) {
    if (e.name === "editing") {
        var editRowKey = e.component.option("editing.editRowKey"),
            changes = e.component.option("editing.changes");

        changes = changes.map((change) => {
            return {
                type: change.type,
                key: change.type !== "insert" ? change.key : undefined,
                data: change.data
            };
        });

        if (changes && changes.length) {
            setCambios();

            var revision = changes[0].data.Revision;
            var porComprar = changes[0].data.PorComprar;

            //Obtenemos todos los registros que hay en el dxGridDetallesPorComprar
            var detalles;
            dxGridDetalles.getDataSource().store().load().done((res) => { detalles = res; });

            rowEditar = detalles.find(x => x.RequisicionMaterialDetalleId === changes[0].key);

            if (revision || porComprar) {
                rowEditar.Revision = revision === true;
                rowEditar.PorComprar = porComprar === true;
                rowEditar.CantidadSurtir = null;

                registrosPorSurtir.forEach(x => {
                    if (x.RequisicionMaterialDetalleId == rowEditar.RequisicionMaterialDetalleId) {
                        x.CantidadSurtir = null;
                    }
                });

                editaMotivo(null);
            } else {
                rowEditar.Motivo = null;
            }
        }
    }
}

var gridBox_displayExpr = function (item) {
    return item ? item.ProductoId + ' - ' + item.Producto : null;
}

var onCellPrepared = function (e) {
    var tooltip = $('#tooltip').dxTooltip("instance");
    var propiedad = e.column.dataField;
    var MostrarTooltip = ["UnidadAdministrativaId", "ProyectoId", "FuenteFinanciamientoId", "TipoGastoId"];

    if (e.rowType == "data" && MostrarTooltip.includes(propiedad)) {
        e.cellElement.mouseover(function () {
            tooltip.option("contentTemplate", null);

            switch (propiedad) {
                case "UnidadAdministrativaId":
                    tooltip.option("contentTemplate", document.createTextNode(e.data.UnidadAdministrativa));
                    break;

                case "ProyectoId":
                    tooltip.option("contentTemplate", document.createTextNode(e.data.Proyecto));
                    break;

                case "FuenteFinanciamientoId":
                    tooltip.option("contentTemplate", document.createTextNode(e.data.FuenteFinanciamiento));
                    break;

                case "TipoGastoId":
                    tooltip.option("contentTemplate", document.createTextNode(e.data.TipoGasto));
                    break;
            }

            tooltip.option("target", e.cellElement);
            tooltip.show();
        });

        e.cellElement.mouseout(function () {
            tooltip.hide();
        });
    }
}

var regresarListado = function () {
    window.location.href = API_FICHA + "listar";
}

var recargarFicha = function () {
    // Recargamos la ficha según si es registro nuevo o se está editando
    window.location.href = API_FICHA + "editar/" + requisicionId;
}

var toast = function (message, type) {
    DevExpress.ui.notify({ message: message, width: "auto" }, type, 5000);
}