
var signout = function signout() {
    //Eliminamos la cookie
    document.cookie = "QKIE=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
    // Eliminamos Storage
    if (localStorage.getItem("listaMenuPrincipal"))
        localStorage.removeItem("listaMenuPrincipal");
    //Eliminamos el Item seleccionado del menu
    localStorage.removeItem("selectedItem");
    //Eliminamos el usuario logueado
    localStorage.removeItem("usuario");
    //Redireccionamos a la pagina de Login
    window.location.href = '/Login/Login';
}

var listaAlerta = [],
    contadorCiclo = 0;

returnPromises = () => {
    new Promise((resolve, reject) => {
        setInterval(() => {
            $.ajax({
                type: 'GET',
                url: '/home/getalert',
                success: function (response) {
                    // response es como salida de JSON desde en controlador
                    if (response.length > 0) {
                        $('#alerto-rojo').addClass('square-8 bg-danger pos-absolute t-15 r-5 rounded-circle');
                        if (JSON.stringify(listaAlerta) != JSON.stringify(response)) {
                            listaAlerta = response;
                            $('#lista-notificacion').empty();
                            listaAlerta.map(alerta => {
                                // Crear un div de alerta
                                $('#lista-notificacion').append($('<a>', {
                                    class: 'media-list-link',
                                }).append($('<div>', {
                                    class: 'media'
                                }).append($('<img>', {
                                    src: 'https://via.placeholder.com/500',
                                    alt: ''
                                })).append($('<div>', {
                                    class: 'media-body'
                                }).append($('<p>', {
                                    class: 'noti-text',
                                    text: alerta.Nombre
                                })).append($('<span>', {
                                    text: alerta.Fecha
                                })))));
                            });
                        }
                        
                    } else {
                        $('#alerto-rojo').removeClass('square-8 bg-danger pos-absolute t-15 r-5 rounded-circle');
                    }
                    
                    console.log('Ciclo: ' + contadorCiclo);
                    contadorCiclo++;
                    resolve(response);
                    //window.location.href = '/mir/mir/matrizconfiguracionpresupuestal/listar';
                },
                error: function (response, status, error) {
                    // Ocultamos Loader
                    console.log('error')
                    reject(response);
                }
            })
            //resolve('Sample Data');
        }, 3000); // pongas cuanto quieres para el tiempo si 30 segundos seria 30000
    });
}

ExecuteFunction = () => {
    //var getPromise = returnPromises();
    $(".logged-name").text(localStorage.getItem("usuario"));
}

ExecuteFunction();