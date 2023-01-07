// VARIABLES GLOBALES //
var contadorRegistrosNuevos,
    // Modal
    modalCerrar,
    modalConfirmaGuardar,
    // Form
    dxFormMatrizConfiguracionPresupuestal,
    // DataGrid
    dxDataGridDevengado,
    // Listas
    listaMatrizConfiguracionPresupuestalDetalle = [],
    // API
    API_FICHA = '/mir/mir/matrizpresupuestodevengado/';

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
    modalConfirmaGuardar = $('#modalConfirmaGuardar');
    // Form
    dxFormMatrizConfiguracionPresupuestal = $('#dxFormMatrizConfiguracionPresupuestal').dxForm('instance');
    // DataGrid
    //dxDataGridDevengado = $('#dxDataGridDevengado').dxDataGrid('instance');
    // Listas
    //listMatrizConfiguracionPresupuestalDetalle = _listMatrizIndicadorResultadoIndicador ? $.extend(true, [], _listMatrizIndicadorResultadoIndicador) : [];

    dxFormMatrizConfiguracionPresupuestal.updateData("Codigo", _consultaMatrizIndicadorResultado.Codigo);
    dxFormMatrizConfiguracionPresupuestal.updateData("ProgramaPresupuestario", _consultaMatrizIndicadorResultado.ProgramaPresupuestario);

    // Carga listados para MCP y MCPD
    //cargaListados();
}

onInitializedDataGridDevengado = (event) => {
    dxDataGridDevengado = event.component;
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
        MontoMaximo: null,
        MontoIndicador: null,
        Respaldo: null
    }, devengadoModel = {
        Enero: {
            CadaMes: null,
            UltimoMes: null
        },
        Febrero: {
            CadaMes: null,
            UltimoMes: null
        },
        Marzo: {
            CadaMes: null,
            UltimoMes: null
        },
        Abril: {
            CadaMes: null,
            UltimoMes: null
        },
        Mayo: {
            CadaMes: null,
            UltimoMes: null
        },
        Junio: {
            CadaMes: null,
            UltimoMes: null
        },
        Julio: {
            CadaMes: null,
            UltimoMes: null
        },
        Agosto: {
            CadaMes: null,
            UltimoMes: null
        },
        Septiembre: {
            CadaMes: null,
            UltimoMes: null
        },
        Octubre: {
            CadaMes: null,
            UltimoMes: null
        },
        Noviembre: {
            CadaMes: null,
            UltimoMes: null
        },
        Diciembre: {
            CadaMes: null,
            UltimoMes: null
        }
        }, listaMCPD = [];
    const miriGroupBy = functionGroupBy(_listaMatrizIndicadorResultadoIndicador, 'MIRIndicadorComponenteId');
    let componenteCount = 1, componenteTitulo = '', actividadCount = null;
    console.log(_listaDevengado);
    miriGroupBy.map(mirIndicadorComponenteId => {
        var miriComponente = _listaMIRIComponente.find(miriC => miriC.MIRIndicadorId == mirIndicadorComponenteId);
        // Titulo
        componenteTitulo = 'C' + componenteCount + ' - ' + miriComponente.NombreIndicador;
        let totalAnual = 0, totalPorcentaje = 0, contadorIndicadores = 0;
        actividadCount = 1;
        const listaMatrizIndicadorResultadoIndicador = $.extend(true, [], _listaMatrizIndicadorResultadoIndicador.filter(miri => miri.MIRIndicadorComponenteId == mirIndicadorComponenteId));
        // Asignamos el contador cuantos hay indicadoes
        contadorIndicadores = listaMatrizIndicadorResultadoIndicador.length;
        listaMatrizIndicadorResultadoIndicador.map(miri => {
            let _mcpdModel = null;
            // Hacer suma por anual y porcentaje
            // Tipo Componente -> Relacion Actividad
            if (miri.TipoComponenteId == ControlMaestroMapeo.TipoComponente.RELACION_ACTIVIDAD) {
                totalAnual += (miri.MontoProyecto * (miri.PorcentajeProyecto / 100));
                totalPorcentaje += miri.PorcentajeProyecto;
            }
            // Tipo Componente -> Relacion Componente
            if (miri.TipoComponenteId == ControlMaestroMapeo.TipoComponente.RELACION_COMPONENTE) {
                //totalAnual += (miri.MontoActividad * (miri.PorcentajeActividad / 100));
                totalAnual += miri.MontoActividad;
                totalPorcentaje += miri.PorcentajeActividad;
            }
            // Devengado
            mcpd = $.extend(true, {}, _listaMatrizConfiguracionPresupuestalDetalle.find(_mcpd => _mcpd.MIRIndicadorId == miri.MIRIndicadorId && _mcpd.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.DEVENGADO));
            if (mcpd.ConfiguracionPresupuestoDetalleId) {
                // Creamos los objetos nuevos
                let _mcpdModel = $.extend(true, {}, mcpdModel);
                _mcpdModel.ConfiguracionPresupuestoDetalleId = mcpd.ConfiguracionPresupuestoDetalleId;
                _mcpdModel.ConfiguracionPresupuestoId = mcpd.ConfiguracionPresupuestoId;
                _mcpdModel.ClasificadorId = mcpd.ClasificadorId;
                _mcpdModel.MIRIndicadorId = mcpd.MIRIndicadorId;
                _mcpdModel.Enero = mcpd.Enero;
                _mcpdModel.Febrero = mcpd.Febrero;
                _mcpdModel.Marzo = mcpd.Marzo;
                _mcpdModel.Abril = mcpd.Abril;
                _mcpdModel.Mayo = mcpd.Mayo;
                _mcpdModel.Junio = mcpd.Junio;
                _mcpdModel.Julio = mcpd.Julio;
                _mcpdModel.Agosto = mcpd.Agosto;
                _mcpdModel.Septiembre = mcpd.Septiembre;
                _mcpdModel.Octubre = mcpd.Octubre;
                _mcpdModel.Noviembre = mcpd.Noviembre;
                _mcpdModel.Diciembre = mcpd.Diciembre;
                _mcpdModel.Anual = mcpd.Anual;
                _mcpdModel.Porcentaje = mcpd.Porcentaje;
                _mcpdModel.CabeceraMeses.Porcentaje = mcpd.Porcentaje;
                // Repaldo
                _mcpdModel.Respaldo = $.extend(true, {}, _mcpdModel);
                // Titulo
                _mcpdModel.Componente = componenteTitulo;
                _mcpdModel.Actividad = 'Actividad ' + actividadCount;

                _mcpdModel.EsProyecto = miri.TipoComponenteId == ControlMaestroMapeo.TipoComponente.RELACION_COMPONENTE ? true : false;
                // Tipo Componente -> Relacion Componente
                if (_mcpdModel.EsProyecto) {
                    //var devengado = $.extend(true, {}, _listaDevengado.find(d => d.MIRIndicadorId == miri.MIRIndicadorComponenteId));
                    //for (let x = 1; x <= 12; x++) {
                    //    var mes = funcionObtenerMes(x);
                    //    _mcpdModel.CabeceraMeses[mes] = parseFloat(devengado[mes].toFixed(2));
                    //}
                    //_mcpdModel.CabeceraMeses.Anual = parseFloat(devengado.Anual.toFixed(2));

                    var __listaDevengado = $.extend(true, [], _listaDevengado.filter(d => listaMatrizIndicadorResultadoIndicador.find(__miri => __miri.MIRIndicadorId == d.MIRIndicadorId))),
                        _totalAnual = 0;

                    for (let x = 1; x <= 12; x++) {
                        var mes = funcionObtenerMes(x),
                            totalCabeceraMes = 0;
                        // Cabecera
                        __listaDevengado.map(d => {
                            totalCabeceraMes += d[mes];
                        });
                        totalCabeceraMes = parseFloat(totalCabeceraMes.toFixed(2));
                        _mcpdModel.CabeceraMeses[mes] = totalCabeceraMes;

                        // Hay movimiento el mes y tambien si el mes encuentra abierta
                        if (_listaControlMaestroControlPeriodo.find(cp => cp.Codigo == funcionObtenerCodigoMes(mes)).EstatusPeriodoId == ControlMaestroMapeo.MIEstatusPeriodo.ABIERTO) {
                            const movimiento = parseFloat(__listaDevengado.find(d => d.MIRIndicadorId == miri.MIRIndicadorId)[mes].toFixed(2));
                            if (mcpd[mes] == 0 && mcpd[mes] != movimiento) {
                                _mcpdModel[mes] = movimiento;
                                _mcpdModel.Anual += movimiento;

                            }
                        }
                    }
                    // Cabecera
                    __listaDevengado.map(d => _totalAnual += parseFloat(d.Anual.toFixed(2)));
                    _totalAnual = parseFloat(_totalAnual.toFixed(2));
                    _mcpdModel.CabeceraMeses.Anual = _totalAnual;

                } else {
                    var __listaDevengado = $.extend(true, [], _listaDevengado.filter(d => listaMatrizIndicadorResultadoIndicador.find(__miri => __miri.MIRIndicadorId == d.MIRIndicadorId))),
                        _totalAnual = 0;
                    for (let x = 1; x <= 12; x++) {
                        var mes = funcionObtenerMes(x),
                            totalCabeceraMes = 0;
                        // Cabecera
                        __listaDevengado.map(d => {
                            totalCabeceraMes += d[mes];
                        });
                        totalCabeceraMes = parseFloat(totalCabeceraMes.toFixed(2));
                        _mcpdModel.CabeceraMeses[mes] = totalCabeceraMes;
                        // Monto
                        _mcpdModel[mes] = parseFloat(__listaDevengado.find(d => d.MIRIndicadorId == miri.MIRIndicadorId)[mes].toFixed(2));
                    }
                    // Cabecera
                    __listaDevengado.map(d => _totalAnual += parseFloat(d.Anual.toFixed(2)));
                    _totalAnual = parseFloat(_totalAnual.toFixed(2));
                    _mcpdModel.CabeceraMeses.Anual = _totalAnual;
                    // Monto
                    _mcpdModel.Anual = parseFloat(__listaDevengado.find(d => d.MIRIndicadorId == miri.MIRIndicadorId).Anual.toFixed(2));
                    //_mcpdModel.Anual = miri.MontoProyecto;
                    _mcpdModel.MontoMaximo = miri.MontoProyecto;
                    _mcpdModel.MontoIndicador = miri.MontoProyecto;
                }

                listaMCPD.push(_mcpdModel);
            } else {
                // Creamos los objetos nuevos
                let _mcpdModel = $.extend(true, {}, mcpdModel);
                _mcpdModel.ConfiguracionPresupuestoDetalleId = contadorRegistrosNuevos;
                _mcpdModel.ConfiguracionPresupuestoId = _matrizConfiguracionPresupuestal.ConfiguracionPresupuestoId;
                _mcpdModel.ClasificadorId = ControlMaestroMapeo.TipoPresupuesto.DEVENGADO;
                _mcpdModel.MIRIndicadorId = miri.MIRIndicadorId;
                // Titulo
                _mcpdModel.Componente = componenteTitulo;
                _mcpdModel.Actividad = 'Actividad ' + actividadCount;

                _mcpdModel.EsProyecto = miri.TipoComponenteId == ControlMaestroMapeo.TipoComponente.RELACION_COMPONENTE ? true : false;
                // Tipo Componente -> Relacion Componente
                if (_mcpdModel.EsProyecto) {
                    _mcpdModel.Porcentaje = miri.PorcentajeActividad;
                    //_mcpdModel.CabeceraMeses.Porcentaje = miri.PorcentajeActividad;

                    var __listaDevengado = $.extend(true, [], _listaDevengado.filter(d => listaMatrizIndicadorResultadoIndicador.find(__miri => __miri.MIRIndicadorId == d.MIRIndicadorId))),
                        _totalAnual = 0;

                    for (let x = 1; x <= 12; x++) {
                        var mes = funcionObtenerMes(x),
                            totalCabeceraMes = 0;
                        // Cabecera
                        __listaDevengado.map(d => {
                            totalCabeceraMes += d[mes];
                        });
                        totalCabeceraMes = parseFloat(totalCabeceraMes.toFixed(2));
                        _mcpdModel.CabeceraMeses[mes] = totalCabeceraMes;
                        // Monto 
                        _mcpdModel[mes] = parseFloat(__listaDevengado.find(d => d.MIRIndicadorId == miri.MIRIndicadorId)[mes].toFixed(2));
                    }
                    // Cabecera
                    __listaDevengado.map(d => _totalAnual += parseFloat(d.Anual.toFixed(2)));
                    _totalAnual = parseFloat(_totalAnual.toFixed(2));
                    _mcpdModel.CabeceraMeses.Anual = _totalAnual;
                    // Monto
                    _mcpdModel.Anual = parseFloat(__listaDevengado.find(d => d.MIRIndicadorId == miri.MIRIndicadorId).Anual.toFixed(2));
                    //_mcpdModel.Anual = miri.MontoProyecto;
                    _mcpdModel.MontoMaximo = miri.MontoActividad;
                    _mcpdModel.MontoIndicador = miri.MontoActividad;
                } else {
                    _mcpdModel.Porcentaje = miri.PorcentajeProyecto;
                    _mcpdModel.CabeceraMeses.Porcentaje = miri.PorcentajeProyecto;

                    var __listaDevengado = $.extend(true, [], _listaDevengado.filter(d => listaMatrizIndicadorResultadoIndicador.find(__miri => __miri.MIRIndicadorId == d.MIRIndicadorId))),
                        _totalAnual = 0;
                    for (let x = 1; x <= 12; x++) {
                        var mes = funcionObtenerMes(x),
                            totalCabeceraMes = 0;
                        // Cabecera
                        __listaDevengado.map(d => {
                            totalCabeceraMes += d[mes];
                        });
                        totalCabeceraMes = parseFloat(totalCabeceraMes.toFixed(2));
                        _mcpdModel.CabeceraMeses[mes] = totalCabeceraMes;
                        // Monto
                        _mcpdModel[mes] = parseFloat(__listaDevengado.find(d => d.MIRIndicadorId == miri.MIRIndicadorId)[mes].toFixed(2));
                    }
                    // Cabecera
                    __listaDevengado.map(d => _totalAnual += d.Anual);
                    _totalAnual = parseFloat(_totalAnual.toFixed(2));
                    _mcpdModel.CabeceraMeses.Anual = _totalAnual;
                    // Monto
                    _mcpdModel.Anual = parseFloat(__listaDevengado.find(d => d.MIRIndicadorId == miri.MIRIndicadorId).Anual.toFixed(2));
                    //_mcpdModel.Anual = miri.MontoProyecto;
                    _mcpdModel.MontoMaximo = miri.MontoProyecto;
                    _mcpdModel.MontoIndicador = miri.MontoProyecto;
                }
                listaMCPD.push(_mcpdModel);
                // Contador
                contadorRegistrosNuevos--;
            }
            // Count
            actividadCount++;

        });
        // Count
        componenteCount++;
        // Devengado
        var listaDevengado = listaMCPD.filter(mcpd => mcpd.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.DEVENGADO && mcpd.Componente == componenteTitulo);
        listaDevengado.map(devengado => devengado.CabeceraMeses.Porcentaje = totalPorcentaje);
        if (listaDevengado[0].ConfiguracionPresupuestoDetalleId < 0) {
            if (contadorIndicadores > 0) {
                if (listaDevengado[0].EsProyecto) {
                    for (let x = 1; x <= 12; x++) {
                        var mes = funcionObtenerMes(x);
                        let cabeceraTotalMeses = listaDevengado[0].CabeceraMeses[mes], filaTotalMeses = 0, filaUltimoMes = 0;
                        listaDevengado.map(d => {
                            filaTotalMeses += d[mes];
                        });

                        if (filaTotalMeses != cabeceraTotalMeses) {
                            var ultimoDevengado = listaDevengado[listaDevengado.length - 1];
                            if (filaTotalMeses > cabeceraTotalMeses) {
                                ultimoDevengado[mes] = parseFloat(ultimoDevengado[mes] - function2DecimalsRound(filaTotalMeses - cabeceraTotalMeses));
                            } else {
                                ultimoDevengado[mes] = parseFloat(ultimoDevengado[mes] + function2DecimalsRound(cabeceraTotalMeses - filaTotalMeses));
                            }
                        }
                    }
                }
            }
        }
    });
    // Establecer los datos a DataGrid
    // Devengado
    const listaDevengado = listaMCPD.filter(mcpd => mcpd.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.DEVENGADO);
    var dataSourceDevengado = new DevExpress.data.DataSource({
        store: {
            type: 'array',
            key: 'ConfiguracionPresupuestoDetalleId',
            data: listaDevengado
        },
        filter: [['ClasificadorId', '=', ControlMaestroMapeo.TipoPresupuesto.DEVENGADO]]
    });
    dxDataGridDevengado.option('dataSource', dataSourceDevengado);
    // Actualizamos el campo Presupuesto
    actualizaPresupuesto();
}


actualizaPresupuesto = () => {
    var items = [], totalPresupuesto = 0;
    dxDataGridDevengado.getDataSource().store().load({ filter: ['ClasificadorId', '=', ControlMaestroMapeo.TipoPresupuesto.DEVENGADO] }).done(response => items = response).fail(() => { toast("Error los datos, intentalo mas tarde.", "error"); return; });
    items.map(_item => {
        for (let x = 1; x <= 12; x++) {
            var mes = funcionObtenerMes(x);
            if (_item[mes] != null)
                totalPresupuesto += _item[mes];
        }
    });
    dxFormMatrizConfiguracionPresupuestal.getEditor('PresupuestoDevengado').option('value', totalPresupuesto);
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
                    event.totalValue = parseFloat(event.value.CabeceraMeses[event.name]);
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
        // Deshablitar la celda solo para tipo prespuesto DEVENGADO y no el proyecto
        if (event.row.data.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.DEVENGADO && !event.row.data.EsProyecto)
            event.editorOptions.disabled = true;

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
    // Devengado
    if (event.row.data.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.DEVENGADO) {
        // Asignamos el monto de la fila que estoy editando
        item = items.find(_item => _item.ConfiguracionPresupuestoDetalleId == itemId);
        item[event.dataField] = monto;
        // Actulizamos Anual
        item.Anual = 0;
        for (let x = 1; x <= 12; x++) {
            var mes = funcionObtenerMes(x);
            if (item[mes] != null) {
                item.Anual += item[mes];
            }
        }

        dataSource.store().update(item.ConfiguracionPresupuestoDetalleId, item).done(() => { dataSource.reload(); }).fail(() => { toast("Error los datos, intentalo mas tarde.", "error"); return; });
        // Actualizamos el presupuesto
        actualizaPresupuesto();
    }
}

onCellPrepared = (event) => {
    if (event.rowType == 'data' && (event.column.dataField == 'Anual' || event.column.dataField == 'Porcentaje') && event.cellElement || event.rowType == 'groupFooter' && event.cellElement) {
        //console.log('aaaa');
        // Establecer color a celda
        event.cellElement.addClass('pixvs-datagrid-cell');
        if (event.columnIndex != 0 || event.columnIndex != 1) {
            // Establecer border a celda
            event.cellElement.addClass('pixvs-datagrid-cell-border');
        }
    }
    // Pon color las celdas cuando no estan guardandas
    if (event.rowType == 'data' && (event.column.dataField == 'Enero' || event.column.dataField == 'Febrero' || event.column.dataField == 'Marzo' || event.column.dataField == 'Abril' || event.column.dataField == 'Mayo' || event.column.dataField == 'Junio' || event.column.dataField == 'Julio' || event.column.dataField == 'Agosto' || event.column.dataField == 'Septiembre' || event.column.dataField == 'Octubre' || event.column.dataField == 'Noviembre' || event.column.dataField == 'Diciembre') && event.cellElement) {
        if (event.data.ConfiguracionPresupuestoDetalleId > 0) {
            if (event.data[event.column.dataField] == event.data.Respaldo[event.column.dataField]) {
                event.cellElement.removeClass('pixvs-datagrid-cell-save');
            } else {
                setTimeout(() => {
                    event.cellElement.addClass('pixvs-datagrid-cell-save');
                }, 0);
                
            }
        } else {
            event.cellElement.addClass('pixvs-datagrid-cell-save');
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

headerCellTemplate = (header, info) => {
    //var mcp = dxFormMatrizConfiguracionPresupuestal.option('formData');
    if (_matrizConfiguracionPresupuestal.PresupuestoPorEjercer > _matrizConfiguracionPresupuestal.PresupuestoDevengado) {
        // Crear un <i> para icon
        $('<i>', {
            id: 'icon-header',
            class: 'fas fa-exclamation-triangle'
        }).mouseover(() => {
            const mcp = dxFormMatrizConfiguracionPresupuestal.option('formData');
            $('#dxTooltip').dxTooltip('instance').option('contentTemplate', 'Necesita llenan para que el completo del proyecto al 100 %, ahora esta ' + function2DecimalsRound(functionPercentageV2IsV1(mcp.PresupuestoPorEjercer, mcp.PresupuestoDevengado)) + ' %')
        }).appendTo(header);
        // Crear un <div> para tooltip (devexpress)
        $('<div>', {
            id: 'dxTooltip'
        }).appendTo(header);
        //$('<div>', {
        //    id: 'tooltip1'
        //}).html('Necesita llenan para que el completo del proyecto al 100%, ahora esta ' + functionPercentageV2IsV1(presupuestoPorEjercer, presupuestoDevengado) + '%').appendTo(header);

        $('#dxTooltip').dxTooltip({
            target: '#icon-header',
            showEvent: 'mouseenter',
            hideEvent: 'mouseleave',
            position: 'top',
            closeOnOutsideClick: false
        });
    }
}

test1 = (header, info) => {
    //var mcp = dxFormMatrizConfiguracionPresupuestal.option('formData');
    header.append($('<div>', {
        id: 'cabecera',
        class: 'd-flex justify-content-center pixvs-cursor-pointer'
    }).click(() => {
        if ($('#icon-header').hasClass('fa-lock-open')) {
            $('#icon-header').removeClass('fa-lock-open').addClass('fa-lock');
            info.component.columnOption(info.columnIndex).editorOptions.disabled = true;
        } else {
            $('#icon-header').removeClass('fa-lock').addClass('fa-lock-open');
            let items = [];
            dxDataGridPorEjercer.getDataSource().store().load({ filter: ['ClasificadorId', '=', ControlMaestroMapeo.TipoPresupuesto.POR_EJERCER] }).done(response => items = response).fail(() => { toast("Error los datos, intentalo mas tarde.", "error"); return; });

            info.component.columnOption(info.columnIndex).editorOptions.disabled = false;
            //items.map(item => {
            //    dxDataGridPorEjercer.
            //});
        }
    }).append($('<i>', {
        id: 'icon-header',
        class: 'fas fa-lock-open mr-1'
    })).append($('<p>', {
        class: 'm-0',
        text: info.column.dataField
    })));
}

validaPeriodo = () => {
    // Crear el objeto array
    let esPeriodoCerrado = false;

    bucle:
    for (let x = 1; x <= 12; x++) {
        var mes = funcionObtenerMes(x);
        if (_listaControlMaestroControlPeriodo.find(cp => cp.Codigo == funcionObtenerCodigoMes(mes)).EstatusPeriodoId != ControlMaestroMapeo.MIEstatusPeriodo.ABIERTO) {
            esPeriodoCerrado = true;
            break bucle;
        }
    }

    if (esPeriodoCerrado) {
        //modalConfirmaGuardar.attr("listaMCPD", JSON.stringify(listaMCPD));
        modalConfirmaGuardar.modal('show');
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
    dxDataGridDevengado.getDataSource().store().load().done(response => response.map(mcpd => listaMCPD.push(mcpd)));
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
        // Tipo de Presupuesto -> Devengado
        if (mcpd.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.DEVENGADO
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
                : false) {
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
        listaDevengado = listaMCPD.filter(mcpd => mcpd.ClasificadorId == ControlMaestroMapeo.TipoPresupuesto.DEVENGADO);
        listaComponenteGroupBy = functionGroupBy(listaDevengado, 'Componente');

    if (_esValida) {

        bucle:
        for (let y = 0; y < listaComponenteGroupBy.length; y++) {
            let listaComponente = listaDevengado.filter(mcpd => mcpd.Componente == listaComponenteGroupBy[y]),
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
        }
    }
    return _esValida;
}

esValidaDevengado = (listaMCPD) => {
    let valida = true,
        contadorActividad = listaMCPD.length;
    bucle:
    for (let i = 0; i < contadorActividad; i++) {
        if ((listaMCPD[i].Enero == null || listaMCPD[i].Enero != null && listaMCPD[i].Enero >= 0) || (listaMCPD[i].Febrero == null || listaMCPD[i].Febrero != null && listaMCPD[i].Febrero >= 0) || (listaMCPD[i].Marzo == null || listaMCPD[i].Marzo != null && listaMCPD[i].Marzo >= 0) || (listaMCPD[i].Abril == null || listaMCPD[i].Abril != null && listaMCPD[i].Abril >= 0) || (listaMCPD[i].Mayo == null || listaMCPD[i].Mayo != null && listaMCPD[i].Mayo >= 0) || (listaMCPD[i].Junio == null || listaMCPD[i].Junio != null && listaMCPD[i].Junio >= 0) || (listaMCPD[i].Julio == null || listaMCPD[i].Julio != null && listaMCPD[i].Julio >= 0) || (listaMCPD[i].Agosto == null || listaMCPD[i].Agosto != null && listaMCPD[i].Agosto >= 0) || (listaMCPD[i].Septiembre == null || listaMCPD[i].Septiembre != null && listaMCPD[i].Septiembre >= 0) || (listaMCPD[i].Octubre == null || listaMCPD[i].Octubre != null && listaMCPD[i].Octubre >= 0) || (listaMCPD[i].Noviembre == null || listaMCPD[i].Noviembre != null && listaMCPD[i].Noviembre >= 0) || (listaMCPD[i].Diciembre == null || listaMCPD[i].Diciembre != null && listaMCPD[i].Diciembre >= 0)) {

            // Meses
            for (let mes = 1; mes <= 12; mes++) {
                const campoMes = funcionObtenerMes(mes);
                if (listaMCPD[i][campoMes] == null || listaMCPD[i][campoMes] == 0 || (i + 1) == contadorActividad) {
                    // Valida: El mes no debe ser NULL
                    if (listaMCPD[i][campoMes] == null) {
                        toast('Necesita llenar la actividad: "' + listaMCPD[i].Actividad + '" de Componente: "' + listaMCPD[i].Componente + '".', 'error');
                        valida = false;
                        // Asignamos a la celda como enfocar
                        //let mcpd = {};
                        //dxDataGridPorEjercer.getDataSource().store().load({ filter: [['Componente', '=', listaMCPD[i].Componente], 'and', ['Actividad', '=', listaMCPD[i].Actividad]] }).done(response => mcpd = response[0]).fail(() => { toast("Error los datos, intentalo mas tarde.", "error"); });
                        //const rowIndex = dxDataGridPorEjercer.getRowIndexByKey(mcpd.ConfiguracionPresupuestoDetalleId);
                        //dxDataGridPorEjercer.editCell(rowIndex, campoMes);
                        break bucle;
                    }

                    // Valida: El campo de Diferencia no debe haber diferencias que no permite guardar
                    if ((i + 1) == contadorActividad) {
                        let totalDiferencia = 0;
                        listaMCPD.map(mcpd => { totalDiferencia += mcpd[campoMes]; });
                        if (totalDiferencia < listaMCPD[i].CabeceraMeses[campoMes]) {
                            toast('El campo de Diferencia debe estar $0 en el mes ' + campoMes + ' en la actividad: "' + listaMCPD[i].Actividad + '" de Componente: "' + listaMCPD[i].Componente + '".', 'error');
                            valida = false;
                            break bucle;
                        }
                    }
                }
            }
        }
    }
    return valida;
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
    dxDataGridDevengado.getDataSource().store().load().done(response => response.map(mcpd => listaMCPD.push(mcpd)));

    // Obtener los datos para saber hay los datos o no hay para guardar
    const data = obtenerData(listaMCPD);
    //console.log(data);
    if (data.matrizConfiguracionPresupuestal == null && data.listaMatrizConfiguracionPresupuestalDetalle.length == 0) {
        regresarListado();
    } else {
        modalCerrar.modal('show');
    }
}

//////////

// Toast //
toast = (message, type) => {
    DevExpress.ui.notify({ message: message, width: "auto" }, type, 3500);
}
///////////