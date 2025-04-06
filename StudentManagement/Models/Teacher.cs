using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudentManagement.Models
{
    public class Teacher
    {
        [Key]
        [StringLength(10)]
        public string TeacherID { get; set; }

        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [StringLength(10)]
        public string DepartmentID { get; set; }

        [StringLength(100)]
        public string Specialization { get; set; }

        // Navigation properties
        [ValidateNever]
        public virtual Department Department { get; set; }
        [ValidateNever]
        public virtual ICollection<Class> Classes { get; set; }
    }
}