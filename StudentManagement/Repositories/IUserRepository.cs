using Microsoft.AspNetCore.Identity;

namespace StudentManagement.Repositories
{
    public interface IUserRepository
    {
        Task<IdentityUser> GetUserByIdAsync(string userId);
        Task<IEnumerable<IdentityUser>> GetUsersByRoleAsync(string role);
        Task<IEnumerable<IdentityUser>> GetChatContactsAsync(string currentUserId);
    }
}
