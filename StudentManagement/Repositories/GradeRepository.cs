using Microsoft.EntityFrameworkCore;
using StudentManagement.Helper;
using StudentManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace StudentManagement.Repositories
{
    public class GradeRepository : BaseRepository<Grade>, IGradeRepository
    {
        public GradeRepository(StudentDbContext context) : base(context) { }

        public IEnumerable<Grade> GetGradesByStudent(string studentId)
        {
            return _context.Grades
                    .Where(g => g.StudentID == studentId)
                    .Include(g => g.Course) // Nạp dữ liệu từ bảng Course
                    .ToList();
        }

        public IEnumerable<Grade> GetCoursesByStudentId(string studentId)
        {
            return _context.Grades
                .Include(g => g.Course)
                .Where(g => g.StudentID == studentId)
                .OrderBy(g => g.AcademicYear)
                .ThenBy(g => g.Semester)
                .ToList();
        }

        public Grade GetGradeByStudentAndCourse(string studentId, string courseId)
        {
            return _context.Grades.FirstOrDefault(g => g.StudentID == studentId && g.CourseID == courseId);
        }

        public Grade GetById(int id)
        {
            return _context.Grades.FirstOrDefault(g => g.GradeID == id);
        }

        public void Delete(int id)
        {
            var grade = GetById(id);
            if (grade != null)
            {
                _context.Grades.Remove(grade);
                _context.SaveChanges();
            }
        }
        public void UpdateRegistrations(string studentId, List<string> selectedCourseIds)
        {
            var (semester, academicYear) = DateHelper.GetCurrentSemesterAndAcademicYear();

            var currentGrades = _context.Grades
                .Where(g => g.StudentID == studentId
                            && g.Score == null
                            && g.Semester == semester
                            && g.AcademicYear == academicYear)
                .ToList();

            // Xóa các môn không còn được chọn trong học kỳ hiện tại
            var toRemove = currentGrades
                .Where(g => !selectedCourseIds.Contains(g.CourseID))
                .ToList();

            _context.Grades.RemoveRange(toRemove);

            // Thêm các môn mới (chưa có trong danh sách)
            var existingCourseIds = currentGrades.Select(g => g.CourseID).ToHashSet();
            var toAdd = selectedCourseIds
                .Where(courseId => !existingCourseIds.Contains(courseId))
                .Select(courseId => new Grade
                {
                    StudentID = studentId,
                    CourseID = courseId,
                    Score = null,
                    Semester = semester,
                    AcademicYear = academicYear
                });

            _context.Grades.AddRange(toAdd);
            _context.SaveChanges();
        }
    }
}