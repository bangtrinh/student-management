using Microsoft.AspNetCore.Identity;
using StudentManagement.Models;

namespace StudentManagement.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly StudentDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserRepository(StudentDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IdentityUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IEnumerable<IdentityUser>> GetUsersByRoleAsync(string role)
        {
            return await _userManager.GetUsersInRoleAsync(role);
        }

        public async Task<IEnumerable<IdentityUser>> GetChatContactsAsync(string currentUserId)
        {
            var currentUser = await GetUserByIdAsync(currentUserId);
            if (currentUser == null) return new List<IdentityUser>();

            if (await _userManager.IsInRoleAsync(currentUser, "Teacher"))
            {
                return await GetUsersByRoleAsync("Student");
            }
            else
            {
                return await GetUsersByRoleAsync("Teacher");
            }
        }
    }
}
