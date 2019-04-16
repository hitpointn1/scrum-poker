VotingHub();
function VotingHub() {
    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/ScrumRoom")
        .build();
    connection.on("UpdatePage", function () { window.location.reload(); });
    connection.on("EntranceUser", function () {
        const RoomId = window.document.getElementById("RoomId").value;
        connection.invoke("OfflineUser", RoomId);
        const UserId = window.document.getElementById("UserId").value;
        connection.invoke("OnlineUser", UserId);
    });
    connection.on("ExitUser", function () {
        const RoomId = window.document.getElementById("RoomId").value;
        connection.invoke("OfflineUser", RoomId);
        const UserId = window.document.getElementById("UserId").value;
        connection.invoke("OnlineUser", UserId);
    });
    connection.start();
}