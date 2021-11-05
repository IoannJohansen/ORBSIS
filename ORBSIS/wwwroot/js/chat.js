let connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build(),
    notifymessage = document.getElementsByClassName("message")[0];
    
$(document).ready(function () {
    $('.toast').toast({
        'autohide': true,
        delay: 4000
    });

    connection.on("Send", function (message, author) {
        notifymessage.textContent = `${message} by author: ${author}`;
        $('.toast').toast('show');
    });

    connection.start();

    $('#cls').click(() => {
        $('.toast').toast('hide');
    });

});