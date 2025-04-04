using StudentManagement.Models;
using StudentManagement.Models.ViewModel;

namespace StudentManagement.Repositories
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<IEnumerable<Message>> GetConversationAsync(string senderId, string receiverId);
        Task<Message> GetMessageByIdAsync(int id);
        Task AddMessageAsync(Message message);
        Task MarkMessagesAsReadAsync(string senderId, string receiverId);
        Task<int> GetUnreadMessagesCountAsync(string senderId, string receiverId);
    }
}
