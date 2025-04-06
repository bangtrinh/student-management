using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudentManagement.Models
{
    public class Class
    {
        [Key]
        [StringLength(10)]
        public string ClassID { get; set; }

        [Required(ErrorMessage = "Tên lớp là bắt buộc")]
        [StringLength(100)]
        public string ClassName { get; set; }

        [StringLength(10)]
        public string? MajorID { get; set; } // Nullable

        [StringLength(10)]
        public string? TeacherID { get; set; } // Nullable

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [ValidateNever]
        public virtual Major Major { get; set; }
        [ValidateNever]
        public virtual Teacher Teacher { get; set; }
        [ValidateNever]
        public virtual ICollection<Student> Students { get; set; }
    }
}