using Microsoft.AspNetCore.Identity;

namespace StudentManagement.Models.ViewModel
{
    public class ChatViewModel
    {
        public IdentityUser CurrentUser { get; set; }
        public IdentityUser ContactUser { get; set; }
        public List<Message> Messages { get; set; }
        public List<ContactWithUnread> ContactsWithUnread { get; set; }
        public string NewMessage { get; set; }
    }
}
