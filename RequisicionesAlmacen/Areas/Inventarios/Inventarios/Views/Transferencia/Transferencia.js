//Componentes
var dxCboAlmacen;
var dxTxtFecha;
var dxTxtComentarios;
var dxCboProductoOrigen;
var dxCboProductoDestino;

//Modales
var modalDetalle;
var modalConfirmaDeshacer;
var modalConfirmaEliminarArticulo;
var modalConfirmaLimpiar;
var modalConfirmaAfectar;

//Botones
var dxButtonCancelar;
var dxButtonDeshacer;
var dxButtonGuardaCambios;

//Forms
var dxForm;
var dxFormModal;
var dxGridDetalles;

//Variables Globales
var modeloVacio;
var contadorRegistrosNuevos;
var rowEliminar;

//Variables de Control
var transferenciaId;
var ignoraEventos;
var cambios;
var eventoRegresar;
var almacenTemp;
var almacenProductoTempId;
var productoTempId;
var seleccionadosTemp;

var ESTATUS_NUEVO = "N";
var ESTATUS_MODIFICADO = "M";

var API_FICHA = "/inventarios/inventarios/transferencia/";

$(document).ready(function () {
    //Inicializamos las variables para la Ficha
    inicializaVariables();

    //Respaldamos el modelo del Form
    getForm();

    //Deshabilitamos los botones de acciones
    habilitaComponentes();

    //Respaldamos el modelo vacio del Form
    modeloVacio = $.extend(true, {}, dxFormModal.option('formData'));
});

var inicializaVariables = function () {
    dxCboAlmacen = $('#dxCboAlmacen').dxSelectBox("instance");
    dxTxtFecha = $('#dxTxtFecha').dxDateBox("instance");
    dxTxtComentarios = $('#dxTxtComentarios').dxTextArea("instance");
    dxCboProductoOrigen = $('#dxCboProductoOrigen').dxDropDownBox("instance");
    dxCboProductoDestino = $('#dxCboProductoDestino').dxDropDownBox("instance");

    modalDetalle = $('#modalDetalle');
    modalConfirmaDeshacer = $('#modalConfirmaDeshacer');
    modalConfirmaEliminarArticulo = $('#modalConfirmaEliminarArticulo');
    modalConfirmaLimpiar = $('#modalConfirmaLimpiar');
    modalConfirmaAfectar = $('#modalConfirmaAfectar');

    dxButtonCancelar = $('#dxButtonCancelar').dxButton("instance");
    dxButtonDeshacer = $('#dxButtonDeshacer').dxButton("instance");
    dxButtonGuardaCambios = $('#dxButtonGuardaCambios').dxButton("instance");

    dxForm = $("#dxForm").dxForm("instance");
    dxFormModal = $("#dxFormModal").dxForm("instance");
    dxGridDetalles = $("#dxGridDetalles").dxDataGrid("instance");

    contadorRegistrosNuevos = -1;
    rowEliminar = null;
    ignoraEventos = false;
    cambios = false;
    eventoRegresar = true;

    dxCboProductoOrigen.option("dataSource", null);
    //dxCboProductoDestino.option("dataSource", null);

    //Asignamos el límite en los campos de Fecha
    var minDate = _ejercicio + "-01-01";
    var maxDate = _ejercicio + "-12-31";

    dxTxtFecha.option("min", minDate);
    dxTxtFecha.option("max", maxDate);
}

var getForm = function () {
    var modelo = $.extend(true, {}, dxForm.option('formData'));
    modelo.TransferenciaId = modelo.TransferenciaId || 0;
    transferenciaId = modelo.TransferenciaId;

    var fecha = modelo.Fecha;

    fecha.setHours(0);
    fecha.setMinutes(0);
    fecha.setSeconds(0);
    fecha.setMilliseconds(0);

    modelo.Fecha = fecha.toLocaleString();

    return modelo;
}

var habilitaComponentes = function () {
    dxButtonDeshacer.option("visible", !_soloLectura);
    dxButtonDeshacer.option("disabled", !cambios);
    dxButtonGuardaCambios.option("visible", !_soloLectura);
}

var setCambios = function () {
    cambios = true;
    habilitaComponentes();
}

var validaEliminarArticulo = function (event) {
    //Obtenemos una copia del objeto a eliminar
    rowEliminar = $.extend(true, {}, event.row.data);

    //Mostramos el modal de confirmacion
    modalConfirmaEliminarArticulo.modal('show');
}

var eliminaArticulo = function () {
    //Obtenemos la instancia store del DataSource
    var store = dxGridDetalles.getDataSource().store();

    //Eliminamos el registro de la tabla
    if (rowEliminar != null) {
        store.remove(rowEliminar.TransferenciaMovtoId)
            .done(function () {
                //Recargamos la informacion de la tabla
                dxGridDetalles.getDataSource().reload();

                setCambios();
            })
            .fail(function () {
                toast("No se pudo eliminar el registro de la tabla.", "error");
            });

        rowEliminar = null;
    }
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

var cboAlmacenChange = function (event) {
    if (!ignoraEventos) {
        almacenTemp = event.previousValue;

        validaLimpiarTabla();
    }
}

var validaLimpiarTabla = function () {
    if (!ignoraEventos) {
        //Obtenemos todos los registros que hay en el dxGridDetalles
        var detalles;
        dxGridDetalles.getDataSource().store().load().done((res) => { detalles = res; });

        //Validamos que haya registros en la tabla detalles
        if (detalles && detalles.length) {
            //Mostramos el modal de confirmacion
            modalConfirmaLimpiar.modal('show');
        } else {
            limpiarTabla();
        }

        setCambios();
    }
}

var cancelarLimpirar = function () {
    ignoraEventos = true;

    dxCboAlmacen.option("value", almacenTemp);

    ignoraEventos = false;
}

var limpiarTabla = function () {
    //Limpiamos y recargamos la informacion de la tabla
    var dataSource = dxGridDetalles.getDataSource();
    dataSource.store().clear();
    dataSource.reload();

    dxCboProductoOrigen.option("dataSource", null);
    //dxCboProductoDestino.option("dataSource", null);

    almacenTemp = dxCboAlmacen.option("value");
}

var nuevoRegistro = function () {
    //Obtenemos los productos y Mostramos el modal
    getComboProductos(true, null);
}

var mostrarNuevoRegistro = function () {
    //Inicializamos el modelo del Form
    dxFormModal.option("formData", $.extend(true, {}, modeloVacio));

    //Inicializamos los campos del Form
    dxFormModal.resetValues();

    //Marcamos el modal con estatus "NUEVO"
    modalDetalle.attr("estatus", ESTATUS_NUEVO);

    //Mostramos el modal
    modalDetalle.modal('show');
}

var editaRegistro = function (event) {
    //Obtenemos los productos y Mostramos el modal
    getComboProductos(false, event);
}

var mostrarRegistroEditado = function (event) {
    //Obtenemos una copia del objeto a modificar
    var modelo = $.extend(true, {}, event.row.data);

    //Le pasamos el objeto al Form para que cargue sus valores
    dxFormModal.option("formData", modelo);

    //Marcamos el modal con estatus "EDITADO"
    modalDetalle.attr("estatus", ESTATUS_MODIFICADO);

    //Mostramos el modal
    modalDetalle.modal('show');
}

var getComboProductos = function (nuevo, event) {
    if (!dxCboAlmacen.option("value")) {
        toast("Favor de seleccionar un almacén.", 'warning');

        return;
    }

    //Deshabilitamos el combo de Productos
    dxCboProductoOrigen.option("disabled", true);

    if (!dxCboProductoOrigen.option("dataSource") || !dxCboProductoOrigen.option("dataSource").length) {
        dxCboProductoOrigen.option("value", null);
        dxCboProductoOrigen.option("dataSource", null);
        dxCboProductoOrigen.option("contentTemplate", null);
        dxCboProductoOrigen.option("contentTemplate", $("#DataGridTemplateOrigen"));

        var almacenId = dxCboAlmacen.option("value");
        var registros = [];

        //Mostramos Loader
        dxLoaderPanel.show();

        _listProductos.forEach(registro => {
            if (registro.AlmacenId === almacenId) {
                registros.push(registro);
            }
        });

        //Asignamos el listado al combo
        dxCboProductoOrigen.option("dataSource", registros);

        //Habilitamos el combo de Productos
        dxCboProductoOrigen.option("disabled", false);

        // Ocultamos Loader
        dxLoaderPanel.hide();

        //Mostramos el modal
        if (nuevo) {
            mostrarNuevoRegistro();
        } else {
            mostrarRegistroEditado(event);
        }
    } else {
        //Habilitamos el combo de Productos
        dxCboProductoOrigen.option("disabled", false);

        //Mostramos el modal
        if (nuevo) {
            mostrarNuevoRegistro();
        } else {
            mostrarRegistroEditado(event);
        }
    }
}

var cboProductoOrigenChange = function (event) {
    dxCboProductoOrigen.close();

    dxCboProductoDestino.option("disabled", true);

    almacenProductoTempId = event.AlmacenProductoId;
    productoTempId = event.ProductoId;

    if (almacenProductoTempId) {
        dxCboProductoDestino.option("disabled", false);
    }

    dxCboProductoDestino.option("value", null);
    dxCboProductoDestino.option("contentTemplate", null);
    dxCboProductoDestino.option("contentTemplate", $("#DataGridTemplateDestino"));
}

var filtrarProductosDestino = function (event) {
    return event.AlmacenProductoId != almacenProductoTempId && event.ProductoId === productoTempId;
}

var cboProductoDestinoChange = function (selectedRowsData) {
    seleccionadosTemp = selectedRowsData;
}

var existeProducto = function (almacenProductoOrigenId, almacenProductoDestinoId) {
    //Obtenemos todos los registros que hay en el dxGrid
    var detalles;
    dxGridDetalles.getDataSource().store().load().done((res) => { detalles = res; });

    //Buscamos un registro que tenga el mismo registro
    var encontrado = detalles.find(x => x.AlmacenProductoOrigenId === almacenProductoOrigenId && x.AlmacenProductoDestinoId === almacenProductoDestinoId);

    //Retornamos <true> si se encontro un registro , <false> de lo contrario
    return encontrado;
}

var guardaCambiosModal = function () {
    //Obtenemos el Objeto que se esta creando/editando en el Form 
    var modelo = dxFormModal.option("formData");

    //Validamos que la informacion requerida del Formulario este completa
    if (!dxFormModal.validate().isValid) {
        toast("Favor de completar los campos requeridos.", 'error');

        return;
    }

    var origen = _listProductos.find(x => x.AlmacenProductoId === dxCboProductoOrigen.option("value"));

    seleccionadosTemp.forEach(registro => {
        //Creamos un modelo por cada destino
        var modelo = {
            TransferenciaMovtoId: contadorRegistrosNuevos,
            ProductoId: registro.ProductoId,
            Descripcion: registro.Descripcion,
            UnidadMedidaId: registro.UnidadMedidaId,
            UnidadDeMedida: registro.UnidadDeMedida,
            AlmacenProductoOrigenId: origen.AlmacenProductoId,
            AlmacenProductoDestinoId: registro.AlmacenProductoId,

            CuentaPresupuestalOrigenId: origen.CuentaPresupuestalEgrId,
            UnidadAdministrativaOrigenId: origen.UnidadAdministrativaId,
            UnidadAdministrativaOrigen: origen.UnidadAdministrativa,
            ProyectoOrigenId: origen.ProyectoId,
            ProyectoOrigen: origen.Proyecto,
            FuenteFinanciamientoOrigenId: origen.FuenteFinanciamientoId,
            FuenteFinanciamientoOrigen: origen.FuenteFinanciamiento,
            TipoGastoOrigenId: origen.TipoGastoId,
            TipoGastoOrigen: origen.TipoGasto,

            CuentaPresupuestalDestinoId: registro.CuentaPresupuestalEgrId,
            UnidadAdministrativaDestinoId: registro.UnidadAdministrativaId,
            UnidadAdministrativaDestino: registro.UnidadAdministrativa,
            ProyectoDestinoId: registro.ProyectoId,
            ProyectoDestino: registro.Proyecto,
            FuenteFinanciamientoDestinoId: registro.FuenteFinanciamientoId,
            FuenteFinanciamientoDestino: registro.FuenteFinanciamiento,
            TipoGastoDestinoId: registro.TipoGastoId,
            TipoGastoDestino: registro.TipoGasto,
            AlmacenDestinoId: registro.AlmacenId,
            AlmacenDestino: registro.Almacen
        }

        //Decrementamos el contador de Registros para el siguiente nuevo registro
        contadorRegistrosNuevos -= 1;

        //Obtenemos la instancia store del DataSource
        var store = dxGridDetalles.getDataSource().store();

        //Validamos que el Producto no se repita
        if (!existeProducto(modelo.AlmacenProductoOrigenId, modelo.AlmacenProductoDestinoId)) {
            store.insert(modelo)
                .done(function () {
                    //Recargamos la informacion de la tabla
                    dxGridDetalles.getDataSource().reload();

                    setCambios();

                    //Ocultamos el modal
                    modalDetalle.modal('hide');
                })
                .fail(function () {
                    toast("No se pudo agregar el nuevo registro a la tabla.", "error");
                });
        }
    });
}

var validaAfectar = function () {
    //Validamos que la informacion requerida del Formulario este completa
    if (!dxForm.validate().isValid) {
        toast("Favor de completar los campos requeridos.", 'error');

        return;
    }

    //Obtenemos todos los registros que hay en el dxGridDetalles
    var detalles;
    dxGridDetalles.getDataSource().store().load().done((res) => { detalles = res; });

    //Validamos que la informacion requerida del Formulario este completa
    if (!detalles || !detalles.length) {
        toast("Favor de agregar artículos para la Transferencia.", 'error');

        return;
    }

    var mensajeCantidad = "";

    detalles.forEach(registro => {
        if (!registro.Cantidad || registro.Cantidad <= 0) {
            mensajeCantidad = "La cantidad para el origen / destino ["
                + registro.CuentaPresupuestalOrigenId + " / " + registro.CuentaPresupuestalDestinoId
                + "] debe ser mayor a 0.";
        }
    });

    if (mensajeCantidad) {
        toast(mensajeCantidad, 'error');

        return;
    }

    //Mostramos el modal de confirmacion
    modalConfirmaAfectar.modal('show');
}

var guardaCambios = function () {
    //Obtenemos todos los registros que hay en el dxGridDetalles
    var detalles;
    dxGridDetalles.getDataSource().store().load().done((res) => { detalles = res; });

    //Mostramos Loader
    dxLoaderPanel.show();

    $.ajax({
        type: "POST",
        url: API_FICHA + "guardaCambios",
        data: { transferencia: getForm(), movimientos: detalles },
        success: function (response) {
            //Mostramos mensaje de Exito
            toast("Registro guardado con exito!", 'success');

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

var gridBox_displayExpr = function (item) {
    return item ? item.ProductoId + ' - ' + item.Descripcion : null;
}

var onCellPrepared = function (e) {
    var tooltip = $('#tooltip').dxTooltip("instance");
    var propiedad = e.column.dataField;
    var MostrarTooltip = ["CuentaPresupuestalOrigenId", "CuentaPresupuestalDestinoId"];

    if (e.rowType == "data" && MostrarTooltip.includes(propiedad)) {
        e.cellElement.mouseover(function () {
            tooltip.option("contentTemplate", null);

            switch (propiedad) {
                case "CuentaPresupuestalOrigenId":
                    var texto = e.data.UnidadAdministrativaOrigenId + ' - ' + e.data.UnidadAdministrativaOrigen + ', '
                        + e.data.ProyectoOrigenId + ' - ' + e.data.ProyectoOrigen + ', '
                        + e.data.FuenteFinanciamientoOrigenId + ' - ' + e.data.FuenteFinanciamientoOrigen + ', '
                        + e.data.TipoGastoOrigenId + ' - ' + e.data.TipoGastoOrigen;

                    tooltip.option("contentTemplate", document.createTextNode(texto));
                    break;

                case "CuentaPresupuestalDestinoId":
                    var texto = e.data.UnidadAdministrativaDestinoId + ' - ' + e.data.UnidadAdministrativaDestino + ', '
                        + e.data.ProyectoDestinoId + ' - ' + e.data.ProyectoDestino + ', '
                        + e.data.FuenteFinanciamientoDestinoId + ' - ' + e.data.FuenteFinanciamientoDestino + ', '
                        + e.data.TipoGastoDestinoId + ' - ' + e.data.TipoGastoDestino;

                    tooltip.option("contentTemplate", document.createTextNode(texto));
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

var onEditingStart = function (event) {
    if (_soloLectura) {
        event.cancel = true;
    }
}

var regresarListado = function () {
    window.location.href = API_FICHA + "listar";
}

var recargarFicha = function () {
    // Recargamos la ficha según si es registro nuevo o se está editando
    window.location.href = API_FICHA + (transferenciaId == 0 ? "nuevo" : "editar/" + transferenciaId);
}

var toast = function (message, type) {
    DevExpress.ui.notify({ message: message, width: "auto" }, type, 5000);
}