using StudentManagement.Models;

namespace StudentManagement.Repositories
{
    public interface IGradeRepository : IRepository<Grade>
    {
        IEnumerable<Grade> GetGradesByStudent(string studentId);
        IEnumerable<Grade> GetCoursesByStudentId(string studentId);
        public void UpdateRegistrations(string studentId, List<string> selectedCourseIds);
        Grade GetGradeByStudentAndCourse(string studentId, string courseId);
        public void Delete(int id);
        Grade GetById(int id);
    }
}