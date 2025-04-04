using StudentManagement.Models;

namespace StudentManagement.Repositories
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(StudentDbContext context) : base(context) { }
    }
}