let connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

function StartSignal() {

    connection.on("Send", function (message, author, datetime) {
        alert(message);
        alert(author);
        alert(datetime);
    });

    connection.start();
}

StartSignal();