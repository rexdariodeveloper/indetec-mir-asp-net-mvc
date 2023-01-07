//Modales
var modalConfirmaEliminar;

//Variables Globales
var rowEliminar;

var API_FICHA = "/compras/requisiciones/requisicionporsurtir/";

var ver = function (event) {
    window.location.href = API_FICHA + "editar/" + event.row.data.RequisicionMaterialId;
}