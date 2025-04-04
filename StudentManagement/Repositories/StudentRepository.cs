using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace StudentManagement.Repositories
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(StudentDbContext context) : base(context) { }

        public IEnumerable<Student> GetAll()
        {
            return _context.Students.Include(s => s.Class).ToList();
        }

        public IEnumerable<Student> GetStudentsByClass(string classId)
        {
            return _context.Students.Where(s => s.ClassID == classId).ToList();
        }

        public Student GetStudentByEmail(string email)
        {
            return _context.Students
                .Include(s => s.Class) // Load thông tin lớp học
                .ThenInclude(c => c.Major) // Load thông tin ngành học
                .FirstOrDefault(s => s.Email == email);
        }

        public IEnumerable<Student> GetStudentsByCourse(string courseId)
        {
            return _context.Students
                .Where(s => _context.Grades
                    .Where(g => g.CourseID == courseId)
                    .Select(g => g.StudentID) // Lấy danh sách StudentID trong bảng Grades
                    .Contains(s.StudentID))   // Chỉ lấy sinh viên có ID trong danh sách đó
                .ToList();
        }

        public Student GetById(string id)
        {
            return _context.Students
                .Include(s => s.Class) // Load thông tin lớp học
                .FirstOrDefault(s => s.StudentID == id);
        }
    }
}