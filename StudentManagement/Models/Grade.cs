using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Models
{
    public class Grade
    {
        [Key]
        public int GradeID { get; set; }

        [StringLength(10)]
        public string StudentID { get; set; }

        [StringLength(10)]
        public string CourseID { get; set; }

        [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
        public float ?Score { get; set; }

        [StringLength(10)]
        public string Semester { get; set; }

        [StringLength(10)]
        public string AcademicYear { get; set; }

        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
    }
}