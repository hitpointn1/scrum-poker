ChatHub();
function ChatHub() {
    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();
    connection.on("ForwardToClients", (User, Context) => {
        const encoding = User + ": " + Context;
        const li = document.createElement("li");
        li.textContent = encoding;
        document.getElementById("messagesList").appendChild(li);
    });
    connection.start();

    document.getElementById("sendButton").addEventListener("click", event => {
        const UserId = document.getElementById("UserId").value;
        const RoomId = document.getElementById("RoomId").value;
        const Context = document.getElementById("messageInput").value;
        connection.invoke("SendMessage",RoomId, UserId, Context);
        event.preventDefault();
    });
}