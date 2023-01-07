customizeTooltip = (pointsInfo) => {
    return { text: function2DecimalsRound(pointsInfo.originalValue) + "%" };
}

redirigirEditar = (event) => {
    dxLoaderPanel.show();
    $.ajax({
        type: "POST",
        url: "/mir/mir/matrizpresupuestodevengado/existeactividad",
        data: { mirId: event.row.data.MIRId },
        success: function (response) {
            window.location.href = '/mir/mir/matrizpresupuestodevengado/editar/' + event.row.data.MIRId;
        },
        error: function (response, status, error) {
            // Ocultamos Loader
            dxLoaderPanel.hide();
            // Mostramos mensaje de error
            toast(response.responseText, "error");
        }
    });
}

// Toast //
toast = (message, type) => {
    DevExpress.ui.notify({ message: message, width: "auto" }, type, 3500);
}
///////////