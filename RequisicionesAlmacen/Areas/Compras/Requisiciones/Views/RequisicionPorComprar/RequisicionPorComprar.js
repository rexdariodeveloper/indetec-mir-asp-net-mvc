//Modales
var modalComentario;
var modalMotivo;
var modalMotivoVer;
var modalConfirmaDeshacer;

//Botones
var dxButtonProductos;
var dxButtonCompras;
var dxButtonDeshacer;
var dxButtonGuardaCambios;

//Forms
var dxForm;
var dxFormModalComentario;
var dxFormModalMotivo;
var dxFormModalMotivoVer;
var dxGridDetallesPorComprar;

//Variables Globales
var rowEditar;
var registrosDatosTemp;

//Variables de Control
var requisicionId;
var ignoraEventos;
var cambios;
var eventoRegresar;
var seleccionListIndex;

//Drawer
var dxMultiview;
var dxDrawer;
var dxListDrawer;

//Tabs
var tabOrdenesCompra;
var tabInvitacionesCompra;
var tabRevision;
var tabRechazar;

var API_FICHA = "/compras/requisiciones/requisicionporcomprar/";

$(document).ready(function () {
    //Inicializamos las variables para la Ficha
    inicializaVariables();

    //Deshabilitamos los botones de acciones
    habilitaComponentes();
});

var inicializaVariables = function () {
    modalComentario = $('#modalComentario');
    modalMotivo = $('#modalMotivo');
    modalMotivoVer = $('#modalMotivoVer');
    modalConfirmaDeshacer = $('#modalConfirmaDeshacer');

    dxButtonProductos = $('#dxButtonProductos').dxButton("instance");
    dxButtonCompras = $('#dxButtonCompras').dxButton("instance");
    dxButtonDeshacer = $('#dxButtonDeshacer').dxButton("instance");
    dxButtonGuardaCambios = $('#dxButtonGuardaCambios').dxButton("instance");

    dxForm = $("#dxForm").dxForm("instance");
    dxFormModalComentario = $("#dxFormModalComentario").dxForm("instance");
    dxFormModalMotivo = $("#dxFormModalMotivo").dxForm("instance");
    dxFormModalMotivoVer = $("#dxFormModalMotivoVer").dxForm("instance");
    dxGridDetallesPorComprar = $("#dxGridDetallesPorComprar").dxDataGrid("instance");

    registrosDatosTemp = [];

    ignoraEventos = false;
    cambios = false;
    eventoRegresar = true;
    seleccionListIndex = 0;

    //Eliminamos una clase de style en template content
    $('#templateContent').removeClass('d-none');

    dxMultiview = $('#dxMultiview').dxMultiView('instance');
    dxDrawer = $('#dxDrawer').dxDrawer('instance');
    dxListDrawer = $('#dxListDrawer').dxList('instance');

    tabOrdenesCompra = $("#tabOrdenesCompra");
    tabInvitacionesCompra = $("#tabInvitacionesCompra");
    tabRevision = $("#tabRevision");
    tabRechazar = $("#tabRechazar");

    dxButtonProductos.option("visible", false);
    dxButtonGuardaCambios.option("visible", false);
}

var habilitaComponentes = function () {
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

var verMotivo = function (event) {
    //Obtenemos una copia del objeto a modificar
    var modelo = $.extend(true, {}, rowEditar || event.row.data);

    //Le pasamos el objeto al Form para que cargue sus valores
    dxFormModalMotivoVer.option("formData", modelo);

    //Mostramos el modal
    modalMotivoVer.modal('show');
}

var guardaCambios = function () {
    //Obtenemos la instancia del Grid
    var dxGridOrdenesCompra = $("#dxGridOrdenesCompra").dxDataGrid("instance");
    var dxGridInvitacionesCompra = $("#dxGridInvitacionesCompra").dxDataGrid("instance");

    var ordenesCompra;
    dxGridOrdenesCompra.getDataSource().store().load().done((res) => { ordenesCompra = res; });

    var invitacionesCompra;
    dxGridInvitacionesCompra.getDataSource().store().load().done((res) => { invitacionesCompra = res; });

    //Validamos que las OC tengan fecha de recepción
    var valido = true;

    ordenesCompra.forEach(m => {
        if (valido && !m.FechaRecepcion) {
            toast("Favor de seleccionar la Fecha de Recepción para el Proveedor [ " + m.Proveedor + " ]" + " y el Almacén [ " + m.Almacen + " ].", 'error');

            valido = false;
        }
    });

    if (!valido) {
        mostrarOrdenesCompra(); return;
    }

    //Obtenemos los registros que se enviarán a Revisión o Por Comprar
    var detallesRevision = [];

    //Obtenemos todos los registros que hay en el dxGridDetallesPorComprar
    var detalles;
    dxGridDetallesPorComprar.getDataSource().store().load().done((res) => { detalles = res; });

    detalles.forEach(m => {
        if (m.Revision || m.Rechazar) {
            detallesRevision.push(m);
        }
    });

    //Validamos que la informacion requerida del Formulario este completa
    if ((!ordenesCompra || !ordenesCompra.length)
        && (!invitacionesCompra || !invitacionesCompra.length)
        && (!detallesRevision || !detallesRevision.length)) {

        toast("Favor de agregar artículos para comprar.", 'error');

        return;
    }

    ordenesCompra.forEach(x => {
        x.FechaRecepcion = x.FechaRecepcion.toLocaleString();
    });

    //Mostramos Loader
    dxLoaderPanel.show();

    $.ajax({
        type: "POST",
        url: API_FICHA + "guardaCambios",
        data: { ordenesCompra: ordenesCompra, invitacionesCompra: invitacionesCompra, detallesRevision: detallesRevision },
        success: function (response) {
            //Mostramos mensaje de Exito
            toast("Registro guardado con exito!", 'success');

            //Recargamos la ficha
            recargarFicha();
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
    var store = dxGridDetallesPorComprar.getDataSource().store();

    store.update(modelo.RequisicionMaterialDetalleId, modelo)
        .done(function () {
            //Recargamos la informacion de la tabla
            dxGridDetallesPorComprar.getDataSource().reload();

            setCambios();

            rowEditar = null;

            //Ocultamos el modal
            modalMotivo.modal('hide');
        })
        .fail(function () {
            toast("No se pudo actualizar el registro en la tabla.", "error");
        });    
}

var onEditingStart = function (event) {
    if (event.data.EstatusId != ControlMaestroMapeo.AREstatusRequisicionDetalle.POR_COMPRAR) {
        event.cancel = true;
    }
}

var onDetallesChange = function (e) {
    if (e.name === "editing") {
        var editRowKey = e.component.option("editing.editRowKey");
        var changes = e.component.option("editing.changes");

        changes = changes.map((change) => {
            return {
                type: change.type,
                key: change.type !== "insert" ? change.key : undefined,
                data: change.data
            };
        });

        if (changes && changes.length) {
            setCambios();

            var cambios = changes[0].data;
            var propiedad = Object.getOwnPropertyNames(cambios)[0];

            var revision = cambios.Revision;
            var comprar = cambios.Comprar;
            var rechazar = cambios.Rechazar;

            //Obtenemos todos los registros que hay en el dxGridDetallesPorComprar
            var detalles;
            dxGridDetallesPorComprar.getDataSource().store().load().done((res) => { detalles = res; });

            rowEditar = detalles.find(x => x.RequisicionMaterialDetalleId === changes[0].key);

            if (propiedad == "TarifaImpuestoId") {
                rowEditar.TarifaImpuestoId = cambios.TarifaImpuestoId;

                calculaTotalDetalle();
            } else if (propiedad == "CantidadComprar") {
                rowEditar.CantidadComprar = cambios.CantidadComprar === null ? null : round(cambios.CantidadComprar, 4);

                calculaTotalDetalle();
            } else if (propiedad == "Ajuste") {
                rowEditar.Ajuste = cambios.Ajuste === null ? null : round(cambios.Ajuste, 4);

                calculaTotalDetalle();
            } else if (propiedad == "IEPS") {
                rowEditar.IEPS = cambios.IEPS === null ? null : round(cambios.IEPS, 2);

                calculaTotalDetalle();
            } else if (propiedad == "FuenteFinanciamientoId") {
                rowEditar.FuenteFinanciamientoId = cambios.FuenteFinanciamientoId;

                var cuentaPresupuestal = _listFuentesFinanciamiento.find(x =>
                    x.ProductoId == rowEditar.ProductoId &&
                    x.AlmacenId == rowEditar.AlmacenId &&
                    x.UnidadAdministrativaId == rowEditar.UnidadAdministrativaId &&
                    x.ProyectoId == rowEditar.ProyectoId &&
                    x.TipoGastoId == rowEditar.TipoGastoId &&
                    x.RamoId == rowEditar.FuenteFinanciamientoId
                );

                rowEditar.CuentaPresupuestalEgrId = cuentaPresupuestal ? cuentaPresupuestal.CuentaPresupuestalEgrId : null;

                calculaTotalDetalle();
            } else if (propiedad == "ProveedorId") {
                rowEditar.ProveedorId = cambios.ProveedorId;

                calculaTotalDetalle();
            } else if (revision || rechazar) {
                rowEditar.TarifaImpuestoId = null;
                rowEditar.CantidadComprar = null;
                rowEditar.IEPS = null;
                rowEditar.Total = null;
                rowEditar.Importe = null;
                rowEditar.Ajuste = null;
                rowEditar.FuenteFinanciamientoId = null;
                rowEditar.ProveedorId = false;

                rowEditar.Revision = revision === true;
                rowEditar.Comprar = false;
                rowEditar.Rechazar = rechazar === true;

                editaMotivo(null);
            } else if (comprar) {
                rowEditar.Revision = false;
                rowEditar.Rechazar = false;
                rowEditar.Motivo = null;
            } else if (comprar === false) {
                rowEditar.TarifaImpuestoId = null;
                rowEditar.CantidadComprar = null;
                rowEditar.IEPS = null;
                rowEditar.Total = null;
                rowEditar.Importe = null;
                rowEditar.Ajuste = null;
                rowEditar.FuenteFinanciamientoId = null;
                rowEditar.ProveedorId = false;
            } else if (revision === false || rechazar === false) {
                rowEditar.Motivo = null;
            } else {
                calculaTotalDetalle();
            }
        }
    }
}

var calculaTotalDetalle = function () {
    var cantidad = rowEditar.CantidadComprar ? round(rowEditar.CantidadComprar, 4) : 0;
    var costo = rowEditar.CostoUnitario ? round(rowEditar.CostoUnitario, 4) : 0;
    var importe = trunc(cantidad * costo, 4);
    var ieps = rowEditar.IEPS ? round(rowEditar.IEPS, 2) : 0;
    var ajuste = rowEditar.Ajuste ? round(rowEditar.Ajuste, 4) : 0;

    var iva = 0;
    var ish = 0;
    var retencionISR = 0;
    var retencionCedular = 0;
    var retencionIVA = 0;
    var totalPresupuesto = 0;
    var total = 0;

    var proveedor = rowEditar.ProveedorId ? _listProveedores.find(x => x.ProveedorId == rowEditar.ProveedorId) : null;
    var tipoComprobanteFiscalId = proveedor ? proveedor.TipoComprobanteFiscalId : -1;
    var tarifaImpuesto = rowEditar.TarifaImpuestoId ? _listTarifasImpuesto.find(x => x.TarifaImpuestoId == rowEditar.TarifaImpuestoId) : null;

    if (tipoComprobanteFiscalId > 0 && tarifaImpuesto != null) {
        iva = tarifaImpuesto.Porcentaje > 0 ? round(((importe - (tarifaImpuesto.AplicaIvaAlIEPS === false ? ieps : 0)) * tarifaImpuesto.Porcentaje), 2) : 0;
        ish = tarifaImpuesto.porcISH > 0 ? round((importe) * tarifaImpuesto.porcISH, 2) : 0;
        retencionISR = tarifaImpuesto.PorcRetencionISR > 0 && (tipoComprobanteFiscalId == 2 || tipoComprobanteFiscalId == 3) ? round(importe * tarifaImpuesto.PorcRetencionISR, 2) : 0;
        retencionCedular = tarifaImpuesto.porcRetencionCedular > 0 && (tipoComprobanteFiscalId == 2 || tipoComprobanteFiscalId == 3) ? round(importe * tarifaImpuesto.porcRetencionCedular, 2) : 0;
        retencionIVA = tarifaImpuesto.PorcRetencionIVA > 0 && (tipoComprobanteFiscalId == 2 || tipoComprobanteFiscalId == 3) ? round(tarifaImpuesto.porcRetencionIVA > 0 && (tipoComprobanteFiscalId == 2 || tipoComprobanteFiscalId == 3) ? round((tarifaImpuesto.Porcentaje > 0 ? round((importe * tarifaImpuesto.Porcentaje) - ieps, 2) : 0) * tarifaImpuesto.porcRetencionIVA, 2) : 0, 2) : 0;
        totalPresupuesto = round(importe, 2) + (tarifaImpuesto.Porcentaje > 0 ? round(((importe - (tarifaImpuesto.AplicaIvaAlIEPS === true ? ieps : 0)) * tarifaImpuesto.Porcentaje), 2) : 0) + (tarifaImpuesto.porcISH > 0 ? round(importe * tarifaImpuesto.porcISH, 2) : 0) + (ajuste);

        if (tipoComprobanteFiscalId == 1 || tipoComprobanteFiscalId == 4 || tipoComprobanteFiscalId == 5) {
            total = round(importe, 2) + (tarifaImpuesto.Porcentaje > 0 ? round(((importe - (tarifaImpuesto.AplicaIvaAlIEPS === false ? ieps : 0)) * tarifaImpuesto.Porcentaje), 2) : 0) + (tarifaImpuesto.porcISH > 0 ? round(importe * tarifaImpuesto.porcISH, 2) : 0) + (ajuste);
        } else if (tipoComprobanteFiscalId == 2 || tipoComprobanteFiscalId == 3) {
            total = round(importe, 2) + (tarifaImpuesto.Porcentaje > 0 ? round(((importe - (tarifaImpuesto.AplicaIvaAlIEPS === false ? ieps : 0)) * tarifaImpuesto.Porcentaje), 2) : 0) - (tarifaImpuesto.porcRetencionIVA > 0 ? round((round((importe * tarifaImpuesto.Porcentaje) - ieps, 2)) * tarifaImpuesto.porcRetencionIVA, 2) : 0) - (tarifaImpuesto.PorcRetencionISR > 0 ? round(importe * tarifaImpuesto.PorcRetencionISR, 2) : 0) - (tarifaImpuesto.porcRetencionCedular > 0 ? round(importe * tarifaImpuesto.porcRetencionCedular, 2) : 0) + (ajuste);
        } else {
            total = round(importe, 2) + (ajuste);
        }
    } else {
        total = round(importe, 2) + (ajuste);
    }

    //Datos Orden e Invitación de Compra Detalle
    rowEditar.OrdenCompraDetId = 0;
    rowEditar.InvitacionCompraDetalleId = 0;
        
    rowEditar.Importe = importe;
    rowEditar.IVA = round(iva, 2);
    rowEditar.ISH = round(ish, 2);
    rowEditar.RetencionISR = round(retencionISR, 2);
    rowEditar.RetencionCedular = round(retencionCedular, 2);
    rowEditar.RetencionIVA = round(retencionIVA, 2);
    rowEditar.TotalPresupuesto = round(totalPresupuesto, 4);
    rowEditar.Total = round(total, 4);

    //Checkbok
    rowEditar.Revision = false;
    rowEditar.Comprar = true;
    rowEditar.Rechazar = false;

    rowEditar.Motivo = null;
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

var trunc = function (num, pos) {
    var s = num.toString();
    var l = s.length;
    var decimalLength = s.indexOf('.') + 1;

    if (l - decimalLength <= pos) {
        return num;
    }

    // Parte decimal del número
    var isNeg = num < 0;
    var decimal = num % 1;
    var entera = isNeg ? Math.ceil(num) : Math.floor(num);

    // Parte decimal como número entero
    var decimalFormated = Math.floor(
        Math.abs(decimal) * Math.pow(10, pos)
    )

    // Sustraemos del número original la parte decimal
    // y le sumamos la parte decimal que hemos formateado
    var finalNum = entera + ((decimalFormated / Math.pow(10, pos)) * (isNeg ? -1 : 1));

    return finalNum;
}

var filtrarFuentesFinanciamiento = function (event) {
    var dataFilter = [];

    _listFuentesFinanciamiento.forEach(x => {
        var registro = event.data;

        if (registro) {
            if (x.ProductoId === registro.ProductoId
                && x.AlmacenId === registro.AlmacenId
                && x.UnidadAdministrativaId === registro.UnidadAdministrativaId
                && x.ProyectoId === registro.ProyectoId
                && x.TipoGastoId === registro.TipoGastoId) {
                dataFilter.push(x);
            }
        } else {
            dataFilter.push(x);
        }
    });

    var dataSource = new DevExpress.data.DataSource({
        store: {
            type: "array",
            key: "RamoId",
            data: dataFilter
        },
    });

    return dataSource.store();
}

var toolbar_preparing = function (e) {
    var dataGrid = e.component;

    e.toolbarOptions.items.unshift({
        location: "before",
        template: function () {
            return $("<div/>")
                .addClass("informer")
                .append(
                    $("<h2 />")
                        .addClass("count")
                        .text(getGroupCount("Solicitud")),
                    $("<span />")
                        .addClass("name")
                        .text("Total")
                );
        }
    }, {
        location: "before",
        widget: "dxSelectBox",
        options: {
            width: 200,
            items: [{
                value: "Solicitud",
                text: "Agrupado por Solicitud"
            }, {
                value: "Producto",
                text: "Agrupado por Producto"
            }],
            displayExpr: "text",
            valueExpr: "value",
            value: "Solicitud",
            onValueChanged: function (e) {
                dataGrid.clearGrouping();
                dataGrid.columnOption(e.value, "groupIndex", 0);
                $(".informer .count").text(getGroupCount(e.value));
            }
        }
    }, {
        location: "before",
        widget: "dxButton",
        options: {
            text: "Contraer",
            width: 136,
            onClick: function (e) {
                var expanding = e.component.option("text") === "Expandir";
                dataGrid.option("grouping.autoExpandAll", expanding);
                e.component.option("text", expanding ? "Contraer" : "Expandir");
            }
        }
    }, {
        location: "after",
        widget: "dxButton",
        options: {
            icon: "refresh",
            onClick: function () {
                dataGrid.refresh();
            }
        }
    });
}

var getGroupCount = function (groupField) {
    var detalles = [];

    $("#dxGridDetallesPorComprar").dxDataGrid("instance")
        .option("dataSource")
        .store
        .load()
        .done(function (data) {
            detalles = data;
        });

    return DevExpress.data.query(detalles)
        .groupBy(groupField)
        .toArray().length;
}

var invitar = function (event) {
    //Obtenemos una copia del objeto a eliminar
    registroEditar = $.extend(true, {}, event.row.data);

    //Actualizamos las columnas de control
    registroEditar.MovidoPorUsuario = true;

    onInvitarChange(true, registroEditar);
}

var ordenCompra = function (event) {
    //Obtenemos una copia del objeto a eliminar
    registroEditar = $.extend(true, {}, event.row.data);

    //Actualizamos las columnas de control
    registroEditar.MovidoPorUsuario = false;

    onInvitarChange(false, registroEditar);
}

var onInvitarChange = function (invitar, registroEditar) {
    //Obtenemos la instancia del Grid
    var dxGridOrdenesCompra = $("#dxGridOrdenesCompra").dxDataGrid("instance");
    var dxGridInvitacionesCompra = $("#dxGridInvitacionesCompra").dxDataGrid("instance");

    //Obtenemos la instancia store del DataSource
    var storeOC = dxGridOrdenesCompra.getDataSource().store();
    var storeIC = dxGridInvitacionesCompra.getDataSource().store();

    //Actualizamos el registro
    if (invitar) {
        storeIC.insert(registroEditar)
            .done(function () {
                //Recargamos la informacion de la tabla
                dxGridInvitacionesCompra.getDataSource().reload();

                storeOC.remove(registroEditar.OrdenCompraId)
                    .done(function () {
                        //Recargamos la informacion de la tabla
                        dxGridOrdenesCompra.getDataSource().reload();
                    })
                    .fail(function () {
                        mensaje = "Error:\n No se pudieron agregar los registros a la tabla.";
                    })
            })
            .fail(function () {
                mensaje = "Error:\n No se pudieron agregar los registros a la tabla.";
            })
    } else {
        storeOC.insert(registroEditar)
            .done(function () {
                //Recargamos la informacion de la tabla
                dxGridOrdenesCompra.getDataSource().reload();

                storeIC.remove(registroEditar.InvitacionCompraId)
                    .done(function () {
                        //Recargamos la informacion de la tabla
                        dxGridInvitacionesCompra.getDataSource().reload();
                    })
                    .fail(function () {
                        mensaje = "Error:\n No se pudieron agregar los registros a la tabla.";
                    })
            })
            .fail(function () {
                mensaje = "Error:\n No se pudieron agregar los registros a la tabla.";
            })
    }
}

var construirOrdenesCompra = function () {
    //Seleccionamos la vista de las Ordenes de Compra
    dxMultiview.option("selectedIndex", 1);

    dxButtonCompras.option("visible", false);
    dxButtonProductos.option("visible", true);
    dxButtonGuardaCambios.option("visible", true);

    //Obtenemos todos los registros que hay en el dxGridDetallesPorComprar
    var detalles;
    dxGridDetallesPorComprar.getDataSource().store().load().done((res) => { detalles = res; });

    //Limpiamos las Ordenes de Compra
    var ordenesCompra = [];
    var invitacionesCompra = [];
    var revisiones = [];
    var rechazados = [];

    //Inicializamos las variables de control
    var contadorRegistros = -1;
    var mensaje = "";
    var registrosTemp = [];

    //Obtenemos la instancia del Grid Revisar
    var dxGridDetallesPorRevisar = $("#dxGridDetallesPorRevisar").dxDataGrid("instance");

    //Limpiamos el Source
    var dataSourceRevisar = dxGridDetallesPorRevisar.getDataSource();
    dataSourceRevisar.store().clear();
    dataSourceRevisar.reload();

    //Obtenemos la instancia del Grid Rechazar
    var dxGridDetallesPorRechazar = $("#dxGridDetallesPorRechazar").dxDataGrid("instance");

    //Limpiamos el Source
    var dataSourceRechazar = dxGridDetallesPorRechazar.getDataSource();
    dataSourceRechazar.store().clear();
    dataSourceRechazar.reload();

    detalles.forEach(m => {
        if (!mensaje && m.Comprar) {
            var mensajeRegistro = " para " + "la Solicitud [ " + m.Solicitud + " ]" + " y el Producto [ " + m.Descripcion + " ].";

            if (!m.CantidadComprar) {
                mensaje = "Favor de completar la Cantidad por Comprar" + mensajeRegistro;
            } else if (m.Ajuste === null) {
                mensaje = "Favor de completar el Ajuste" + mensajeRegistro;
            } else if (m.IEPS === null) {
                mensaje = "Favor de completar el IEPS" + mensajeRegistro;
            } else if (!m.TarifaImpuestoId) {
                mensaje = "Favor de seleccionar el Impuesto" + mensajeRegistro;
            } else if (!m.FuenteFinanciamientoId) {
                mensaje = "Favor de seleccionar la Fuente de Financiamiento" + mensajeRegistro;
            } else if (!m.ProveedorId) {
                mensaje = "Favor de seleccionar el Proveedor" + mensajeRegistro;
            } else {
                var registro = registrosTemp.find(x => x.ProveedorId == m.ProveedorId && x.AlmacenId == m.AlmacenId);

                m.OrdenCompraId = registro ? registro.OrdenCompraId : contadorRegistros;
                m.InvitacionCompraId = registro ? registro.InvitacionCompraId : contadorRegistros;

                if (registro) {
                    registro.Monto += round(m.Total, 2);
                    registro.Detalles.push(m);
                } else {
                    var proveedor = _listProveedores.find(x => x.ProveedorId == m.ProveedorId);

                    var temp = registrosDatosTemp.find(x => x.ProveedorId == m.ProveedorId && x.AlmacenId == m.AlmacenId);

                    var registroNuevo = {
                        OrdenCompraId: contadorRegistros,
                        InvitacionCompraId: contadorRegistros,
                        ProveedorId: m.ProveedorId,
                        Proveedor: (proveedor ? proveedor.RazonSocial : null),
                        AlmacenId: m.AlmacenId,
                        Almacen: m.Almacen,
                        TipoOperacionId: (proveedor ? proveedor.TipoOperacionId : null),
                        TipoComprobanteFiscalId: (proveedor ? proveedor.TipoComprobanteFiscalId : null),
                        Ejercicio: new Date().getFullYear(),
                        Fecha: new Date().toLocaleString(),
                        FechaRecepcion: (temp ? temp.FechaRecepcion : null),
                        Referencia: (temp ? temp.Referencia : null),
                        Observacion: (temp ? temp.Observacion : null),
                        GastoPorComprobarId: null,
                        Monto: round(m.Total, 2),
                        Detalles: [m],
                        MovidoPorUsuario: (temp ? temp.MovidoPorUsuario : false)
                    };

                    registrosTemp.push(registroNuevo);
                    contadorRegistros -= 1;
                }
            }
        } else if (!mensaje && m.Revision) {
            //Obtenemos la instancia store del DataSource
            var store = dxGridDetallesPorRevisar.getDataSource().store();

            store.insert(m)
                .done(function () {
                    //Recargamos la informacion de la tabla
                    dxGridDetallesPorRevisar.getDataSource().reload();

                    //Agregamos el modelo
                    revisiones.push(m);
                })
                .fail(function () {
                    mensaje = "Error:\n No se pudieron agregar los registros a la tabla.";
                })
        } else if (!mensaje && m.Rechazar) {
            //Obtenemos la instancia store del DataSource
            var store = dxGridDetallesPorRechazar.getDataSource().store();

            store.insert(m)
                .done(function () {
                    //Recargamos la informacion de la tabla
                    dxGridDetallesPorRechazar.getDataSource().reload();

                    //Agregamos el modelo
                    rechazados.push(m);
                })
                .fail(function () {
                    mensaje = "Error:\n No se pudieron agregar los registros a la tabla.";
                })
        }
    });

    if (!mensaje && registrosTemp.length) {
        //Obtenemos la Configuración de los Monto de Compra
        var configuracionCompraDirecta = _listMontosCompra.find(x => x.TipoCompraId == ControlMaestroMapeo.TipoCompra.COMPRA_DIRECTA);
        var configuracionInvitacionCompra = _listMontosCompra.find(x => x.TipoCompraId == ControlMaestroMapeo.TipoCompra.INVITACION_COMPRA);

        if (!configuracionCompraDirecta) {
            toast("Error:\n No existe la Configuración para Compra Directa. Favor de revisar.", "error");

            return false;
        } else if (!configuracionInvitacionCompra) {
            toast("Error:\n No existe la Configuración para Invitación de Compra. Favor de revisar.", "error");

            return false;
        }        

        //Obtenemos la instancia del Grid
        var dxGridOrdenesCompra = $("#dxGridOrdenesCompra").dxDataGrid("instance");
        var dxGridInvitacionesCompra = $("#dxGridInvitacionesCompra").dxDataGrid("instance");

        //Limpiamos las Ordenes de Compra
        var dataSourceOC = dxGridOrdenesCompra.getDataSource();
        dataSourceOC.store().clear();
        dataSourceOC.reload();

        //Limpiamos las Invitaciones de Compra
        var dataSourceIC = dxGridInvitacionesCompra.getDataSource();
        dataSourceIC.store().clear();
        dataSourceIC.reload();

        //Obtenemos la instancia store del DataSource
        var storeOC = dxGridOrdenesCompra.getDataSource().store();
        var storeIC = dxGridInvitacionesCompra.getDataSource().store();

        //Agregamos las Ordenes de Compra
        registrosTemp.forEach(m => {
            if (!mensaje) {
                var esMontoOC = m.Monto >= configuracionCompraDirecta.MontoMinimo
                    && (!configuracionCompraDirecta.MontoMaximo || m.Monto <= configuracionCompraDirecta.MontoMaximo);

                var esMontoInvitacion = m.Monto >= configuracionInvitacionCompra.MontoMinimo
                    && (!configuracionInvitacionCompra.MontoMaximo || m.Monto <= configuracionInvitacionCompra.MontoMaximo);

                if (esMontoOC && !m.MovidoPorUsuario) {
                    //Actualizamos las columnas de control
                    m.MovidoPorUsuario = false;
                    m.PermiteInvitar = true;

                    storeOC.insert(m)
                        .done(function () {
                            //Recargamos la informacion de la tabla
                            dxGridOrdenesCompra.getDataSource().reload();                            

                            //Agregamos el modelo a las Ordenes de Compra
                            ordenesCompra.push(m);
                        })
                        .fail(function () {
                            mensaje = "Error:\n No se pudieron agregar los registros a la tabla.";
                        })
                } else if (esMontoInvitacion || m.MovidoPorUsuario) {
                    //Actualizamos las columnas de control
                    m.PermiteInvitar = esMontoOC;

                    storeIC.insert(m)
                        .done(function () {
                            //Recargamos la informacion de la tabla
                            dxGridInvitacionesCompra.getDataSource().reload();                            

                            //Agregamos el modelo a las Invitaciones de Compra
                            invitacionesCompra.push(m);
                        })
                        .fail(function () {
                            mensaje = "Error:\n No se pudieron agregar los registros a la tabla.";
                        })
                } else {
                    mensaje = "Error:\n No existe Configuración adecuada par los Montos de Compra. Favor de revisar.";
                }
            }
        });

        if (mensaje) {
            toast(mensaje, "error");

            //Limpiamos las Ordenes de Compra
            ordenesCompra = [];
            invitacionesCompra = [];

            //Regresamos a la vista de los detalles
            regresarDetalles();

            return false;
        }        
    } else if (mensaje) {
        toast(mensaje, 'error');

        //Limpiamos las Ordenes de Compra
        ordenesCompra = [];
        invitacionesCompra = [];

        //Regresamos a la vista de los detalles
        regresarDetalles();

        return false;
    }

    if (dxMultiview.option("selectedIndex") === 1) {
        if (ordenesCompra.length) {
            mostrarOrdenesCompra();
        } else if (invitacionesCompra.length) {
            mostrarInvitacionesCompra();
        } else if (revisiones.length) {
            mostrarRevision();
        } else if (rechazados.length) {
            mostrarRechazar();
        } else {
            toast("No hay registros para construir el resumen.", 'warning');

            //Limpiamos las Ordenes de Compra
            ordenesCompra = [];
            invitacionesCompra = [];

            //Regresamos a la vista de los detalles
            regresarDetalles();

            return false;
        }
    }

    return true;
}

var mostrarOrdenesCompra = function () {
    seleccionListIndex = 1;

    siguientePestania();
}

var mostrarInvitacionesCompra = function () {
    seleccionListIndex = 2;

    siguientePestania();
}

var mostrarRevision = function () {
    seleccionListIndex = 3;

    siguientePestania();
}

var mostrarRechazar = function () {
    seleccionListIndex = 4;

    siguientePestania();
}

var regresarDetalles = function () {
    //Obtenemos la instancia del Grid
    guardarCopiaGrids();

    dxMultiview.option("selectedIndex", 0);

    dxButtonCompras.option("visible", true);
    dxButtonProductos.option("visible", false);
    dxButtonGuardaCambios.option("visible", false);
}

var guardarCopiaGrids = function () {
    //Obtenemos la instancia del Grid
    var dxGridOrdenesCompra = $("#dxGridOrdenesCompra").dxDataGrid("instance");
    var dxGridInvitacionesCompra = $("#dxGridInvitacionesCompra").dxDataGrid("instance");

    //Guardamos una copia de las Ordenes de Compra
    registrosDatosTemp = [];
    dxGridOrdenesCompra.getDataSource().store().load().done((res) => { registrosDatosTemp = res; });

    //Guardamos una copia de las Invitaciones de Compra
    var invitacionesCompraTemp = [];
    dxGridInvitacionesCompra.getDataSource().store().load().done((res) => { invitacionesCompraTemp = res; });

    //Agregamos las Invitaciones de Compra a las Ordenes de Compra
    registrosDatosTemp = registrosDatosTemp.concat(invitacionesCompraTemp);
}

var multiView_selectionChanged = function () {
    //Eliminamos una clase de style en template content
    $('#templateContent').removeClass('d-none');

    dxDrawer = $('#dxDrawer').dxDrawer('instance');
    dxListDrawer = $('#dxListDrawer').dxList('instance');

    tabOrdenesCompra = $("#tabOrdenesCompra");
    tabInvitacionesCompra = $("#tabInvitacionesCompra");
    tabRevision = $("#tabRevision");
    tabRechazar = $("#tabRechazar");

    mostrarOrdenesCompra();
}

var onClickToolbar = function () {
    dxDrawer.toggle();
}

var onItemClickDrawer = function (event) {
    seleccionListIndex = event.itemData.index;

    siguientePestania();
}

var siguientePestania = function () {
    // Ocultamos las pestañas
    tabOrdenesCompra.hide();
    tabInvitacionesCompra.hide();
    tabRevision.hide();
    tabRechazar.hide();

    switch (seleccionListIndex) {
        // Ordenes de Compra
        case 1:
            tabOrdenesCompra.show();
            dxListDrawer.option('selectedItemKeys', [1]);
        break;

        // Invitaciones de Compra
        case 2:
            tabInvitacionesCompra.show();
            dxListDrawer.option('selectedItemKeys', [2]);
        break;

        // Invitaciones de Compra
        case 3:
            tabRevision.show();
            dxListDrawer.option('selectedItemKeys', [3]);
        break;

        // Invitaciones de Compra
        case 4:
            tabRechazar.show();
            dxListDrawer.option('selectedItemKeys', [4]);
        break;

        default:
            tabOrdenesCompra.show();
            dxListDrawer.option('selectedItemKeys', [1]);
        break;
    }
}

var regresarListado = function () {
    window.location.href = API_FICHA + "listar";
}

var recargarFicha = function () {
    // Recargamos la ficha según si es registro nuevo o se está editando
    window.location.href = API_FICHA;
}

var gridBox_displayExpr = function (item) {
    return item ? item.ProductoId + ' - ' + item.Descripcion : null;
}

var onCellPrepared = function (e) {
    var tooltip = $('#tooltip').dxTooltip("instance");
    var propiedad = e.column.dataField;
    var MostrarTooltip = ["UnidadAdministrativaId", "ProyectoId", "TipoGastoId"];

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

var toast = function (mensaje, type) {
    DevExpress.ui.notify({ message: mensaje, width: "auto" }, type, 5000);
}