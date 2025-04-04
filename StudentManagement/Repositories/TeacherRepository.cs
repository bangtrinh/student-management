using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace StudentManagement.Repositories
{
    public class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(StudentDbContext context) : base(context) { }

        public IEnumerable<Teacher> GetAll()
        {
            return _context.Teachers.Include(s => s.Department).ToList();
        }

        public IEnumerable<Teacher> GetTeachersByDepartment(string departmentId)
        {
            return _context.Teachers.Where(t => t.DepartmentID == departmentId).ToList();
        }

        public Teacher GetTeacherByEmail(string email)
        {
            return _context.Teachers.FirstOrDefault(t => t.Email == email);
        }

        public Teacher GetById(string id)
        {
            return _context.Teachers
                .Include(t => t.Department)
                .FirstOrDefault(t => t.TeacherID == id);
        }
    }
}