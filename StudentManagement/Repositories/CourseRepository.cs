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
                            .Include(c => c.Major) // Lấy thông tin chuyên ngành
                            .Include(c => c.Teacher) // Lấy thông tin giảng viên
                            .Include(c => c.Grades) // Lấy danh sách điểm
                            .ThenInclude(g => g.Student) // Lấy thông tin sinh viên
                            .FirstOrDefault(c => c.CourseID == courseId);
        }

        public Course GetById(string id)
        {
            return _context.Courses
                .Include(c => c.Major)
                .Include(c => c.Teacher)
                .FirstOrDefault(c => c.CourseID == id);
        }

        public IEnumerable<Course> GetAll()
        {
            return _context.Courses
                .Include(c => c.Major)
                .Include(c => c.Teacher)
                .ToList();
        }
        public IEnumerable<Course> GetOpenCourses()
        {
            return _context.Courses
                .Include(c => c.Teacher) // Kết nối với bảng Teacher
                .ToList();
        }

    }
}