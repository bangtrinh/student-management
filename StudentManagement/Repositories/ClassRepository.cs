using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace StudentManagement.Repositories
{
    public class ClassRepository : BaseRepository<Class>, IClassRepository
    {
        private readonly StudentDbContext _context;

        public IEnumerable<Class> GetAll()
        {
            return _context.Classes
                .Include(c => c.Students) // Load danh sách sinh viên
                .Include(c => c.Teacher)
                .Include(c => c.Major)
                .ToList();
        }

        public ClassRepository(StudentDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Class> GetClassesByTeacher(string teacherId)
        {
            return _context.Classes.Where(c => c.TeacherID == teacherId).ToList();
        }

        public Class GetById(string id)
        {
            return _context.Classes
                .Include(c => c.Students)
                .Include(c => c.Teacher)
                .Include(c => c.Major)// Load danh sách sinh viên
                .FirstOrDefault(c => c.ClassID == id);
        }
    }
}
