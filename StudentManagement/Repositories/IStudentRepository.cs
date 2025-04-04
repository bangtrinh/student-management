using StudentManagement.Models;

namespace StudentManagement.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        IEnumerable<Student> GetStudentsByClass(string classId);
        Student GetStudentByEmail(string email);
        public IEnumerable<Student> GetStudentsByCourse(string courseId);
    }
}