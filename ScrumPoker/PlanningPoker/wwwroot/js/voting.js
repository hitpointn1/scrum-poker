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

    connection.on("OnlineUsers", function (playersOnline, adminName) {
        var table = document.getElementById('tableUserOnline');
        while (table.rows.length) {
            table.deleteRow(0);
        }
        var tbody = document.getElementById('tableUserOnline').children[0];
        for (var i = 0; i < playersOnline.length; i++) {
            //сделать элемент tr
            var tr = document.createElement('tr');
            //взять содержимое текущего элемента
            if (playersOnline[i] === adminName) {
                var data = playersOnline[i] + " (admin)";
            }
            else {
                var data = playersOnline[i];
            }
            //сделать элемент td
            var td = document.createElement('td');
            //положить содержимое в новый td
            td.innerHTML = data;
            //добавить td в tr
            tr.appendChild(td);

            td = document.createElement('td');
            td.innerHTML = "online";
            tr.appendChild(td);
            //добавить tr в tbody
            tbody.appendChild(tr);

        }
    });

    connection.start();

    let RoomId = window.document.getElementById("RoomId").value;
    window.setInterval(() => connection.invoke("OnlineUserList", RoomId), 10000);
}