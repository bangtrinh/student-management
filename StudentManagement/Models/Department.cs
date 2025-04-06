using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudentManagement.Models
{
    public class Department
    {
        [Key]
        [StringLength(10)]
        public string DepartmentID { get; set; }

        [Required(ErrorMessage = "Tên khoa là bắt buộc")]
        [StringLength(100)]
        public string DepartmentName { get; set; }

        [StringLength(100)]
        public string HeadOfDepartment { get; set; }

        // Navigation properties
        [ValidateNever]
        public virtual ICollection<Major> Majors { get; set; }
        [ValidateNever]
        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}