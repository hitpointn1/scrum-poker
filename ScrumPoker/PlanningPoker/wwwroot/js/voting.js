VotingHub();
function VotingHub() {
    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/ScrumRoom")
        .build();
    connection.on("UpdatePage", function () { window.location.reload();});
    connection.start();
};