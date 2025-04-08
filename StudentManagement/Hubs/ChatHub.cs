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
        private readonly IMessageRepository _messageRepository;

        public ChatHub(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        private string GetUserId()
        {
            return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? Context.UserIdentifier
                   ?? string.Empty;
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
