using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Models
{
    public class Course
    {
        [Key]
        [StringLength(10)]
        public string CourseID { get; set; }

        [Required(ErrorMessage = "Tên môn học là bắt buộc")]
        [StringLength(100)]
        public string CourseName { get; set; }

        public int Credits { get; set; }

        [StringLength(50)]
        public string Room { get; set; }
        [StringLength(10)]
        public string? MajorID { get; set; } // Nullable

        [StringLength(10)]
        public string? TeacherID { get; set; } // Nullable

        public virtual Major Major { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
    }
}