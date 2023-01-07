// Variables Globales //
var contadorRegistrosNuevos,
    modalConfiguracionMontoCompra,
    modalConfirmaDeshacer,
    modalConfirmaEliminar,
    dxFormConfiguracionMontoCompra,
    dxDataGridConfiguracionMontoCompra,
    dxButtonDeshacer,
    dxButtonGuardar,
    dxPopupDeshacer,
    dxNumberBoxMontoMinimo,
    dxNumberBoxMontoMaximo,
    rowEliminar;
////////////////////////

// Function Default //
$(() => {
    // Inicializamos las variables para la ficha
    inicializaVariables();

    // Inhabilitamos los botones de acciones
    habilitaComponentes(false);
});
//////////////////////

inicializaVariables = () => {
    contadorRegistrosNuevos = 0;
    modalConfiguracionMontoCompra = $('#modalConfiguracionMontoCompra');
    modalConfirmaDeshacer = $('#modalConfirmaDeshacer');
    modalConfirmaEliminar = $('#modalConfirmaEliminar');
    dxFormConfiguracionMontoCompra = $("#dxFormConfiguracionMontoCompra").dxForm("instance");
    dxDataGridConfiguracionMontoCompra = $("#dxDataGridConfiguracionMontoCompra").dxDataGrid("instance");
    dxButtonDeshacer = $("#dxButtonDeshacer").dxButton("instance");
    dxButtonGuardar = $("#dxButtonGuardar").dxButton("instance");
    dxNumberBoxMontoMinimo = $("#dxNumberBoxMontoMinimo").dxNumberBox("instance");
    dxNumberBoxMontoMaximo = $("#dxNumberBoxMontoMaximo").dxNumberBox("instance");
}

habilitaComponentes = (enabled) => {
    dxButtonDeshacer.option("disabled", !enabled);
    dxButtonGuardar.option("disabled", !enabled);
}

// Modal //
nuevoModal = () => {

    //Inicializamos el modelo del Form
    dxFormConfiguracionMontoCompra.option("formData", $.extend(true, {}, _controlMaestroConfiguracionMontoCompra));

    // Limpian los campos del form
    dxFormConfiguracionMontoCompra.resetValues();

    //Asignamos un ProspectoProveedorId al Form
    dxFormConfiguracionMontoCompra.updateData("ConfiguracionMontoId", contadorRegistrosNuevos);

    // Decrementamos el contador de Registros para el siguiente nuevo registro
    contadorRegistrosNuevos -= 1;

    // Cambiamos el titulo en el modal
    modalConfiguracionMontoCompra.find(".modal-title").text("Nueva Configuración");

    // Cambiamos el modo editar es falso en el modal
    modalConfiguracionMontoCompra.attr("isEdit", false);

    // Mosntramos el modal
    modalConfiguracionMontoCompra.modal('show');
}

editaModal = (event) => {
    //Obtenemos una copia del objeto a modificar
    var controlMaestroConfiguracionMontoCompra = $.extend(true, {}, event.row.data);

    //Le pasamos el objeto al Form para que cargue sus valores
    dxFormConfiguracionMontoCompra.option("formData", controlMaestroConfiguracionMontoCompra);

    // Cambiamos el titulo en el modal
    modalConfiguracionMontoCompra.find(".modal-title").text("Editar Configuración");

    // Cambiamos el modo editar es verdad en el modal
    modalConfiguracionMontoCompra.attr("isEdit", true);

    //Mostramos el modal
    modalConfiguracionMontoCompra.modal('show');
}

guardaCambiosModal = () => {

    // Validamos que la informacion requerida del Formulario este completa
    if (!dxFormConfiguracionMontoCompra.validate().isValid)
        return;

    // Obtenemos el Objeto que se esta creando/editando en el Form
    var controlMaestroConfiguracionMontoCompra = dxFormConfiguracionMontoCompra.option("formData");

    // La validación
    if (!esValidacion(controlMaestroConfiguracionMontoCompra)) {
        return;
    }

    // Obtenemos la instancia store del DataSource
    var store = dxDataGridConfiguracionMontoCompra.getDataSource().store();

    // Si el modo editar es falso (Nuevo) o si es verdad (Editar)
    if (modalConfiguracionMontoCompra.attr("isEdit") == "false") {
        // Nuevo
        store.insert(controlMaestroConfiguracionMontoCompra)
            .done(function () {

                // Recargamos la informacion de la tabla
                dxDataGridConfiguracionMontoCompra.getDataSource().reload();

                // Habilitamos los botones de acciones
                habilitaComponentes(true);

                // Ocultamos el modal
                modalConfiguracionMontoCompra.modal('hide');
            })
            .fail(function () {
                toast("No se pudo agregar el nuevo registro a la tabla.", "error");
            });
    } else {
        // Editar
        store.update(controlMaestroConfiguracionMontoCompra.ConfiguracionMontoId, controlMaestroConfiguracionMontoCompra)
            .done(function () {
                // Recargamos la informacion de la tabla
                dxDataGridConfiguracionMontoCompra.getDataSource().reload();

                // Habilitamos los botones de acciones
                habilitaComponentes(true);

                // Ocultamos el modal
                modalConfiguracionMontoCompra.modal('hide');
            })
            .fail(function () {
                toast("No se pudo actualizar el registro en la tabla.", "error");
            });
    }
}

confirmaEliminarModal = (event) => {
    // Obtenemos una copia del objeto a eliminar
    rowEliminar = $.extend(true, {}, event.row.data);

    // Mostramos el modal de confirmacion
    modalConfirmaEliminar.modal('show');
}
///////////

eliminaRow = () => {
    // Ocultamos el modal confirma eliminar
    modalConfirmaEliminar.modal('hide');

    //Obtenemos la instancia store del DataSource
    var store = dxDataGridConfiguracionMontoCompra.getDataSource().store();

    //Eliminamos el registro de la tabla
    if (rowEliminar != null) {
        // Si el registro viene de la base de datos
        // para posteriormente eliminarlo en la base de datos
        if (rowEliminar.ConfiguracionMontoId > 0) {
            // Actualizamos el estatus del registro a "Borrado"
            rowEliminar.EstatusId = ControlMaestroMapeo.EstatusRegistro.BORRADO;

            store.update(rowEliminar.ConfiguracionMontoId, rowEliminar)
                .done(function () {
                    //Recargamos la informacion de la tabla
                    dxDataGridConfiguracionMontoCompra.getDataSource().reload();

                    // Habilitamos los botones de acciones
                    habilitaComponentes(true);

                })
                .fail(function () {
                    toast("No se pudo actualizar el registro en la tabla.", "error");
                });
        } else {
            store.remove(rowEliminar.ConfiguracionMontoId)
                .done(function () {
                    // Recargamos la informacion de la tabla
                    dxDataGridConfiguracionMontoCompra.getDataSource().reload();

                    // Habilitamos los botones de acciones
                    habilitaComponentes(true);

                })
                .fail(function () {
                    toast("No se pudo eliminar el registro de la tabla.", "error");
                });
        }

        // Limpiar Row Eliminar
        rowEliminar = null;
    }
}

guardaCambios = () => {
    // Mostramos Loader
    dxLoaderPanel.show();

    var listControlMaestroConfiguracionMontoCompra;

    //Obtenemos todos los registros que hay en el DataGrid
    dxDataGridConfiguracionMontoCompra.getDataSource().store().load().done(response => listControlMaestroConfiguracionMontoCompra = response);

    //Enviamos la informacion al controlador
    $.ajax({
        type: "POST",
        url: "/compras/catalogos/configuracionmontocompra/guardar",
        data: { listControlMaestroConfiguracionMontoCompra },
        success: function (response) {
            //Mostramos mensaje de Exito
            toast(response, 'success');

            //Recargamos la ficha
            window.location.href = '/compras/catalogos/configuracionmontocompra/listar';
        },
        error: function (response, status, error) {
            //Ocultamos Loader
            dxLoaderPanel.hide();

            //Mostramos mensaje de error
            toast(response.responseText, "error");
        }
    });
}

recargarFicha = () => {
    modalConfirmaDeshacer.modal('hide');
    window.location.href = '/compras/catalogos/configuracionmontocompra/listar';
}

// Event onChange //
onChangeTipoCompra = (params) => {
    var category = params.value;
    if (category != null) {
        dxFormConfiguracionMontoCompra.itemOption("isTipoCompra", "visible", true);

        // Control Maestro: Tipo Compra -> Compra Directa -> 4
        if (category == ControlMaestroMapeo.TipoCompra.COMPRA_DIRECTA) {
            dxFormConfiguracionMontoCompra.itemOption("isCompraDirecta", "visible", false);
            dxFormConfiguracionMontoCompra.updateData("NumeroMinimoProveedores", null);
        } else {
            dxFormConfiguracionMontoCompra.itemOption("isCompraDirecta", "visible", true);
        }

        // dxNumberBox Instance
        dxNumberBoxMontoMinimo = $("#dxNumberBoxMontoMinimo").dxNumberBox("instance");
        dxNumberBoxMontoMaximo = $("#dxNumberBoxMontoMaximo").dxNumberBox("instance");

    } else {
        dxFormConfiguracionMontoCompra.itemOption("isTipoCompra", "visible", false);
        dxFormConfiguracionMontoCompra.itemOption("isCompraDirecta", "visible", false);
    }

}

onChangeSinLimite = (params) => {
    var value = params.value;

    // Is Sin Límite
    if (value) {
        dxNumberBoxMontoMaximo.option({ "value": null, "disabled": true });
    } else {
        dxNumberBoxMontoMaximo.option({ "value": dxNumberBoxMontoMinimo.option("value"), "disabled": false });
    }


}
////////////////////

// Event onValue //
onValueNuermoMinioProveddores = (params) => {
    var value = params.NumeroMinimoProveedores;
    if (value == null) {
        return "N/A";
    }
    return value;
}

onValueMontoMaximo = (params) => {
    var value = params.MontoMaximo;
    if (value == null) {
        return "Sin límite";
    }
    return value;
}
///////////////////

// Event onFocus //
onFocusOutMontoMinimo = (params) => {
    var valueMontoMinimo = dxNumberBoxMontoMinimo.option("value");
    if (dxNumberBoxMontoMaximo.option("value") == 0 || dxNumberBoxMontoMaximo.option("value") == null) {
        dxNumberBoxMontoMaximo.option("value", valueMontoMinimo);
    }
}

onFocusOutMontoMaximo = (params) => {
    var valueMontoMinimo = dxNumberBoxMontoMinimo.option("value");

    // Monto Maximo no menor que Monto Minimo
    if (dxNumberBoxMontoMaximo.option("value") <= valueMontoMinimo) {
        dxNumberBoxMontoMaximo.option("value", valueMontoMinimo);
    }
}
///////////////////


// Las Validaciones //
esValidacion = (controlMaestroConfiguracionMontoCompra) => {

    // Variable
    var listControlMaestroConfiguracionMontoCompra;

    // Obtenemos todos los registros que hay en el DataGrid
    dxDataGridConfiguracionMontoCompra.getDataSource().store().load().done(response => listControlMaestroConfiguracionMontoCompra = response.filter(filter => filter.EstatusId == 1));

    // Tipo Compra
    if (listControlMaestroConfiguracionMontoCompra.some(cmc => cmc.ConfiguracionMontoId != controlMaestroConfiguracionMontoCompra.ConfiguracionMontoId && cmc.TipoCompraId == controlMaestroConfiguracionMontoCompra.TipoCompraId)) {
        var dxSelectBoxTipoCompra = dxFormConfiguracionMontoCompra.getEditor("TipoCompraId");
        dxSelectBoxTipoCompra.option({
            validationStatus: 'invalid',
            validationError: { type: "custom", message: "Ya esta registrado tipo de compra: " + _listTipoCompra.find(tc => tc.ControlId == controlMaestroConfiguracionMontoCompra.TipoCompraId).Valor }
        });
        //toast("Ya esta registrado tipo de compra: " + _listTipoCompra.find(cmc => cmc.ControlId == controlMaestroConfiguracionMontoCompra.TipoCompraId).Valor, "warning");
        return false;
    }
    for (const cmc of listControlMaestroConfiguracionMontoCompra.filter(cmc => cmc.TipoCompraId != controlMaestroConfiguracionMontoCompra.TipoCompraId)) {

        // Verify Monto Minimo
        if (cmc.MontoMinimo <= controlMaestroConfiguracionMontoCompra.MontoMinimo && cmc.MontoMaximo >= controlMaestroConfiguracionMontoCompra.MontoMinimo) {
            // Monto Minimo
            toast("El monto mínimo ya existe el tipo de compra: " + _listTipoCompra.find(tc => tc.ControlId == cmc.TipoCompraId).Valor + ", el menor: $" + (cmc.MontoMinimo - .01).toFixed(2) + " atras o el mayor: $" + (cmc.MontoMaximo + .01).toFixed(2) + " adelante.", "warning");
            return false;
        }

        // Verify Monto Maximo
        if (cmc.MontoMinimo <= controlMaestroConfiguracionMontoCompra.MontoMaximo && (cmc.MontoMaximo >= controlMaestroConfiguracionMontoCompra.MontoMaximo || cmc.MontoMaximo <= controlMaestroConfiguracionMontoCompra.MontoMaximo)) {
            // Monto Minimo
            if (cmc.MontoMinimo >= controlMaestroConfiguracionMontoCompra.MontoMinimo) {
                toast("El monto máximo ya existe el tipo de compra: " + _listTipoCompra.find(tc => tc.ControlId == cmc.TipoCompraId).Valor + ", el menor: $" + (cmc.MontoMinimo - .01).toFixed(2) + " atras.", "warning");
                return false;
            }

            // Monto Maximo
            //if(_controlMaestroConfiguracionMontoCompra.MontoMaximo <= controlMaestroConfiguracionMontoCompra.MontoMaximo)
            //throw new Exception("El monto máximo ya existe el tipo de compra: " + controlMaestroConfiguracionMontoCompraViewModel.ListTipoCompra.Find(cmc => cmc.ControlId == _controlMaestroConfiguracionMontoCompra.TipoCompraId).Valor + ", el mayor: " + String.Format("$ {0:#,##0.##}", (_controlMaestroConfiguracionMontoCompra.MontoMaximo + decimal.Parse(".01"))) + " adelante.");

        }
    }

    return true;
}
/////////////////

// Toast //
toast = (message, type) => {
    DevExpress.ui.notify({ message: message, width: "auto" }, type, 1500);
}
///////////