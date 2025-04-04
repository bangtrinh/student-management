using StudentManagement.Models;

namespace StudentManagement.Repositories
{
    public interface ITeacherRepository : IRepository<Teacher>
    {
        IEnumerable<Teacher> GetTeachersByDepartment(string departmentId);
        Teacher GetTeacherByEmail(string email);
    }
}