using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Collections.Concurrent;
using StudentManagement.Repositories;
using StudentManagement.Services;
using StudentManagement.Models;

namespace StudentManagement.Hubs 
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _onlineUsers = new();
        private readonly IMessageRepository _messageRepository;

        public ChatHub(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();

            if (!string.IsNullOrEmpty(userId))
            {
                _onlineUsers[userId] = Context.ConnectionId;
                await UpdateOnlineUsers();
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();

            if (!string.IsNullOrEmpty(userId) && _onlineUsers.TryRemove(userId, out _))
            {
                await UpdateOnlineUsers();
            }

            await base.OnDisconnectedAsync(exception);
        }

        private string GetUserId()
        {
            return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? Context.UserIdentifier
                   ?? string.Empty;
        }

        private async Task UpdateOnlineUsers()
        {
            await Clients.All.SendAsync("OnlineUsersUpdated", _onlineUsers.Keys.ToList());
        }

        // Thêm phương thức chat
        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = GetUserId();

            if (string.IsNullOrEmpty(senderId)) return;
            
            var msg = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message,
                SentTime = DateTime.UtcNow
            };
            
            //nếu ko phải do mình gửi thì lưu vào csdl
            if(senderId != receiverId)
            {
                _messageRepository.Add(msg);
            }

            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message, msg.SentTime);
        }
    }
}
