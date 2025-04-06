using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Models
{
    public class OTP
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(64)]
        public string Code { get; set; }

        public DateTime ExpiryTime { get; set; } // OTP hết hạn sau 5 phút
    }
}
