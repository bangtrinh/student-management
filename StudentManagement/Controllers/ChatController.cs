using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using StudentManagement.Models.ViewModel;
using StudentManagement.Repositories;

namespace StudentManagement.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public ChatController(
            IUserRepository userRepository,
            IMessageRepository messageRepository,
            UserManager<IdentityUser> userManager)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string contactId = null)
        {
            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userRepository.GetUserByIdAsync(currentUserId);


            if (currentUser == null)
            {
                return NotFound();
            }

            var contacts = await _userRepository.GetChatContactsAsync(currentUserId);

            var contactsWithUnreadCount = new List<ContactWithUnread>();
            foreach (var contact in contacts)
            {
                var unreadCount = await _messageRepository.GetUnreadMessagesCountAsync(
                    receiverId: currentUserId,
                    senderId: contact.Id
                );

                contactsWithUnreadCount.Add(new ContactWithUnread
                {
                    User = contact,
                    UnreadCount = unreadCount
                });
            }

            IdentityUser contactUser = null;
            IEnumerable<Message> messages = null;

            if (!string.IsNullOrEmpty(contactId))
            {
                contactUser = await _userRepository.GetUserByIdAsync(contactId);

                if (contactUser != null)
                {
                    messages = await _messageRepository.GetConversationAsync(currentUserId, contactId);
                    await _messageRepository.MarkMessagesAsReadAsync(contactId, currentUserId);

                    //tìm contact trong danh sách contactsWithUnreadCount
                    var contactWithUnread = contactsWithUnreadCount
                        .FirstOrDefault(c => c.User.Id == contactId);
                    if (contactWithUnread != null)
                    {
                        //cập nhật số lượng tin nhắn chưa đọc
                        contactWithUnread.UnreadCount = 0;
                    }

                }
            }

            var model = new ChatViewModel
            {
                CurrentUser = currentUser,
                ContactUser = contactUser,
                Messages = messages?.ToList(),
                ContactsWithUnread = contactsWithUnreadCount
            };

            return View(model);
        }
    }
}