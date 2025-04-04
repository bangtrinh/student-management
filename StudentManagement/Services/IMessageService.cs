using StudentManagement.Models;
using StudentManagement.Repositories;

namespace StudentManagement.Services
{
    public interface IMessageService
    {
        //thêm tin nhắn vào database
        Task SaveMessageAsync(string senderId, string receiverId, string message);
    }

    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task SaveMessageAsync(string senderId, string receiverId, string message)
        {
            var newMessage = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message,
                SentTime = DateTime.UtcNow,
                IsRead = false
            };
            await _messageRepository.AddMessageAsync(newMessage);
        }
    }
}
