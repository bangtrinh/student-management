using StudentManagement.Models;

namespace StudentManagement.Repositories
{
    public interface IMajorRepository : IRepository<Major>
    {
        IEnumerable<Major> GetMajorsByDepartment(string departmentId);
    }
}