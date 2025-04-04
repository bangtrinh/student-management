// wwwroot/js/chat.js
document.addEventListener('DOMContentLoaded', function () {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("ReceiveMessage", (senderId, message, sentTime) => {
        const isCurrentUser = senderId === document.getElementById('currentUserId').value;
        addMessageToChat(isCurrentUser, message, sentTime);
    });

    connection.on("UpdateOnlineUsers", (onlineUsers) => {
        updateOnlineStatus(onlineUsers);
    });

    connection.start()
        .then(() => {
            document.getElementById('sendButton').addEventListener('click', sendMessage);
            document.getElementById('messageInput').addEventListener('keypress', function (e) {
                if (e.key === 'Enter') sendMessage();
            });
        })
        .catch(console.error);

    function sendMessage() {
        const messageInput = document.getElementById('messageInput');
        const contactId = document.getElementById('contactId').value;
        const message = messageInput.value.trim();

        if (message && contactId) {
            connection.invoke("SendMessage", contactId, message)
                .catch(err => console.error(err));
            messageInput.value = '';
        }
    }

    function addMessageToChat(isCurrentUser, message, sentTime) {
        const chatMessages = document.getElementById('chatMessages');
        const messageDiv = document.createElement('div');
        messageDiv.className = `message ${isCurrentUser ? 'sent' : 'received'}`;

        messageDiv.innerHTML = `
            <div class="message-content">${message}</div>
            <div class="message-time">${sentTime}</div>
        `;

        chatMessages.appendChild(messageDiv);
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }

    function updateOnlineStatus(onlineUsers) {
        document.querySelectorAll('.contact-item').forEach(item => {
            const userId = item.dataset.userid;
            item.classList.toggle('online', onlineUsers.includes(userId));
        });
    }
});