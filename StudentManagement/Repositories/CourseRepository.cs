using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace StudentManagement.Repositories
{
    public class CourseRepository : BaseRepository<Course>, ICourseRepository
    {
        public CourseRepository(StudentDbContext context) : base(context) { }

        public IEnumerable<Course> GetCoursesByMajor(string majorId)
        {
            return _context.Courses
                .Include(c => c.Major)
                .Include(c => c.Teacher)
                .Where(c => c.MajorID == majorId).ToList();
        }

        public IEnumerable<Course> GetCoursesByTeacher(string teacherId)
        {
            return _context.Courses.Where(c => c.TeacherID == teacherId).ToList();
        }

        public Course GetCourseDetails(string courseId)
        {
            return _context.Courses
                            .Include(c => c.Grades) // Lấy danh sách điểm
                            .ThenInclude(g => g.Student) // Lấy thông tin sinh viên
                            .FirstOrDefault(c => c.CourseID == courseId);
        }
    }
}