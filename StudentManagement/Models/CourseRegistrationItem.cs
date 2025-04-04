namespace StudentManagement.Models
{
    public class CourseRegistrationItem
    {
        public string CourseID { get; set; }
        public string CourseName { get; set; }
        public string TeacherName { get; set; }

        public string Room { get; set; }
        public bool IsSelected { get; set; }
        
    }
}
