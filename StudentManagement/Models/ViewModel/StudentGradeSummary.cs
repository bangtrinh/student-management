// Thêm vào thư mục Models/ViewModels
namespace StudentManagement.Models.ViewModel
{
    public class StudentGradeSummary
    {
        public string StudentID { get; set; }
        public string FullName { get; set; }
        public string ClassID { get; set; }
        public string ClassName { get; set; }
        public string MajorID { get; set; }
        public string MajorName { get; set; }
        public float? AverageScore { get; set; }
        public string AcademicYear { get; set; }
        public string Semester { get; set; }
    }
}