using System.ComponentModel.DataAnnotations;

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
        public virtual ICollection<Major> Majors { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}