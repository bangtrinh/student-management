using StudentManagement.Models;

namespace StudentManagement.Repositories
{
    public interface IClassRepository : IRepository<Class>
    {
        IEnumerable<Class> GetClassesByTeacher(string teacherId);
    }
}