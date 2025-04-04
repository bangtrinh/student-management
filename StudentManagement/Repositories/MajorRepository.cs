using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace StudentManagement.Repositories
{
    public class MajorRepository : BaseRepository<Major>, IMajorRepository
    {
        public MajorRepository(StudentDbContext context) : base(context) { }

        public IEnumerable<Major> GetMajorsByDepartment(string departmentId)
        {
            return _context.Majors.Where(m => m.DepartmentID == departmentId).ToList();
        }

        public Major GetById(string id)
        {
            return _context.Majors
                .Include(m => m.Department)
                .FirstOrDefault(m => m.MajorID == id);
        }

        public IEnumerable<Major> GetAll() {
            return _context.Majors.Include(m => m.Department).ToList();
        }
    }
}