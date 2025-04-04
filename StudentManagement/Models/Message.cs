using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace StudentManagement.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime SentTime { get; set; }
        public bool IsRead { get; set; }

        [ForeignKey("SenderId")]
        public virtual IdentityUser Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual IdentityUser Receiver { get; set; }
    }
}
