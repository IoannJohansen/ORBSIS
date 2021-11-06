let connection = new signalR.HubConnectionBuilder().withUrl("/chathub").configureLogging(signalR.LogLevel.Information).build(),
    notifymessage = document.getElementsByClassName("message")[0],
    connectionId = "",
    sendButtons = document.getElementsByName("button");

const serverMethodName = 'ProcessUserChoice';

$(document).ready(function () {

    sendButtons.forEach(button => {
        button.onclick = function () {
            connection.invoke(serverMethodName, button.value);
        }
    });

    $('.toast').toast({
        'autohide': true,
        delay: 4000
    });

    connection.on("Notify", function (message, author) {
        notifymessage.textContent = `${message} by author: ${author}`;
        $('.toast').toast('show');
    });

    connection.start();

    $('#cls').click(() => {
        $('.toast').toast('hide');
    });

});