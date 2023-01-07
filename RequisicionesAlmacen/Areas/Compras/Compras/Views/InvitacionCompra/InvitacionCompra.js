//Modales
var modalConfirmaDeshacer;
var modalConfirmaEliminar;
var modalConfirmaEliminarArticulo;
var modalCotizacion;
var modalConfirmaEliminarCotizacion;
var modalConfirmaOC;

//Componentes
var dxCboProveedorCotizacion;
var dropZoneText;
var dropZoneFile;
var dxFileUploader;
var dxTxtFileName;

//Botones
var dxButtonGuardaCambios;
var dxButtonCancelar;
var dxButtonDeshacer;
var dxButtonEliminar;
var dxButtonEliminarDropZone;

//Grids
var dxGridDetalles;
var dxGridProveedoresInvitados;
var dxGridListadoProveedores;
var dxGridPreciosProveedor;
var dxGridGanadoresProveedor;
var dxGridCotizacionesProveedor;
var dxGridOrdenesCompra;

//Forms
var dxForm;
var dxFormModalCotizacion;

//Variables Globales
var articuloEliminar;
var articulosEliminados;
var preciosProveedor;
var ganadoresProveedor;
var ordenesCompra;
var modeloVacioCotizacion;
var file;
var cotizacionEliminar;
var cotizacionesEliminadas;
var invitacionOC;

//Variables de Control
var invitacionCompraId;
var ignoraEventos;
var cambios;
var eventoRegresar;
var seleccionListIndex;
var contadorArchivos;

//Drawer
var dxDrawer;
var dxListDrawer;

//Tabs
var tabProveedores;
var tabCotizaciones;
var tabGanadores;
var tabOrdenesCompra;

var API_FICHA = "/compras/compras/invitacioncompra/";

var ESTATUS_REGISTRO_ACTIVO = 1;
var ESTATUS_REGISTRO_BORRADO = 3;

var ESTATUS_INVITACION_GUARDADA = 92;

var ESTATUS_DETALLE_GUARDADO = 93;
var ESTATUS_DETALLE_CANCELADO = 94;

$(document).ready(function () {
    //Inicializamos las variables para la Ficha
    inicializaVariables();

    //Respaldamos el modelo del Form
    getForm();

    //Deshabilitamos los botones de acciones
    habilitaComponentes();

    //Respaldamos el modelo vacio del Form Cotizacion
    modeloVacioCotizacion = $.extend(true, {}, dxFormModalCotizacion.option('formData'));

    //Contruimos la tabla de Precios por Proveedor
    convertirListadoPreciosProveedor();

    //Contruimos la tabla de Ganadores por Proveedor
    convertirListadoGanadoresProveedor();

    //Inicializamos el drawer
    mostrarProveedores();
});

var inicializaVariables = function () {
    modalConfirmaDeshacer = $('#modalConfirmaDeshacer');
    modalConfirmaEliminar = $('#modalConfirmaEliminar');
    modalConfirmaEliminarArticulo = $('#modalConfirmaEliminarArticulo');
    modalCotizacion = $('#modalCotizacion');
    modalConfirmaEliminarCotizacion = $('#modalConfirmaEliminarCotizacion');
    modalConfirmaOC = $('#modalConfirmaOC');

    dxCboProveedorCotizacion = $('#dxCboProveedorCotizacion').dxSelectBox("instance");
    dropZoneText = $("#dropzone-text")[0];
    dropZoneFile = $("#dropzone-file")[0];
    dropZoneText.style.display = dropZoneFile.src != "" ? "none" : "";
    dxFileUploader = $('#dxFileUploader').dxFileUploader("instance");
    dxTxtFileName = $('#dxTxtFileName').dxTextBox("instance");

    dxButtonGuardaCambios = $('#dxButtonGuardaCambios').dxButton("instance");
    dxButtonCancelar = $('#dxButtonCancelar').dxButton("instance");
    dxButtonDeshacer = $('#dxButtonDeshacer').dxButton("instance");
    dxButtonEliminar = $('#dxButtonEliminar').dxButton("instance");
    dxButtonEliminarDropZone = $('#dxButtonEliminarDropZone').dxButton("instance");
    dxButtonEliminarDropZone.option("visible", dropZoneFile.src != "");

    dxGridDetalles = $("#dxGridDetalles").dxDataGrid("instance");
    dxGridProveedoresInvitados = $("#dxGridProveedoresInvitados").dxDataGrid("instance");
    dxGridListadoProveedores = $("#dxGridListadoProveedores").dxDataGrid("instance");
    dxGridPreciosProveedor = $("#dxGridPreciosProveedor").dxDataGrid("instance");
    dxGridGanadoresProveedor = $("#dxGridGanadoresProveedor").dxDataGrid("instance");
    dxGridCotizacionesProveedor = $("#dxGridCotizacionesProveedor").dxDataGrid("instance");
    dxGridOrdenesCompra = $("#dxGridOrdenesCompra").dxDataGrid("instance");
    dxGridPreciosProveedor.option("dataSource", []);
    dxGridGanadoresProveedor.option("dataSource", []);

    dxForm = $("#dxForm").dxForm("instance");
    dxFormModalCotizacion = $("#dxFormModalCotizacion").dxForm("instance");

    ignoraEventos = false;
    cambios = false;
    eventoRegresar = true;
    seleccionListIndex = 0;
    contadorArchivos = 0;

    articuloEliminar = null;
    articulosEliminados = [];
    preciosProveedor = [];
    ganadoresProveedor = [];
    ordenesCompra = [];
    cotizacionEliminar = null;
    cotizacionesEliminadas = [];
    invitacionOC = null;

    //Eliminamos una clase de style en template content
    $('#templateContent').removeClass('d-none');

    dxDrawer = $('#dxDrawer').dxDrawer('instance');
    dxListDrawer = $('#dxListDrawer').dxList('instance');

    tabProveedores = $("#tabProveedores");
    tabCotizaciones = $("#tabCotizaciones");
    tabGanadores = $("#tabGanadores");
    tabOrdenesCompra = $("#tabOrdenesCompra");
}

var getForm = function () {
    var modelo = $.extend(true, {}, dxForm.option('formData'));

    modelo.InvitacionCompraId = modelo.InvitacionCompraId || 0;
    invitacionCompraId = modelo.InvitacionCompraId;

    return modelo;
}

var habilitaComponentes = function () {
    dxButtonDeshacer.option("visible", !_soloLectura);
    dxButtonDeshacer.option("disabled", !cambios);
    dxButtonEliminar.option("visible", !cambios && getForm().EstatusId === ESTATUS_INVITACION_GUARDADA);

    if (_soloLectura) {
        dxButtonDeshacer.option("visible", false);
        dxButtonEliminar.option("visible", false);
        dxButtonGuardaCambios.option("visible", false);
    }
}

var setCambios = function () {
    cambios = true;
    habilitaComponentes();
}

var onClickToolbar = function () {
    dxDrawer.toggle();
}

var onItemClickDrawer = function (event) {
    seleccionListIndex = event.itemData.index;

    siguientePestania();
}

var mostrarProveedores = function () {
    seleccionListIndex = 1;

    siguientePestania();
}

var mostrarCotizaciones = function () {
    seleccionListIndex = 2;

    siguientePestania();
}

var mostrarGanadores = function () {
    seleccionListIndex = 3;

    siguientePestania();
}

var mostrarOrdenesCompra = function () {
    seleccionListIndex = 4;

    siguientePestania();
}

var siguientePestania = function () {
    //Ocultamos las pestañas
    tabProveedores.hide();
    tabCotizaciones.hide();
    tabGanadores.hide();
    tabOrdenesCompra.hide();

    //Respaldamos las tablas
    respaldarTablas();

    switch (seleccionListIndex) {
        //Proveedores
        case 1:
            tabProveedores.show();
            dxListDrawer.option('selectedItemKeys', [1]);
        break;

        //Cotizaciones
        case 2:
            tabCotizaciones.show();
            dxListDrawer.option('selectedItemKeys', [2]);

            //Construímos la tabla
            construirTblArticuloProveedores();
        break;

        //Ganadores
        case 3:
            tabGanadores.show();
            dxListDrawer.option('selectedItemKeys', [3]);

            //Construímos la tabla
            construirTblGanadoresProveedores();
        break;

        //Ordenes de Compra
        case 4:
            tabOrdenesCompra.show();
            dxListDrawer.option('selectedItemKeys', [4]);

            //Construímos la tabla
            construirTblOrdenesCompra();
        break;

        default:
            tabProveedores.show();
            dxListDrawer.option('selectedItemKeys', [1]);
        break;
    }
}

var gridBox_displayExpr = function (item) {
    return item ? item.ProductoId + ' - ' + item.Descripcion : null;
}

var validaEliminarArticulo = function (event) {
    //Obtenemos una copia del objeto a eliminar
    articuloEliminar = $.extend(true, {}, event.row.data);

    //Mostramos el modal de confirmacion
    modalConfirmaEliminarArticulo.modal('show');
}

var eliminaArticulo = function () {
    //Obtenemos la instancia store del DataSource
    var store = dxGridDetalles.getDataSource().store();

    //Eliminamos el registro de la tabla
    if (articuloEliminar != null) {
        store.remove(articuloEliminar.InvitacionCompraDetalleId)
            .done(function () {
                //Si el registro viene de la base de datos, respaldamos el registro 
                //para posteriormente eliminarlo en la base de datos
                if (articuloEliminar.InvitacionCompraDetalleId > 0) {
                    //Actualizamos el estatus del registro a "Cancelado"
                    articuloEliminar.EstatusId = ESTATUS_DETALLE_CANCELADO; //Borrar registro

                    //Respaldamos el registro que se acaba de eliminar
                    articulosEliminados.push(articuloEliminar);
                }

                //Construimos las tablas
                construirTblArticuloProveedores();
                construirTblGanadoresProveedores();
                construirTblOrdenesCompra();

                //Recargamos la informacion de la tabla
                dxGridDetalles.getDataSource().reload();

                setCambios();
            })
            .fail(function () {
                toast("No se pudo eliminar el registro de la tabla.", "error");
            });

        articuloEliminar = null;
    }
}

var filtrarProveedores = function (event) {
    return event.Seleccionado;
}

var onListadoProveedoresChange = function (e) {
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
            //Obtenemos todos los registros que hay en el dxGridListadoProveedores
            var proveedores;
            dxGridListadoProveedores.getDataSource().store().load().done((res) => { proveedores = res; });

            var proveedor = proveedores.find(x => x.InvitacionCompraProveedorId === changes[0].key);
            proveedor.Seleccionado = changes[0].data.Seleccionado === true;

            //Actualizamos los listados de proveedores
            actualizarProveedor(proveedor);
        }
    }
}

var onListadoGanadoresProveedoresChange = function (e) {
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
            //Marcamos los cambios
            setCambios();

            var cambios = changes[0].data;
            var propiedadTmp = Object.getOwnPropertyNames(cambios)[0];
            var seleccionado = cambios[propiedadTmp];

            //Obtenemos todos los registros que hay en el dxGridListadoProveedoresGanadores
            var registro = getGanadoresProveedores().find(x => x.InvitacionCompraDetallePrecioProveedorId === changes[0].key);

            //Actualizamos los proveedores ganadores
            if (seleccionado === true) {
                Object.keys(registro).forEach(propiedad => {
                    if (propiedad != "InvitacionCompraDetallePrecioProveedorId"
                        && propiedad != "InvitacionCompraDetalleId"
                        && propiedad != "ProductoId"
                        && propiedad != "Producto"
                        && propiedad != "Descripcion"
                        && propiedad != "Comentario"
                        && propiedad != propiedadTmp) {

                        registro[propiedad] = false;
                    }
                });
            }
        }
    }
}

var onEditingStartPreciosGanadores = function (event) {
    var detalle = getDetalles().find(x => x.InvitacionCompraDetalleId === event.data.InvitacionCompraDetalleId);

    if (_soloLectura || !detalle.PermiteEditar) {
        event.cancel = true;
    }
}

var onEditingStartOC = function (event) {
    if (_soloLectura || cambios || event.data.OrdenCompraId > 0) {
        event.cancel = true;
    }
}

var onEditingStartProveedor = function (event) {
    if (_soloLectura || !permiteEliminarProveedor(event.data.ProveedorId)) {
        event.cancel = true;
    }
}

var validaEliminarProveedor = function (event) {
    return permiteEliminarProveedor(event.row.data.ProveedorId);
}

var permiteEliminarProveedor = function (proveedorId) {
    var existe = _listOrdenesCompra.find(x => x.ProveedorId === proveedorId);

    return !_soloLectura && !existe;
}

var eliminarProveedor = function (event) {
    //Obtenemos todos los registros que hay en el dxGridListadoProveedores
    var proveedores;
    dxGridListadoProveedores.getDataSource().store().load().done((res) => { proveedores = res; });

    var proveedor = proveedores.find(x => x.InvitacionCompraProveedorId === event.row.data.InvitacionCompraProveedorId);
    proveedor.Seleccionado = false;

    //Actualizamos los listados de proveedores
    actualizarProveedor(proveedor);
}

var actualizarProveedor = function (proveedor) {
    if (proveedor) {
        //Obtenemos la instancia store del DataSource y actualizamos
        dxGridProveedoresInvitados.getDataSource().store()
            .update(proveedor.InvitacionCompraProveedorId, proveedor)
            .done(function () {
                //Recargamos la informacion de la tabla
                dxGridProveedoresInvitados.getDataSource().reload();

                //Recargamos la informacion de la tabla
                dxGridListadoProveedores.getDataSource().reload();

                //Eliminamos los archivos de cotización del proveedor
                if (proveedor.Seleccionado === false) {
                    getCotizacionesProveedor().forEach(cotizacion => {
                        if (cotizacion.InvitacionCompraProveedorId === proveedor.InvitacionCompraProveedorId) {
                            cotizacionEliminar = cotizacion;
                            eliminaCotizacion();
                        }
                    });
                }
            })
            .fail(function () {
                mensaje = "Error:\n No se pudieron actualizar los registros de la tabla.";
            })

        //Construimos las tablas
        construirTblArticuloProveedores();
        construirTblGanadoresProveedores();
        construirTblOrdenesCompra();

        //Marcamos los cambios
        setCambios();
    }
}

var convertirListadoPreciosProveedor = function () {
    var contadorRegistros = 0;

    _listPreciosProveedores.forEach(precioProveedor => {
        if (!preciosProveedor.find(x => x.ProductoId === precioProveedor.ProductoId)) {
            contadorRegistros -= 1;

            var registroNuevo = {
                InvitacionCompraDetallePrecioProveedorId: contadorRegistros,
                InvitacionCompraDetalleId: precioProveedor.InvitacionCompraDetalleId,
                ProductoId: precioProveedor.ProductoId,
                Descripcion: precioProveedor.Descripcion,
                Producto: precioProveedor.Producto
            }

            _listPreciosProveedores.forEach(temp => {
                if (temp.ProductoId === precioProveedor.ProductoId) {
                    registroNuevo[temp.Proveedor] = temp.PrecioArticulo;
                }
            });

            preciosProveedor.push(registroNuevo);
        }
    });

    var dataSource = new DevExpress.data.DataSource({
        store: {
            type: "array",
            key: "InvitacionCompraDetallePrecioProveedorId",
            data: preciosProveedor
        },
    });

    dxGridPreciosProveedor.option("dataSource", dataSource);
}

var convertirListadoGanadoresProveedor = function () {
    var contadorRegistros = 0;

    _listPreciosProveedores.forEach(precioProveedor => {
        if (!ganadoresProveedor.find(x => x.ProductoId === precioProveedor.ProductoId)) {
            contadorRegistros -= 1;

            var registroNuevo = {
                InvitacionCompraDetallePrecioProveedorId: contadorRegistros,
                InvitacionCompraDetalleId: precioProveedor.InvitacionCompraDetalleId,
                ProductoId: precioProveedor.ProductoId,
                Descripcion: precioProveedor.Descripcion,
                Producto: precioProveedor.Producto
            }

            var comentario = null;

            _listPreciosProveedores.forEach(temp => {
                if (temp.ProductoId === precioProveedor.ProductoId) {
                    registroNuevo[temp.Proveedor] = temp.Ganador;

                    if (temp.Ganador === true) {
                        comentario = temp.Comentario;
                    }
                }
            });

            //Asignamos el Comentario
            registroNuevo.Comentario = comentario;

            //Agregamos el registro
            ganadoresProveedor.push(registroNuevo);
        }
    });

    var dataSource = new DevExpress.data.DataSource({
        store: {
            type: "array",
            key: "InvitacionCompraDetallePrecioProveedorId",
            data: ganadoresProveedor
        },
    });

    dxGridGanadoresProveedor.option("dataSource", dataSource);
}

var respaldarTablas = function () {
    //Respaldamos los Precios por Proveedor
    preciosProveedor = [];
    dxGridPreciosProveedor.getDataSource().store().load().done((res) => { preciosProveedor = res; });

    //Respaldamos los Ganadores por Proveedor
    ganadoresProveedor = [];
    dxGridGanadoresProveedor.getDataSource().store().load().done((res) => { ganadoresProveedor = res; });

    //Respaldamos las Ordenes de Compra
    ordenesCompra = [];
    dxGridOrdenesCompra.getDataSource().store().load().done((res) => { ordenesCompra = res; });
}

var construirTblArticuloProveedores = function () {
    var dataFilter = [];
    var contadorRegistros = 0;

    //Obtenemos todos los registros que hay en el dxGridDetalles
    getDetalles().forEach(articulo => {
        //Disminuímos el contador
        contadorRegistros = contadorRegistros - 1;

        var registroNuevo = {
            InvitacionCompraDetallePrecioProveedorId: contadorRegistros,
            InvitacionCompraDetalleId: articulo.InvitacionCompraDetalleId,
            ProductoId: articulo.ProductoId,
            Descripcion: articulo.Descripcion,
            Producto: articulo.ProductoId + ' - ' + articulo.Descripcion,
            Ganador: false
        }

        //Buscamos el registro
        var existe = preciosProveedor.find(x => x.InvitacionCompraDetalleId === registroNuevo.InvitacionCompraDetalleId);

        //Obtenemos todos los registros que hay en el dxGridListadoProveedores
        getProveedoresInvitados().forEach(proveedor => {
            var columna = proveedor.ProveedorId + ' - ' + proveedor.RazonSocial;

            registroNuevo[columna] = existe && existe[columna] ? existe[columna] : 0.0;
        });

        //Agregamos el registro al source
        dataFilter.push(registroNuevo);
    });

    var dataSource = new DevExpress.data.DataSource({
        store: {
            type: "array",
            key: "InvitacionCompraDetallePrecioProveedorId",
            data: dataFilter
        },
    });

    dxGridPreciosProveedor.option("dataSource", dataSource);
}

var construirTblGanadoresProveedores = function () {
    var dataFilter = [];
    var contadorRegistros = 0;

    //Obtenemos todos los registros que hay en el dxGridDetalles
    getDetalles().forEach(articulo => {
        //Disminuímos el contador
        contadorRegistros = contadorRegistros - 1;

        var registroNuevo = {
            InvitacionCompraDetallePrecioProveedorId: contadorRegistros,
            InvitacionCompraDetalleId: articulo.InvitacionCompraDetalleId,
            ProductoId: articulo.ProductoId,
            Descripcion: articulo.Descripcion,
            Producto: articulo.ProductoId + ' - ' + articulo.Descripcion
        }

        //Buscamos el registro
        var existe = ganadoresProveedor.find(x => x.InvitacionCompraDetalleId === registroNuevo.InvitacionCompraDetalleId);

        var comentario = null;

        //Obtenemos todos los registros que hay en el dxGridListadoProveedores
        getProveedoresInvitados().forEach(proveedor => {
            var columna = proveedor.ProveedorId + ' - ' + proveedor.RazonSocial;

            registroNuevo[columna] = existe && existe[columna] ? existe[columna] : false;
        });

        //Asignamos el Comentario
        registroNuevo.Comentario = existe ? existe.Comentario : null;

        //Agregamos el registro al source
        dataFilter.push(registroNuevo);
    });

    var dataSource = new DevExpress.data.DataSource({
        store: {
            type: "array",
            key: "InvitacionCompraDetallePrecioProveedorId",
            data: dataFilter
        },
    });

    dxGridGanadoresProveedor.option("dataSource", dataSource);

    $("#dxGridGanadoresProveedor").dxDataGrid({
        onOptionChanged: function (e) {
            onListadoGanadoresProveedoresChange(e);
        }
    });
}

var construirTblOrdenesCompra = function () {
    //Construir tablas complementarias
    construirTblArticuloProveedores();
    construirTblGanadoresProveedores();

    //Validamos los registros que hay en el dxGridProveedoresInvitados
    var preciosProveedorTemp = getPreciosProveedores();

    var contadorPreciosProveedor = 0;
    var preciosProveedor = [];

    preciosProveedorTemp.forEach(precioProveedor => {
        var proveedorGanador = getGanadoresProveedores().find(x => x.InvitacionCompraDetallePrecioProveedorId === precioProveedor.InvitacionCompraDetallePrecioProveedorId);

        var proveedores = [];

        Object.keys(precioProveedor).forEach(propiedad => {
            if (propiedad != "InvitacionCompraDetallePrecioProveedorId"
                && propiedad != "InvitacionCompraDetalleId"
                && propiedad != "ProductoId"
                && propiedad != "Producto"
                && propiedad != "Descripcion"
                && propiedad != "Ganador"
                && propiedad != "Comentario") {

                var proveedor = {
                    Id: propiedad.substring(0, propiedad.indexOf(" - ")),
                    PrecioArticulo: precioProveedor[propiedad],
                    Ganador: proveedorGanador[propiedad],
                    Comentario: proveedorGanador.Comentario
                };

                proveedores.push(proveedor);
            }
        });

        proveedores.forEach(proveedor => {
            contadorPreciosProveedor -= 1;

            var proveedorId = Number.parseInt(proveedor.Id);

            var existe = _listPreciosProveedores.find(x => x.InvitacionCompraDetalleId === precioProveedor.InvitacionCompraDetalleId && x.ProveedorId === proveedorId);

            var modelo = {
                InvitacionCompraDetallePrecioProveedorId: existe ? existe.InvitacionCompraDetallePrecioProveedorId : contadorPreciosProveedor,
                InvitacionCompraDetalleId: precioProveedor.InvitacionCompraDetalleId,
                ProveedorId: proveedorId,
                PrecioArticulo: proveedor.PrecioArticulo,
                Ganador: proveedor.Ganador,
                Comentario: proveedor.Ganador === true ? proveedor.Comentario : null,
                EstatusId: ESTATUS_REGISTRO_ACTIVO
            };

            preciosProveedor.push(modelo);
        });
    });

    var registrosGanadores = [];

    preciosProveedor.forEach(registro => {
        if (registro.Ganador === true) {
            registrosGanadores.push(registro);
        }
    });

    var almacen = _listAlmacenes.find(x => x.AlmacenId === getForm().AlmacenId);
    var contadorRegistros = 0;
    var ordenesCompraTemp = [];

    registrosGanadores.forEach(modelo => {
        var detalle = getDetalles().find(x => x.InvitacionCompraDetalleId === modelo.InvitacionCompraDetalleId && x.EstatusId === ESTATUS_DETALLE_GUARDADO);

        if (detalle) {
            var existe = ordenesCompraTemp.find(x => x.ProveedorId === modelo.ProveedorId && x.OrdenCompraId < 0);

            var cantidad = round(detalle.Cantidad, 4);
            var costo = round(modelo.PrecioArticulo, 4);
            var importe = trunc(cantidad * costo, 4);
            var ieps = round(detalle.IEPS, 2);
            var ajuste = round(detalle.Ajuste, 4);

            var iva = 0;
            var ish = 0;
            var retencionISR = 0;
            var retencionCedular = 0;
            var retencionIVA = 0;
            var totalPresupuesto = 0;
            var total = 0;

            var proveedor = _listProveedores.find(x => x.ProveedorId === modelo.ProveedorId);
            var tipoComprobanteFiscalId = proveedor ? proveedor.TipoComprobanteFiscalId : -1;
            var tarifaImpuesto = detalle.TarifaImpuestoId ? _listTarifasImpuesto.find(x => x.TarifaImpuestoId == detalle.TarifaImpuestoId) : null;

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

            var ordenCompraDet = {
                OrdenCompraDetId: -1,
                OrdenCompraId: existe ? existe.OrdenCompraId : contadorRegistros,
                TarifaImpuestoId: detalle.TarifaImpuestoId,
                ProductoId: detalle.ProductoId,
                CuentaPresupuestalEgrId: detalle.CuentaPresupuestalEgrId,
                Descripcion: detalle.Descripcion,
                Status: 'A',
                Cantidad: cantidad,
                Costo: costo,
                Importe: importe,
                IEPS: ieps,
                Ajuste: ajuste,
                IVA: round(iva, 2),
                ISH: round(ish, 2),
                RetencionISR: round(retencionISR, 2),
                RetencionCedular: round(retencionCedular, 2),
                RetencionIVA: round(retencionIVA, 2),
                TotalPresupuesto: round(totalPresupuesto, 4),
                Total: round(total, 4),
                InvitacionCompraDetalleId: modelo.InvitacionCompraDetalleId
            };

            if (!existe) {
                contadorRegistros -= 1;

                var temp = ordenesCompra.find(x => x.ProveedorId === proveedor.ProveedorId && x.OrdenCompraId < 0);

                var ordenCompra = {
                    OrdenCompraId: contadorRegistros,
                    ProveedorId: proveedor.ProveedorId,
                    Proveedor: proveedor.RazonSocial,
                    AlmacenId: almacen.AlmacenId,
                    Almacen: almacen.Nombre,
                    TipoOperacionId: proveedor.TipoOperacionId,
                    TipoComprobanteFiscalId: proveedor.TipoComprobanteFiscalId,
                    Ejercicio: new Date().getFullYear(),
                    Fecha: new Date().toLocaleString(),
                    FechaRecepcion: (temp ? temp.FechaRecepcion : null),
                    Referencia: (temp ? temp.Referencia : null),
                    Observacion: (temp ? temp.Observacion : null),
                    GastoPorComprobarId: null,
                    Monto: ordenCompraDet.Total,
                    Detalles: [ordenCompraDet]
                };

                ordenesCompraTemp.push(ordenCompra);
            } else {
                existe.Monto += ordenCompraDet.Total;
                existe.Detalles.push(ordenCompraDet);
            }
        }
    });

    //Agregamos las OC nuevas
    var listadoOC = [];

    _listOrdenesCompra.forEach(oc => {
        listadoOC.push(oc);
    });

    ordenesCompraTemp.forEach(oc => {
        listadoOC.push(oc);
    });

    //Agregamos los datos al source
    dxGridOrdenesCompra.option("dataSource", listadoOC);
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

var ocultarColumnasProveedor = function () {
    $("#dxGridPreciosProveedor").dxDataGrid("columnOption", "InvitacionCompraDetallePrecioProveedorId", "visible", false);
    $("#dxGridPreciosProveedor").dxDataGrid("columnOption", "InvitacionCompraDetalleId", "visible", false);
    $("#dxGridPreciosProveedor").dxDataGrid("columnOption", "ProductoId", "visible", false);
    $("#dxGridPreciosProveedor").dxDataGrid("columnOption", "Descripcion", "visible", false);
    $("#dxGridPreciosProveedor").dxDataGrid("columnOption", "ProveedorId", "visible", false);
    $("#dxGridPreciosProveedor").dxDataGrid("columnOption", "EstatusId", "visible", false);
    $("#dxGridPreciosProveedor").dxDataGrid("columnOption", "Ganador", "visible", false);

    $("#dxGridPreciosProveedor").dxDataGrid("columnOption", "Producto", "allowEditing", false);

    $("#dxGridGanadoresProveedor").dxDataGrid("columnOption", "InvitacionCompraDetallePrecioProveedorId", "visible", false);
    $("#dxGridGanadoresProveedor").dxDataGrid("columnOption", "InvitacionCompraDetalleId", "visible", false);
    $("#dxGridGanadoresProveedor").dxDataGrid("columnOption", "ProductoId", "visible", false);
    $("#dxGridGanadoresProveedor").dxDataGrid("columnOption", "Descripcion", "visible", false);
    $("#dxGridGanadoresProveedor").dxDataGrid("columnOption", "ProveedorId", "visible", false);
    $("#dxGridGanadoresProveedor").dxDataGrid("columnOption", "EstatusId", "visible", false);

    $("#dxGridGanadoresProveedor").dxDataGrid("columnOption", "Producto", "allowEditing", false);

    //Obtenemos todos los registros que hay en el dxGridListadoProveedores
    getProveedoresInvitados().forEach(proveedor => {
        var columna = proveedor.ProveedorId + ' - ' + proveedor.RazonSocial;

        $("#dxGridPreciosProveedor").dxDataGrid("columnOption", columna, "format", "$ #,##0.0000");
    });
}

var getDetalles = function () {
    //Obtenemos todos los registros que hay en el dxGridDetalles
    var registros;
    dxGridDetalles.getDataSource().store().load().done((res) => { registros = res; });

    return registros;
}

var getProveedoresInvitados = function () {
    //Obtenemos todos los registros que hay en el dxGridProveedoresInvitados
    var proveedoresInvitados = [];
    var registros;
    dxGridProveedoresInvitados.getDataSource().store().load().done((res) => { registros = res; });

    //Buscamos los Proveedores seleccionados
    registros.forEach(m => {
        if (m.Seleccionado === true) {
            proveedoresInvitados.push(m);
        }
    });

    return proveedoresInvitados;
}

var getPreciosProveedores = function () {
    //Obtenemos todos los registros que hay en el dxGridPreciosProveedor
    var registros;
    dxGridPreciosProveedor.getDataSource().store().load().done((res) => { registros = res; });

    return registros;
}

var getGanadoresProveedores = function () {
    //Obtenemos todos los registros que hay en el dxGridGanadoresProveedor
    var registros;
    dxGridGanadoresProveedor.getDataSource().store().load().done((res) => { registros = res; });

    return registros;
}

var getCotizacionesProveedor = function () {
    //Obtenemos todos los registros que hay en el dxGridCotizacionesProveedor
    var registros;
    dxGridCotizacionesProveedor.getDataSource().store().load().done((res) => { registros = res; });

    return registros;
}

var nuevaCotizacion = function (event) {
    //Inicializamos el modelo del Form
    dxFormModalCotizacion.option("formData", $.extend(true, {}, modeloVacioCotizacion));

    //Inicializamos los campos del Form
    dxFormModalCotizacion.resetValues();

    //Mostramos el modal
    modalCotizacion.modal('show');

    //Limpiamos el DropZone
    eliminarDropZone();

    //Limpiamos el source
    dxCboProveedorCotizacion.option("dataSource", []);

    //Actualizamos el DataSource
    var dataSource = new DevExpress.data.DataSource({
        store: {
            type: "array",
            key: "InvitacionCompraProveedorId",
            data: getProveedoresInvitados()
        },
        paginate: true,
        pageSize: 10
    });

    //Asignamos el source
    dxCboProveedorCotizacion.option("dataSource", dataSource);
}

var agregarCotizacion = function () {
    //Obtenemos el Objeto que se esta creando/editando en el Form 
    var modelo = dxFormModalCotizacion.option("formData");

    //Validamos que la informacion requerida del Formulario este completa
    if (!dxFormModalCotizacion.validate().isValid) {
        toast("Favor de completar los campos requeridos.", 'error');

        return;
    }

    //Validamos que se haya agregado un archivo
    if (!file) {
        toast("Favor de adjuntar el archivo.", 'error');

        return;
    }

    //Actualizamos el contador de archivos
    contadorArchivos -= 1;

    //Asignamos el Id al modelo
    var nombreArchivo = file.name;

    modelo.InvitacionCompraProveedorCotizacionId = contadorArchivos;
    modelo.Proveedor = _listProveedores.find(x => x.InvitacionCompraProveedorId === modelo.InvitacionCompraProveedorId).RazonSocial;
    modelo.NombreArchivo = nombreArchivo;
    modelo.FechaCotizacion = new Date(Date.now());
    modelo.Tipo = getTipoArchivo(nombreArchivo.substring(nombreArchivo.indexOf("."), nombreArchivo.length));

    //Create FormData object
    var formData = new FormData();

    //Looping over all files and add it to FormData object
    formData.append("file", file);

    //Mostramos Loader
    dxLoaderPanel.show();

    //Enviamos la informacion al controlador
    $.ajax({
        type: "POST",
        url: API_FICHA + "guardaArchivoTemporal",
        contentType: false,
        processData: false,
        data: formData,
        success: function (response) {
            //Agregamos el nombre temporal del archivo
            modelo.NombreArchivoTmp = response;

            //Obtenemos la instancia store del DataSource
            var store = dxGridCotizacionesProveedor.getDataSource().store();

            store.insert(modelo)
                .done(function () {
                    //Recargamos la informacion de la tabla
                    dxGridCotizacionesProveedor.getDataSource().reload();

                    //Ocultamos el modal
                    modalCotizacion.modal('hide');

                    //Marcamos los cambios
                    setCambios();

                    //Ocultamos Loader
                    dxLoaderPanel.hide();
                })
                .fail(function () {
                    //Ocultamos Loader
                    dxLoaderPanel.hide();

                    toast("No se pudo agregar el nuevo registro a la tabla.", "error");
                });

            //Ocultamos Loader
            dxLoaderPanel.hide();
        },
        error: function (response, status, error) {
            //Ocultamos Loader
            dxLoaderPanel.hide();

            //Mostramos mensaje de error
            toast("Error:\n" + response.responseText, 'error');
        }
    });
}

var validaEliminarCotizacion = function (event) {
    //Obtenemos una copia del objeto a eliminar
    cotizacionEliminar = $.extend(true, {}, event.row.data);

    //Mostramos el modal de confirmacion
    modalConfirmaEliminarCotizacion.modal('show');
}

var eliminaCotizacion = function () {
    //Obtenemos la instancia store del DataSource
    var store = dxGridCotizacionesProveedor.getDataSource().store();

    //Eliminamos el registro de la tabla
    if (cotizacionEliminar != null) {
        store.remove(cotizacionEliminar.InvitacionCompraProveedorCotizacionId)
            .done(function () {
                //Si el registro viene de la base de datos, respaldamos el registro 
                //para posteriormente eliminarlo en la base de datos
                if (cotizacionEliminar.InvitacionCompraProveedorCotizacionId > 0) {
                    //Actualizamos el estatus del registro a "Cancelado"
                    cotizacionEliminar.EstatusId = ESTATUS_REGISTRO_BORRADO; //Borrar registro

                    //Respaldamos el registro que se acaba de eliminar
                    cotizacionesEliminadas.push(cotizacionEliminar);
                }

                //Recargamos la informacion de la tabla
                dxGridCotizacionesProveedor.getDataSource().reload();

                setCambios();
            })
            .fail(function () {
                toast("No se pudo eliminar el registro de la tabla.", "error");
            });

        cotizacionEliminar = null;
    }
}

var descargarCotizacion = function (event) {
    var cotizacionId = event.row.data.CotizacionId;
    var nombreArchivoTmp = event.row.data.NombreArchivoTmp;
    var nombreOriginal = event.row.data.NombreArchivo;

    if (nombreArchivoTmp || cotizacionId) {
        //Enviamos la informacion al controlador
        $.ajax({
            type: "POST",
            url: API_FICHA + "buscarArchivo",
            data: { archivoId: cotizacionId, nombreArchivoTmp: nombreArchivoTmp, nombreOriginal: nombreOriginal },
            success: function (response) {
                //Ocultamos Loader
                dxLoaderPanel.hide();

                //Descargamos el archivo
                window.open(API_FICHA + 'descargarArchivo/');
            },
            error: function (response, status, error) {
                //Ocultamos Loader
                dxLoaderPanel.hide();

                //Mostramos mensaje de error
                toast("Error:\n" + response.responseText, 'error');
            }
        });
    } else {
        toast("No se pudo descargar el archivo.", "error");

        return;
    }
}

function dropZoneEnter(e) {
    if (e.dropZoneElement.id === "dropzone-external") {
        toggleDropZoneActive(e.dropZoneElement, true);
    }
}

function dropZoneLeave(e) {
    if (e.dropZoneElement.id === "dropzone-external") {
        toggleDropZoneActive(e.dropZoneElement, false);
    }
}

function toggleDropZoneActive(dropZone, activo) {
    if (activo) {
        dropZone.classList.add("dx-theme-accent-as-border-color");
        dropZone.classList.remove("dx-theme-border-color");
        dropZone.classList.add("dropzone-active");
    } else {
        dropZone.classList.remove("dx-theme-accent-as-border-color");
        dropZone.classList.add("dx-theme-border-color");
        dropZone.classList.remove("dropzone-active");
    }
}

function toggleVisible(visible) {
    $("#dropzone-file")[0].hidden = !visible;
}

var cambiarDropZone = function (e) {
    file = e.value[0];

    const fileReader = new FileReader();

    fileReader.onload = function () {
        toggleDropZoneActive($("#dropzone-external")[0], false);
        dropZoneFile.src = fileReader.result;
    }

    fileReader.readAsDataURL(file);

    dropZoneText.style.display = "none";

    toggleVisible(false);

    dxButtonEliminarDropZone.option("visible", true);
    dxTxtFileName.option("visible", true);
    dxTxtFileName.option("value", file.name);
    $('#dropzone-external').hide();
}

var eliminarDropZone = function () {
    file = null;

    dropZoneFile.src = "";
    dropZoneText.style.display = "";

    dxButtonEliminarDropZone.option("visible", false);
    dxTxtFileName.option("visible", false);
    dxTxtFileName.option("value", null);
    $('#dropzone-external').show();
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

var guardaCambios = function () {
    if (cambios === false) {
        //Mostramos Loader
        dxLoaderPanel.show();

        //Mostramos mensaje de Exito
        toast("Registro guardado con exito!", 'success');

        //Regresamos al listado
        regresarListado();
    } else {
        //Respaldamos las tablas
        respaldarTablas();

        //Validamos que la informacion requerida del Formulario este completa
        if (!dxForm.validate().isValid) {
            toast("Favor de completar los campos requeridos.", 'error');

            return;
        }

        //Validamos los registros que hay en el dxGridDetalles
        var detalles = getDetalles();

        if (!detalles || !detalles.length) {
            toast("No es posible guardar sin artículos para la invitación.", 'error');

            return;
        }

        //Validamos que cada detalle tenga un ganador
        var mensajeProductoSinGanador = "";

        detalles.forEach(articulo => {
            var isGanador = false;
            var proveedorGanador = getGanadoresProveedores().find(x => x.InvitacionCompraDetalleId === articulo.InvitacionCompraDetalleId);

            Object.keys(proveedorGanador).forEach(propiedad => {
                if (propiedad != "InvitacionCompraDetallePrecioProveedorId"
                    && propiedad != "InvitacionCompraDetalleId"
                    && propiedad != "ProductoId"
                    && propiedad != "Producto"
                    && propiedad != "Descripcion"
                    && propiedad != "Comentario") {

                    //Validamos si hay un ganador
                    isGanador = proveedorGanador[propiedad] === true ? true : isGanador;
                }
            });

            if (!proveedorGanador || isGanador === false) {
                mensajeProductoSinGanador = "Favor de seleccionar un proveedor ganador para el artículo [" + articulo.ProductoId + " - " + articulo.Descripcion + "].";
            }
        });

        if (mensajeProductoSinGanador) {
            toast(mensajeProductoSinGanador, 'error');

            mostrarGanadores();

            return;
        }

        //Agregamos los registros borrados, para eliminarlos en base de datos
        detalles = $.merge(detalles, articulosEliminados);

        //Validamos los registros que hay en el dxGridProveedoresInvitados
        var proveedoresInvitados = getProveedoresInvitados();

        if (!proveedoresInvitados || !proveedoresInvitados.length) {
            toast("Favor de seleccionar los proveedores para la invitación.", 'error');

            mostrarProveedores();

            return;
        }

        //Validamos los registros que hay en el dxGridCotizacionesProveedor
        var proveedorCotizaciones = getCotizacionesProveedor();

        if (!proveedorCotizaciones || !proveedorCotizaciones.length) {
            toast("Favor de adjuntar los archivos de las cotizaciones.", 'error');

            mostrarCotizaciones();

            return;
        }

        var proveedoresInvitados = [];

        //Obtenemos todos los registros que hay en el dxGridListadoProveedores
        var proveedores;
        dxGridListadoProveedores.getDataSource().store().load().done((res) => { proveedores = res; });

        proveedores.forEach(proveedor => {
            if (proveedor.Seleccionado === true) {
                //Asignamos el estus
                proveedor.EstatusId = ESTATUS_REGISTRO_ACTIVO;

                //Agregamos los archivos de la cotización para cada proveedor
                proveedor.Cotizaciones = [];

                proveedorCotizaciones.forEach(cotizacion => {
                    if (cotizacion.NombreArchivoTmp && cotizacion.InvitacionCompraProveedorId === proveedor.InvitacionCompraProveedorId) {
                        cotizacion.CotizacionId = null;
                        cotizacion.FechaCotizacion = cotizacion.FechaCotizacion.toLocaleString();
                        cotizacion.EstatusId = ESTATUS_REGISTRO_ACTIVO;

                        proveedor.Cotizaciones.push(cotizacion);
                    }
                });

                proveedoresInvitados.push(proveedor);
            } else if (proveedor.Seleccionado === false && proveedor.InvitacionCompraProveedorId > 0) {
                //Asignamos el estus
                proveedor.EstatusId = ESTATUS_REGISTRO_BORRADO;

                proveedoresInvitados.push(proveedor);
            }
        });

        //Validamos los registros que hay en el dxGridProveedoresInvitados
        var preciosProveedorTemp = getPreciosProveedores();

        if (!preciosProveedorTemp || !preciosProveedorTemp.length) {
            toast("Favor de completar los precios por artículo para cada proveedor.", 'error');

            mostrarCotizaciones();

            return;
        }

        var contadorPreciosProveedor = 0;
        var mensajePrecioProveedor = "";
        var preciosProveedor = [];

        preciosProveedorTemp.forEach(precioProveedor => {
            var proveedorGanador = getGanadoresProveedores().find(x => x.InvitacionCompraDetallePrecioProveedorId === precioProveedor.InvitacionCompraDetallePrecioProveedorId);
            var detalle = getDetalles().find(x => x.InvitacionCompraDetalleId === precioProveedor.InvitacionCompraDetalleId);

            var proveedores = [];

            Object.keys(precioProveedor).forEach(propiedad => {
                if (propiedad != "InvitacionCompraDetallePrecioProveedorId"
                    && propiedad != "InvitacionCompraDetalleId"
                    && propiedad != "ProductoId"
                    && propiedad != "Producto"
                    && propiedad != "Descripcion"
                    && propiedad != "Ganador"
                    && propiedad != "Comentario") {

                    if (detalle.PermiteEditar && (!precioProveedor[propiedad] || precioProveedor[propiedad] <= 0)) {
                        mensajePrecioProveedor = "El precio del artículo [" + precioProveedor.Producto + "] para el proveedor [" + propiedad + "] debe ser mayor a 0.";
                    }

                    var proveedor = {
                        Id: propiedad.substring(0, propiedad.indexOf(" - ")),
                        PrecioArticulo: precioProveedor[propiedad],
                        Ganador: proveedorGanador[propiedad],
                        Comentario: proveedorGanador.Comentario
                    };

                    proveedores.push(proveedor);
                }
            });

            proveedores.forEach(proveedor => {
                contadorPreciosProveedor -= 1;

                var proveedorId = Number.parseInt(proveedor.Id);

                var existe = _listPreciosProveedores.find(x => x.InvitacionCompraDetalleId === precioProveedor.InvitacionCompraDetalleId && x.ProveedorId === proveedorId);

                var modelo = {
                    InvitacionCompraDetallePrecioProveedorId: existe ? existe.InvitacionCompraDetallePrecioProveedorId : contadorPreciosProveedor,
                    InvitacionCompraDetalleId: precioProveedor.InvitacionCompraDetalleId,
                    ProveedorId: proveedorId,
                    PrecioArticulo: proveedor.PrecioArticulo,
                    Ganador: proveedor.Ganador,
                    Comentario: proveedor.Ganador === true ? proveedor.Comentario : null,
                    EstatusId: ESTATUS_REGISTRO_ACTIVO
                };

                if (detalle.PermiteEditar) {
                    preciosProveedor.push(modelo);
                }
            });
        });

        //Añadimos los Precios por Proveedor Eliminados
        _listPreciosProveedores.forEach(precioProveedor => {
            var detalle = getDetalles().find(x => x.InvitacionCompraDetalleId === precioProveedor.InvitacionCompraDetalleId);

            if (detalle.PermiteEditar) {
                var existe = preciosProveedor.find(x => x.InvitacionCompraDetalleId === precioProveedor.InvitacionCompraDetalleId && x.ProveedorId === precioProveedor.ProveedorId);

                if (!existe) {
                    precioProveedor.EstatusId = ESTATUS_REGISTRO_BORRADO;

                    preciosProveedor.push(precioProveedor);
                }
            }
        });

        if (mensajePrecioProveedor) {
            toast(mensajePrecioProveedor, 'error');

            mostrarCotizaciones();

            return;
        }

        //Mostramos Loader
        dxLoaderPanel.show();

        $.ajax({
            type: "POST",
            url: API_FICHA + "guardaCambios",
            data: {
                invitacionCompra: getForm(),
                detallesEliminados: articulosEliminados,
                proveedoresInvitados: proveedoresInvitados,
                preciosProveedor: preciosProveedor,
                cotizacionesEliminadas: cotizacionesEliminadas
            },
            success: function (response) {
                //Mostramos mensaje de Exito
                toast("Registro guardado con exito!", 'success');

                //Regresamos al listado
                regresarListado();
            },
            error: function (response, status, error) {
                //Ocultamos Loader
                dxLoaderPanel.hide();

                //Mostramos mensaje de error
                toast("Error al guadar:\n" + response.responseText, 'error');
            }
        });
    }
}

var validaConvertirOC = function (event) {
    invitacionOC = event.row.data;

    if (!invitacionOC.FechaRecepcion) {
        toast("Favor de seleccionar la Fecha de Recepción para el Proveedor [ " + invitacionOC.Proveedor + " ].", 'error');
        invitacionOC = null;

        return;
    }

    invitacionOC.FechaRecepcion = invitacionOC.FechaRecepcion.toLocaleString();

    //Mostramos el modal de confirmacion
    modalConfirmaOC.modal('show');
}

var convertirOC = function () {
    //Mostramos Loader
    dxLoaderPanel.show();

    $.ajax({
        type: "POST",
        url: API_FICHA + "convertirOC",
        data: { invitacionCompra: getForm(), invitacionOC: invitacionOC },
        success: function (response) {
            //Mostramos mensaje de Exito
            toast("Registro guardado con exito!", 'success');

            //Recargamos la ficha
            recargarFicha();
        },
        error: function (response, status, error) {
            //Limpiamos la OC
            invitacionOC = null;

            //Ocultamos Loader
            dxLoaderPanel.hide();

            //Mostramos mensaje de error
            toast("Error al guadar:\n" + response.responseText, 'error');
        }
    });
}

var validaEliminar = function () {
    //Mostramos el modal de confirmacion
    modalConfirmaEliminar.modal('show');
}

var eliminaOC = function () {
    //Mostramos Loader
    dxLoaderPanel.show();

    //Hacemos la petición para eliminar el registro
    $.ajax({
        type: "POST",
        url: API_FICHA + "eliminarPorModelo",
        data: { invitacionCompra: getForm() },
        success: function (response) {
            // Ocultamos Loader
            dxLoaderPanel.hide();

            //Mostramos mensaje de Exito
            toast("Registro eliminado con exito!", 'success');

            //Regresamos al listado
            regresarListado();
        },
        error: function (response, status, error) {
            // Ocultamos Loader
            dxLoaderPanel.hide();

            //Mostramos mensaje de error
            toast("Error al eliminar:\n" + response.responseText, 'error');
        }
    });
}

var getTipoArchivo = function (extension) {
    switch (extension) {
        case ".doc":
        case ".docm":
        case ".docx":
        case ".dot":
        case ".dotm":
        case ".dotx":
        case ".txt":
            return 'Documento de texto';
        case ".xlsx":
        case ".xlsm":
        case ".xlsb":
        case ".xltx":
        case ".xltm":
        case ".xls":
        case ".xlt":
        case ".xlam":
        case ".xla":
        case ".xlw":
        case ".xlr":
        case ".csv":
            return 'Hoja de cálculo';
        case ".xml":
            return 'XML';
        case ".pdf":
            return 'PDF';
        case ".bmp":
        case ".gif":
        case ".jpg":
        case ".jpeg":
        case ".tif":
        case ".tiff":
        case ".png":
            return 'Imagen';
        default:
            return 'Otro';
    }
}

var regresarListado = function () {
    window.location.href = API_FICHA + "listar";
}

var recargarFicha = function () {
    //Recargamos la ficha según si es registro nuevo o se está editando
    window.location.href = API_FICHA + (invitacionCompraId == 0 ? "nuevo" : "editar/" + invitacionCompraId);
}

var toast = function (message, type) {
    DevExpress.ui.notify({ message: message, width: "auto" }, type, 5000);
}