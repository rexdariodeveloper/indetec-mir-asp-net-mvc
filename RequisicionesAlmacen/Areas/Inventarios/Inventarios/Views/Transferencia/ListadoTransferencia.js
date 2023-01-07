﻿var API_FICHA = "/inventarios/inventarios/transferencia/";

$(document).ready(function () { });

var nuevo = function () {
    window.location.href = API_FICHA + "nuevo";
}

var ver = function (event) {
    window.location.href = API_FICHA + "ver/" + event.row.data.TransferenciaId;
}

var recargarFicha = function () {
    //Recargamos la ficha
    window.location.href = API_FICHA + "listar";
}

var toast = function (message, type) {
    DevExpress.ui.notify({ message: message, width: "auto" }, type, 5000);
}