using Microsoft.AspNetCore.Identity;

namespace StudentManagement.Models.ViewModel
{
    public class ContactWithUnread
    {
        public IdentityUser User { get; set; }
        public int UnreadCount { get; set; }
    }
}
