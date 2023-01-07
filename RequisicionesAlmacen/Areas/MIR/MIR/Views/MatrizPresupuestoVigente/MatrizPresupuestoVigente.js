// VARIABLES GLOBALES //
var contadorRegistrosNuevos,
    // Modal
    modalCerrar,
    modalConfirmaRecalcularMeses,
    modalConfirmaGuardar,
    // Form
    dxFormMatrizConfiguracionPresupuestal,
    // DataGrid
    dxDataGridPorEjercer,
    // Listas
    listaMatrizConfiguracionPresupuestalDetalle = [],
    // API
    API_FICHA = '/mir/mir/matrizpresupuestovigente/';

let nombreMes = '';

//////////////////////

// Function Default //
$(() => {
    // Inicializamos las variables para la ficha
    inicializaVariables();
});
//////////////////////

inicializaVariables = () => {

    contadorRegistrosNuevos = -1;
    // Modal
    modalCerrar = $('#modalCerrar');
    modalConfirmaRecalcularMeses = $('#modalConfirmaRecalcularMeses');
    modalConfirmaGuardar = $('#modalConfirmaGuardar');
    // Form
    dxFormMatrizConfiguracionPresupuestal = $('#dxFormMatrizConfiguracionPresupuestal').dxForm('instance');
    // DataGrid
    //dxDataGridPorEjercer = $('#dxDataGridPorEjercer').dxDataGrid('instance');
    // Listas
    //listMatrizConfiguracionPresupuestalDetalle = _listMatrizIndicadorResultadoIndicador ? $.extend(true, [], _listMatrizIndicadorResultadoIndicador) : [];

    dxFormMatrizConfiguracionPresupuestal.updateData("Codigo", _consultaMatrizIndicadorResultado.Codigo);
    dxFormMatrizConfiguracionPresupuestal.updateData("ProgramaPresupuestario", _consultaMatrizIndicadorResultado.ProgramaPresupuestario);

    // Carga listados para MCP y MCPD
    //cargaListados();
}

onInitializedDataGridPorEjercer = (event) => {
    dxDataGridPorEjercer = event.component;
    setTimeout(() => {
        // Carga listados para MCP y MCPD
        cargaListados();
    }, 0);
}

cargaListados = () => {
    const mcpdModel = {
        ConfiguracionPresupuestoDetalleId: null,
        ConfiguracionPresupuestoId: null,
        ClasificadorId: null,
        MIRIndicadorId: null,
        Enero: null,
        Febrero: null,
        Marzo: null,
        Abril: null,
        Mayo: null,
        Junio: null,
        Julio: null,
        Agosto: null,
        Septiembre: null,
        Octubre: null,
        Noviembre: null,
        Diciembre: null,
        Anual: null,
        Porcentaje: null,
        Componente: '',
        Actividad: '',
        EsProyecto: false,
        CabeceraMeses: {
            Enero: null,
            Febrero: null,
            Marzo: null,
            Abril: null,
            Mayo: null,
            Junio: null,
            Julio: null,
            Agosto: null,
            Septiembre: null,
            Octubre: null,
            Noviembre: null,
            Diciembre: null,
            Anual: null,
            Porcentaje: null
        },
        MontoIndicador: null,
        EsEditado: false,
        Modifica: {
            ContadorMes: 12,
            TotalMontoMesesIndicadores: 0
        }
    }, listaMCPD = [];

    // Asignamos el campo nombreMes para saber inicio de mes
    if (_listaConsultaPresupuestoVigente.length > 0)
        nombreMes = _listaConsultaPresupuestoVigente[0].InicioMes;

    _listaConsultaPresupuestoVigente.map(mcpd => {
        // Variables
        let _mcpdModel = $.extend(true, {}, Object.assign(mcpdModel, mcpd));

        // Asignamos Anual y Mes por la cabecera
        _mcpdModel.CabeceraMeses.Anual = mcpd.CabeceraAnual;
        _mcpdModel.CabeceraMeses[nombreMes] = mcpd.CabeceraMes;

        // Total porcentaje para asignar el campo Porcentaje
        _mcpdModel.CabeceraMeses.Porcentaje = mcpd.TotalPorcentaje;

        // Hacer suma del mes en la cabecera
        bucle:
        for (let x = 1; x <= 12; x++) {
            var mes = funcionObtenerMes(x);
            if (nombreMes != mes) {
                let totalFila = 0;
                _listaConsultaPresupuestoVigente.filter(mcpd_ => mcpd_.MIRIndicadorComponenteId == mcpd.MIRIndicadorComponenteId).map(mcpd_ => {
                    totalFila += mcpd_[mes];
                });
                totalFila = parseFloat(totalFila.toFixed(2));
                _mcpdModel.CabeceraMeses[mes] = totalFila; 
            } else {
                break bucle;
            }
        }

        // Agregamos los datos al lista
        listaMCPD.push(_mcpdModel);
    });

    const listaPorEjercer = listaMCPD.filter(mcpd => mcpd.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.POR_EJERCER);
    var dataSourcePorEjercer = new DevExpress.data.DataSource({
        store: {
            type: 'array',
            key: 'ConfiguracionPresupuestoDetalleId',
            data: listaPorEjercer
        },
        filter: [['ClasificadorId', '=', ControlMaestroMapeo.TipoPresupuesto.POR_EJERCER]]
    });
    dxDataGridPorEjercer.option('dataSource', dataSourcePorEjercer);
    // Actualizamos el campo Presupuesto
    actualizaPresupuesto();
}


actualizaPresupuesto = () => {
    var items = [], totalPresupuesto = 0;
    dxDataGridPorEjercer.getDataSource().store().load({ filter: ['ClasificadorId', '=', ControlMaestroMapeo.TipoPresupuesto.POR_EJERCER] }).done(response => items = response).fail(() => { toast("Error los datos, intentalo mas tarde.", "error"); return; });
    items.map(_item => {
        for (let x = 1; x <= 12; x++) {
            var mes = funcionObtenerMes(x);
            if (_item[mes] != null)
                totalPresupuesto += _item[mes];
        }
    });
    dxFormMatrizConfiguracionPresupuestal.getEditor('PresupuestoPorEjercer').option('value', totalPresupuesto);
}

calculateCustomSummary = (event) => {
    if (event.name) {
        if (event.name == 'EneroCabecera' || event.name == 'FebreroCabecera' || event.name == 'MarzoCabecera' || event.name == 'AbrilCabecera' || event.name == 'MayoCabecera' || event.name == 'JunioCabecera' || event.name == 'JulioCabecera' || event.name == 'AgostoCabecera' || event.name == 'SeptiembreCabecera' || event.name == 'OctubreCabecera' || event.name == 'NoviembreCabecera' || event.name == 'DiciembreCabecera' || event.name == 'AnualCabecera') {
            if (event.summaryProcess == 'calculate') {
                event.totalValue = event.value.CabeceraMeses[event.name.replace('Cabecera', '')];
            }
        }
        if (event.name == 'Porcentaje') {
            if (event.summaryProcess == 'calculate')
                if (event.value.EsProyecto)
                    event.totalValue = event.value.CabeceraMeses[event.name];
        }

        if (event.name == 'Enero' || event.name == 'Febrero' || event.name == 'Marzo' || event.name == 'Abril' || event.name == 'Mayo' || event.name == 'Junio' || event.name == 'Julio' || event.name == 'Agosto' || event.name == 'Septiembre' || event.name == 'Octubre' || event.name == 'Noviembre' || event.name == 'Diciembre' || event.name == 'Anual') {

            if (event.summaryProcess == 'start') {
                event.totalValue = 0;
                event.esInicia = true;
            }
            if (event.summaryProcess == 'calculate') {
                if (event.esInicia == true) {
                    event.esInicia = false;
                    event.totalValue = event.value.CabeceraMeses[event.name];
                }
                event.totalValue -= event.value[event.name];
                event.totalValue = parseFloat(parseFloat(event.totalValue).toFixed(2));
                if (event.totalValue == -0 || event.totalValue == -0.0 || event.totalValue == -0.00) {
                    event.totalValue = 0;
                }
            }
            if (event.summaryProcess == 'finalize') {
            }
        }
    }
}

onEditingStart = (event) => {
    // valida: Si un mes no esta llenado debe el mensaje de advertencia debe llenar el anterior del mes
    if (event.data.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.POR_EJERCER) {
        if (event.data[event.column.dataField] == null) {
            const obtenerNumeroMes = funcionObtenerNumeroMes(event.column.dataField);

            bucle:
            for (let x = obtenerNumeroMes - 1; x >= 1; x--) {
                var mes = funcionObtenerMes(x);
                if (event.data[mes] != null) {
                    event.cancel = true;
                    toast('No se puede asignar el monto, necesitas guardar el mes ' + mes + '.', 'error');
                    break bucle;
                }
            }
        }
    }

    // Valida: Cuando una vez guardado el mes no se permite editar relacion de indicador con proyecto
    //if (event.data.EsProyecto && event.data.ConfiguracionPresupuestoDetalleId > 0 && event.data.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.POR_EJERCER) {
    //    event.cancel = true;
    //    toast('No se puede editar porque este es el proyecto.', 'Warning');
    //    return;
    //}

    //if (event.data.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.DEVENGADO) {
    //    let listaMCPD = [];
    //    dxDataGridPorEjercer.getDataSource().store().load({ filter: ['Componente', '=', event.data.Componente] }).done(response => listaMCPD = response).fail(() => { toast("Error los datos, intentalo mas tarde.", "error"); return; });
    //    if (!esValidaDevengado(listaMCPD)) {
    //        event.cancel = true;
    //    }
    //}
}

onEditorPreparing = (event) => {
    //console.log(event);
    if (event.parentType == 'dataRow' && event.row) {
        if (_listaControlMaestroControlPeriodo.find(cp => cp.Codigo == funcionObtenerCodigoMes(event.dataField)).EstatusPeriodoId != ControlMaestroMapeo.MIEstatusPeriodo.ABIERTO) {
            toast('El mes ' + event.dataField + ' no se encuentra abierto.', 'warning');
            // Deshabilitar la celda
            event.editorOptions.disabled = true;
        }
        // Establecer Minimo
        event.editorOptions.min = 0;
        // Obtener los datos del DataGrid
        var dataSource = event.component.getDataSource();
        // Función para cuando un campo con editar y concentarse
        event.editorOptions.onValueChanged = (_event) => {
            // Obtener el valor
            const value = _event.component.option('value');
            actualizaDataSource(value, event, dataSource);
        };
    }
}

esEditado = (monto, items, event, dataSource) => {
    // Variables
    const obtenerNumeroMes = funcionObtenerNumeroMes(event.dataField);
    let _obtenerNumeroMes = null;

    if (items.EditorMes != '')
        _obtenerNumeroMes = funcionObtenerNumeroMes(items.EditorMes);

    if (items.EditorMes == '' || (_obtenerNumeroMes != null && !!(obtenerNumeroMes >= _obtenerNumeroMes))) {
        return true;
    } else {
        if (monto > items.CabeceraMeses[event.dataField]) {
            // Establecer Maximo
            items[event.dataField] = items.CabeceraMeses[event.dataField];
            // un mensaje de advertencia
            toast('El monto de ' + items.EditorMes + ' ya esta editado solo se pueden los meses posteriores.');
            // Actualizar el dato a DataGrid
            dataSource.store().update(items.ConfiguracionPresupuestoDetalleId, items).done(() => { dataSource.reload(); return false; }).fail(() => { toast("Error los datos, intentalo mas tarde.", "error"); return; });

        } else {
            return false;
        }
    }
}

actualizaDataSource = (monto, event, dataSource) => {
    // Variable
    var itemId = event.row.data.ConfiguracionPresupuestoDetalleId, item = {}, items = [];

    dataSource.store().load({ filter: [['Componente', '=', event.row.data.Componente], 'and', ['ClasificadorId', '=', event.row.data.ClasificadorId]] }).done(response => items = response).fail(() => { toast("Error los datos, intentalo mas tarde.", "error"); return; });
    // Variables
    const obtenerNumeroMes = funcionObtenerNumeroMes(event.dataField);
    // Obtenemos el ITEM que estas editado
    var item = items.find(_item => _item.ConfiguracionPresupuestoDetalleId == itemId);

    // Tipo Presupuesto -> Por Ejercer
    if (event.row.data.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.POR_EJERCER) {
        let contadorPosterior = 0;
        // Validar si el mes ya esta guardando para eliminar a los meses
        for (let x = obtenerNumeroMes + 1; x <= 12; x++) {
            var mes = funcionObtenerMes(x);
            if (event.row.data[mes] != null)
                contadorPosterior++;
        }
        if (contadorPosterior > 0) {
            // Marcamos el modo editar es false para el nuevo
            modalConfirmaRecalcularMeses.attr("itemId", itemId);
            modalConfirmaRecalcularMeses.attr("items", JSON.stringify(items));
            modalConfirmaRecalcularMeses.attr("mes", event.dataField);
            modalConfirmaRecalcularMeses.attr("monto", monto);
            if (contadorPosterior == 1) {
                modalConfirmaRecalcularMeses.find('.modal-title').text('Si desea recalcular el monto de el mes ya guardado posterior al mes que esta editando.');
            } else {
                modalConfirmaRecalcularMeses.find('.modal-title').text('Si desea recalcular los montos de los meses ya guardados posteriores al mes que esta editando.');
            }
            //Mostramos el modal
            modalConfirmaRecalcularMeses.modal('show');
        } else {
            let totalMontoActividades = monto;
            // Suman actividades para saber si el monto es mayor que anual para pasar
            items.map(_item => {
                bucle:
                for (let x = 1; x <= 12; x++) {
                    var mes = funcionObtenerMes(x);
                    if (item.ConfiguracionPresupuestoDetalleId == _item.ConfiguracionPresupuestoDetalleId) {
                        if (event.dataField != mes) {
                            if (_item[mes] == null)
                                break bucle;
                            totalMontoActividades += _item[mes];
                        }
                    } else {
                        if (_item[mes] == null)
                            break bucle;
                        totalMontoActividades += _item[mes];
                    }
                }
            });
            totalMontoActividades = function2DecimalsRound(totalMontoActividades);
            // Si el monto (meses anteriores y editado) es mayor que Anual
            /*if (totalMA >= items.CabeceraMeses.Anual) {*/
            if (totalMontoActividades >= item.CabeceraMeses.Anual) {
                let ultimoMonto = function2DecimalsRound(item.CabeceraMeses.Anual - (totalMontoActividades - monto));
                // Asignamos el monto de la fila que estoy editando
                items.find(_item => _item.ConfiguracionPresupuestoDetalleId == itemId)[event.dataField] = ultimoMonto;
            } else {
                // Asignamos el monto de la fila que estoy editando
                items.find(_item => _item.ConfiguracionPresupuestoDetalleId == itemId)[event.dataField] = monto;
            }

            // Actualizamos la cabecera
            let actualizaCabeceraMonto = 0;
            items.map(_item => actualizaCabeceraMonto += _item[event.dataField]);
            actualizaCabeceraMonto = function2DecimalsRound(actualizaCabeceraMonto);

            let contadorItems = 1;
            items.map(_item => {
                // Actualizamos Anual
                _item.Anual = 0;
                for (let x = 1; x <= 12; x++) {
                    var mes = funcionObtenerMes(x);
                    if (_item[mes] != null) {
                        _item.Anual += _item[mes];
                    }
                }

                // Actualizamos la cabecera
                _item.CabeceraMeses[event.dataField] = actualizaCabeceraMonto;

                dataSource.store().update(_item.ConfiguracionPresupuestoDetalleId, _item).done(() => { contadorItems == items.length ? dataSource.reload() : contadorItems++ }).fail(() => { toast("Error los datos, intentalo mas tarde.", "error"); return; });

            });
            
            // Actualizamos el presupuesto
            actualizaPresupuesto();
        }
    }
}

cancelaRecalcularMeses = () => {
    var itemId = parseInt(modalConfirmaRecalcularMeses.attr('itemId')), items = JSON.parse(modalConfirmaRecalcularMeses.attr('items')), item = items.find(_item => _item.ConfiguracionPresupuestoDetalleId == itemId);
    if (item.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.POR_EJERCER) {
        var dataSource = dxDataGridPorEjercer.getDataSource();
        dataSource.store().update(item.ConfiguracionPresupuestoDetalleId, item).done(() => { dataSource.reload(); }).fail(() => { toast("Error los datos, intentalo mas tarde.", "error"); return; });
    }
    // Ocultamos el modal
    modalConfirmaRecalcularMeses.modal('hide');
}

actualizaRecalcularMeses = () => {
    var itemId = parseInt(modalConfirmaRecalcularMeses.attr('itemId')),
        items = JSON.parse(modalConfirmaRecalcularMeses.attr('items')),
        monto = parseFloat(modalConfirmaRecalcularMeses.attr('monto')),
        mes = modalConfirmaRecalcularMeses.attr('mes'),
        // Obtenemos el ITEM que estas editado
        item = items.find(_item => _item.ConfiguracionPresupuestoDetalleId == itemId);

    // Cambiamos el valor de nombreMes
    nombreMes = mes;

    if (items.find(_item => _item.ConfiguracionPresupuestoDetalleId == itemId).ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.POR_EJERCER) {
        var dataSource = dxDataGridPorEjercer.getDataSource();
        // Asignamos el NULL a Los meses posteriores en fila y cabecera
        items.map(_item => {
            // Asignamos el monto de la fila que estoy editando
            //if (_item.ConfiguracionPresupuestoDetalleId == itemId)
            //    _item[mes] = monto;
            const obtenerNumeroMes = funcionObtenerNumeroMes(mes);
            
            for (let x = obtenerNumeroMes + 1; x <= 12; x++) {
                var _mes = funcionObtenerMes(x);
                _item[_mes] = null;
                _item.CabeceraMeses[_mes] = null;
            }
        });

        let totalMontoActividades = monto;
        // Suman actividades para saber si el monto es mayor que anual para pasar
        items.map(_item => {
            bucle:
            for (let x = 1; x <= 12; x++) {
                var _mes = funcionObtenerMes(x);
                if (item.ConfiguracionPresupuestoDetalleId == _item.ConfiguracionPresupuestoDetalleId) {
                    if (mes != _mes) {
                        if (_item[_mes] == null)
                            break bucle;
                        totalMontoActividades += _item[_mes];
                    }
                } else {
                    if (_item[mes] == null)
                        break bucle;
                    totalMontoActividades += _item[_mes];
                }
            }
        });

        totalMontoActividades = function2DecimalsRound(totalMontoActividades);
        // Si el monto (meses anteriores y editado) es mayor que Anual
        /*if (totalMA >= items.CabeceraMeses.Anual) {*/
        if (totalMontoActividades >= item.CabeceraMeses.Anual) {
            let ultimoMonto = function2DecimalsRound(item.CabeceraMeses.Anual - (totalMontoActividades - monto));
            // Asignamos el monto de la fila que estoy editando
            items.find(_item => _item.ConfiguracionPresupuestoDetalleId == itemId)[mes] = ultimoMonto;
        } else {
            // Asignamos el monto de la fila que estoy editando
            items.find(_item => _item.ConfiguracionPresupuestoDetalleId == itemId)[mes] = monto;
        }

        // Actualizamos la cabecera
        let actualizaCabeceraMonto = 0;
        items.map(_item => actualizaCabeceraMonto += _item[mes]);
        actualizaCabeceraMonto = function2DecimalsRound(actualizaCabeceraMonto);

        let contadorItems = 1;
        items.map(_item => {
            // Actualizamos Anual
            _item.Anual = 0;
            for (let x = 1; x <= 12; x++) {
                var _mes = funcionObtenerMes(x);
                if (_item[_mes] != null) {
                    _item.Anual += _item[_mes];
                }
            }

            // Actualizamos la cabecera
            _item.CabeceraMeses[mes] = actualizaCabeceraMonto;

            dataSource.store().update(_item.ConfiguracionPresupuestoDetalleId, _item).done(() => { contadorItems == items.length ? dataSource.reload() : contadorItems++ }).fail(() => { toast("Error los datos, intentalo mas tarde.", "error"); return; });
        });

        // Actualizamos el presupuesto
        actualizaPresupuesto();
    }
    // Ocultamos el modal
    modalConfirmaRecalcularMeses.modal('hide');
}

onCellPrepared = (event) => {
    if (event.rowType == 'data' && (event.column.dataField == 'Anual' || event.column.dataField == 'Porcentaje') && event.cellElement || event.rowType == 'groupFooter' && event.cellElement) {
        // Establecer color a celda
        event.cellElement.addClass('pixvs-datagrid-cell');
        if (event.columnIndex != 0 || event.columnIndex != 1) {
            // Establecer border a celda
            event.cellElement.addClass('pixvs-datagrid-cell-border');
        }
    }
    // La celda con color amarillo cuando el monto es mayor que diferencia
    if (event.rowType == 'groupFooter' && event.cellElement && event.columnIndex != 1) {
        var item = event.summaryItems ? event.summaryItems[0] : null;
        if (item != null) {
            let monto = parseFloat(item.value.toFixed(2));
            if (monto > 0) {
                event.cellElement.addClass('pixvs-datagrid-cell-money');
            } else {
                event.cellElement.removeClass('pixvs-datagrid-cell-money');
                if (monto < 0) {
                    event.cellElement.addClass('pixvs-datagrid-cell-money-red');
                } else {
                    event.cellElement.removeClass('pixvs-datagrid-cell-money-red');
                }
            }
        }
    }
}

validaPeriodo = () => {
    var estatusPeriodo = _listaControlMaestroControlPeriodo.find(cp => cp.Codigo == funcionObtenerCodigoMes(nombreMes));

    if (estatusPeriodo) {
        if (estatusPeriodo.EstatusPeriodoId != ControlMaestroMapeo.MIEstatusPeriodo.ABIERTO) {
            //modalConfirmaGuardar.attr("listaMCPD", JSON.stringify(listaMCPD));
            modalConfirmaGuardar.modal('show');
        } else {
            guardaCambios();
        }
    } else {
        guardaCambios();
    }
}

modalAceptaPeriodo = () => {
    //var listaMPCD = JSON.parse(modalConfirmaGuardar.attr('listaMCPD'));
    // Ocultamos el modal
    modalConfirmaGuardar.modal('hide');

    //guardaCambios(listaMPCD);
}

guardaCambios = () => {
    // Mostramos Loader
    dxLoaderPanel.show();
    // Crear el objeto array
    let listaMCPD = [];
    // Obtenemos todos los registros que hay en el DataGrid
    dxDataGridPorEjercer.getDataSource().store().load().done(response => response.map(mcpd => listaMCPD.push(mcpd)));
    // Valida
    if (!esValida(listaMCPD)) {
        // Ocultamos Loader
        dxLoaderPanel.hide();
        return;
    }

    // Obtener los datos para saber hay los datos o no hay para guardar
    const data = obtenerData(listaMCPD);
    //console.log(data);
    if (data.matrizConfiguracionPresupuestal == null && data.listaMatrizConfiguracionPresupuestalDetalle.length == 0) {
        toast("No se puede guardar ya que no hubo cambios en la información", "error");
        // Ocultamos Loader
        dxLoaderPanel.hide();
        return;
    }

    $.ajax({
        type: "POST",
        url: API_FICHA + "guardar",
        data: data,
        success: function (response) {
            // Mostramos mensaje de Exito
            toast(response, 'success');
            // Redirigir a listado
            recargarFicha();
            //window.location.href = '/mir/mir/matrizconfiguracionpresupuestal/listar';
        },
        error: function (response, status, error) {
            // Ocultamos Loader
            dxLoaderPanel.hide();
            // Mostramos mensaje de error
            //toast(response.responseText, "error");
            toast('No se puede guardar los cambios, inténtalo de nuevo más tarde', "error");
        }
    });
}

obtenerData = (listaMCPD) => {
    let data = {
        matrizConfiguracionPresupuestal: null,
        listaMatrizConfiguracionPresupuestalDetalle: []
    };
    // NOTA: MCP y MCPD No se puede eliminar
    // Matriz Configuracion Presupuestal
    var matrizConfiguracionPresupuestal = dxFormMatrizConfiguracionPresupuestal.option('formData');
    if (matrizConfiguracionPresupuestal.ConfiguracionPresupuestoId > 0) {
        if (matrizConfiguracionPresupuestal.PresupuestoPorEjercer != _matrizConfiguracionPresupuestal.PresupuestoPorEjercer || matrizConfiguracionPresupuestal.PresupuestoDevengado != _matrizConfiguracionPresupuestal.PresupuestoDevengado) {
            let __matrizConfiguracionPresupuestal = $.extend(true, {}, matrizConfiguracionPresupuestal);
            data.matrizConfiguracionPresupuestal = __matrizConfiguracionPresupuestal;
        }
    } else {
        data.matrizConfiguracionPresupuestal = matrizConfiguracionPresupuestal;
    }
    // Matriz Configuracion Presupuestal Detalle
    // Crear los objectos
    listaMCPD.map(mcpd => {
        // Tipo de Presupuesto -> Por Ejercer
        if (mcpd.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.POR_EJERCER
            ? mcpd.Enero != null
            || mcpd.Febrero != null
            || mcpd.Marzo != null
            || mcpd.Abril != null
            || mcpd.Mayo != null
            || mcpd.Junio != null
            || mcpd.Julio != null
            || mcpd.Agosto != null
            || mcpd.Septiembre != null
            || mcpd.Octubre != null
            || mcpd.Noviembre != null
            || mcpd.Diciembre != null
            :  false) {
            if (mcpd.ConfiguracionPresupuestoDetalleId > 0) {
                const _mcpd = _listaMatrizConfiguracionPresupuestalDetalle.find(__mcpd => __mcpd.ConfiguracionPresupuestoDetalleId == mcpd.ConfiguracionPresupuestoDetalleId);
                if (mcpd.Enero != _mcpd.Enero || mcpd.Febrero != _mcpd.Febrero || mcpd.Marzo != _mcpd.Marzo || mcpd.Abril != _mcpd.Abril || mcpd.Mayo != _mcpd.Mayo || mcpd.Junio != _mcpd.Junio || mcpd.Julio != _mcpd.Julio || mcpd.Agosto != _mcpd.Agosto || mcpd.Septiembre != _mcpd.Septiembre || mcpd.Octubre != _mcpd.Octubre || mcpd.Noviembre != _mcpd.Noviembre || mcpd.Diciembre != _mcpd.Diciembre) {
                    let actualizarMCPD = $.extend(true, {}, _mcpd);
                    actualizarMCPD.Enero = mcpd.Enero;
                    actualizarMCPD.Febrero = mcpd.Febrero;
                    actualizarMCPD.Marzo = mcpd.Marzo;
                    actualizarMCPD.Abril = mcpd.Abril;
                    actualizarMCPD.Mayo = mcpd.Mayo;
                    actualizarMCPD.Junio = mcpd.Junio;
                    actualizarMCPD.Julio = mcpd.Julio;
                    actualizarMCPD.Agosto = mcpd.Agosto;
                    actualizarMCPD.Septiembre = mcpd.Septiembre;
                    actualizarMCPD.Octubre = mcpd.Octubre;
                    actualizarMCPD.Noviembre = mcpd.Noviembre;
                    actualizarMCPD.Diciembre = mcpd.Diciembre;
                    actualizarMCPD.Anual = mcpd.Anual;
                    actualizarMCPD.Porcentaje = mcpd.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.POR_EJERCER ? mcpd.Porcentaje : mcpd.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.DEVENGADO ? null : null;
                    actualizarMCPD.Timestamp = btoa(String.fromCharCode.apply(null, new Uint8Array(_mcpd.Timestamp)));
                    data.listaMatrizConfiguracionPresupuestalDetalle.push(actualizarMCPD);
                }
            } else {
                // Crear el objecto nuevo
                let nuevoMCPD = $.extend(true, {}, _matrizConfiguracionPresupuestalDetalleModel);
                nuevoMCPD.ConfiguracionPresupuestoId = mcpd.ConfiguracionPresupuestoId;
                nuevoMCPD.ClasificadorId = mcpd.ClasificadorId;
                nuevoMCPD.MIRIndicadorId = mcpd.MIRIndicadorId;
                nuevoMCPD.Enero = mcpd.Enero;
                nuevoMCPD.Febrero = mcpd.Febrero;
                nuevoMCPD.Marzo = mcpd.Marzo;
                nuevoMCPD.Abril = mcpd.Abril;
                nuevoMCPD.Mayo = mcpd.Mayo;
                nuevoMCPD.Junio = mcpd.Junio;
                nuevoMCPD.Julio = mcpd.Julio;
                nuevoMCPD.Agosto = mcpd.Agosto;
                nuevoMCPD.Septiembre = mcpd.Septiembre;
                nuevoMCPD.Octubre = mcpd.Octubre;
                nuevoMCPD.Noviembre = mcpd.Noviembre;
                nuevoMCPD.Diciembre = mcpd.Diciembre;
                nuevoMCPD.Anual = mcpd.Anual;
                nuevoMCPD.Porcentaje = mcpd.Porcentaje;
                nuevoMCPD.EstatusId = ControlMaestroMapeo.EstatusRegistro.ACTIVO;
                data.listaMatrizConfiguracionPresupuestalDetalle.push(nuevoMCPD);
            }
        }

    });

    return data;
}

esValida = (listaMCPD) => {
    let _esValida = true,
        // Por Ejercer
        listaPorEjercer = listaMCPD.filter(mcpd => mcpd.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.POR_EJERCER),
        listaComponenteGroupBy = functionGroupBy(listaPorEjercer, 'Componente');

    if (_esValida) {
        bucle:
        for (let y = 0; y < listaComponenteGroupBy.length; y++) {
            let listaComponente = listaPorEjercer.filter(mcpd => mcpd.Componente == listaComponenteGroupBy[y]),
                contadorActividad = listaComponente.length;

            // Validacion: La Diferencia tiene que estar 0 por el mes del anual para pasar si no mandar un mensaje
            for (let x = 1; x <= 12; x++) {
                let totalMes = 0;
                const mes = funcionObtenerMes(x);
                listaComponente.map(componente => totalMes += componente[mes]);
                totalMes = parseFloat(totalMes.toFixed(2));
                if (totalMes > listaComponente[0].CabeceraMeses[mes] || totalMes < listaComponente[0].CabeceraMeses[mes]) {
                    toast('El campo de Diferencia debe estar $0 en el mes ' + mes + ' en Componente: "' + listaComponente[0].Componente + '".', 'error');
                    _esValida = false;
                    break bucle;
                }
            }


            //for (let i = 0; i < contadorActividad; i++) {
            //    if (listaComponente[i].Enero == null && listaComponente[i].Enero >= 0 || listaComponente[i].Febrero == null && listaComponente[i].Febrero >= 0 || listaComponente[i].Marzo == null && listaComponente[i].Marzo >= 0 || listaComponente[i].Abril == null && listaComponente[i].Abril >= 0 || listaComponente[i].Mayo == null && listaComponente[i].Mayo >= 0 || listaComponente[i].Junio == null && listaComponente[i].Junio >= 0 || listaComponente[i].Julio == null && listaComponente[i].Julio >= 0 || listaComponente[i].Agosto == null && listaComponente[i].Agosto >= 0 || listaComponente[i].Septiembre == null && listaComponente[i].Septiembre >= 0 || listaComponente[i].Octubre != null && listaComponente[i].Octubre >= 0 || listaComponente[i].Noviembre == null && listaComponente[i].Noviembre >= 0 || listaComponente[i].Diciembre == null && listaComponente[i].Diciembre >= 0) {
            //        // Meses
            //        for (let mes = 1; mes <= 12; mes++) {
            //            const campoMes = funcionObtenerMes(mes);
            //            if (listaComponente[i][campoMes] < 0 || (i + 1) == contadorActividad) {
            //                // Valida: El mes no debe ser NULL
            //                //if (listaComponente[i][campoMes] == null) {
            //                //    toast('Necesita llenar el mes ' + campoMes + ' en Componente: "' + listaComponente[i].Componente + '".', 'error');
            //                //    valida = false;
            //                //    // Si es devengado directo a pestaña "Por Ejercer"
            //                //    if (seleccionPestaniaIndex == 1)
            //                //        dxTabPanel.option('selectedIndex', 0);
            //                //    // Asignamos a la celda como enfocar
            //                //    const rowIndex = dxDataGridPorEjercer.getRowIndexByKey(listaComponente[i].ConfiguracionPresupuestoDetalleId);
            //                //    dxDataGridPorEjercer.editCell(rowIndex, campoMes);
            //                //    break bucle;
            //                //}
            //                // Valida: El mes debe ser mayor que 0
            //                if (listaComponente[i][campoMes] < 0) {
            //                    toast('El mes ' + campoMes + ' debe ser mayor que $0 en Componente: "' + listaComponente[i].Componente + '".', 'error');
            //                    valida = false;
            //                    // Si es devengado directo a pestaña "Por Ejercer"
            //                    if (seleccionPestaniaIndex == 1)
            //                        dxTabPanel.option('selectedIndex', 0);
            //                    // Asignamos a la celda como enfocar
            //                    const rowIndex = dxDataGridPorEjercer.getRowIndexByKey(listaComponente[i].ConfiguracionPresupuestoDetalleId);
            //                    dxDataGridPorEjercer.editCell(rowIndex, campoMes);
            //                    break bucle;
            //                }
            //                // Valida: El campo de Diferencia no debe haber diferencias que no permite guardar
            //                //if ((i + 1) == contadorActividad) {
            //                //    let totalDiferencia = 0;
            //                //    listaComponente.map(mcpd => { totalDiferencia += mcpd[campoMes] });
            //                //    if (totalDiferencia < listaComponente[i].CabeceraMeses[campoMes]) {
            //                //        toast('El campo de Diferencia debe estar $0 en el mes ' + campoMes + ' en Componente: "' + listaComponente[i].Componente + '".', 'error');
            //                //        valida = false;
            //                //        // Si es devengado directo a pestaña "Por Ejercer"
            //                //        if (seleccionPestaniaIndex == 1)
            //                //            dxTabPanel.option('selectedIndex', 0);
            //                //        break bucle;
            //                //    }
            //                //}
            //            }
            //        }
            //    }
            //}
        }
    }
    return _esValida;
}

// Otro //
regresarListado = () => {
    window.location.href = API_FICHA + "listar";
}

recargarFicha = () => {
    window.location.href = API_FICHA + 'editar/' + _matrizConfiguracionPresupuestal.MIRId;
}

validaHayCambios = () => {
    let listaMCPD = [];
    // Obtenemos todos los registros que hay en el DataGrid
    dxDataGridPorEjercer.getDataSource().store().load().done(response => response.map(mcpd => listaMCPD.push(mcpd)));

    // Obtener los datos para saber hay los datos o no hay para guardar
    const data = obtenerData(listaMCPD);
    //console.log(data);
    if (data.matrizConfiguracionPresupuestal == null && data.listaMatrizConfiguracionPresupuestalDetalle.length == 0) {
        regresarListado();
    } else {
        modalCerrar.modal('show');
    }
}

groupCellTemplate = (header, info) => {
    $('<div>', {
        id: 'titulo-componente-' + info.data.key,
        class: 'd-flex flex-row align-items-center',
        text: 'Componente: ' + info.data.key
    }).appendTo(header);

    if (info.data.items && info.data.items.length > 0) {
        if (info.data.items[0].EsEditado) {
            $('<i>', {
                id: 'icon-header-' + info.data.key,
                class: 'fas fa-exclamation-triangle ml-2'
            }).mouseover(() => {
                const mcp = dxFormMatrizConfiguracionPresupuestal.option('formData');
                $('#dxTooltip-' + info.data.key).dxTooltip('instance').option('contentTemplate', 'Es editado')
            }).appendTo($('#titulo-componente-' + info.data.key));
            // Crear un <div> para tooltip (devexpress)
            $('<div>', {
                id: 'dxTooltip-' + info.data.key
            }).appendTo($('#titulo-componente-' + info.data.key));
        
            $('#dxTooltip-' + info.data.key).dxTooltip({
                target: '#icon-header-' + info.data.key,
                showEvent: 'mouseenter',
                hideEvent: 'mouseleave',
                position: 'top',
                closeOnOutsideClick: false
            });
        }
    }
}

//////////

// Toast //
toast = (message, type) => {
    DevExpress.ui.notify({ message: message, width: "auto" }, type, 3500);
}
///////////