using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudentManagement.Models
{
    public class Major
    {
        [Key]
        [StringLength(10)]
        public string MajorID { get; set; }

        [Required(ErrorMessage = "Tên ngành là bắt buộc")]
        [StringLength(100)]
        public string MajorName { get; set; }

        [StringLength(10)]
        public string DepartmentID { get; set; }

        public int Duration { get; set; }

        // Navigation properties
        [ValidateNever]
        public virtual Department Department { get; set; }
        [ValidateNever]
        public virtual ICollection<Class> Classes { get; set; }
    }
}