// Thêm vào thư mục Services
using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;
using StudentManagement.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StudentManagement.Services
{
    public interface IGradeService
    {
        List<StudentGradeSummary> GetStudentGradeSummaries(string academicYear, string semester, string classId, string majorId);
    }

    public class GradeService : IGradeService
    {
        private readonly StudentDbContext _context;

        public GradeService(StudentDbContext context)
        {
            _context = context;
        }

        public List<StudentGradeSummary> GetStudentGradeSummaries(
    string academicYear = null,
    string semester = null,
    string classId = null,
    string majorId = null)
        {
            if (!string.IsNullOrEmpty(academicYear) && !Regex.IsMatch(academicYear, @"^\d{4}-\d{4}$"))
            {
                throw new ArgumentException("Năm học phải có định dạng YYYY-YYYY");
            }

            var studentsQuery = _context.Students
                .Include(s => s.Class)
                .ThenInclude(c => c.Major)
                .AsQueryable();

            if (!string.IsNullOrEmpty(classId))
            {
                studentsQuery = studentsQuery.Where(s => s.ClassID == classId);
            }

            if (!string.IsNullOrEmpty(majorId))
            {
                studentsQuery = studentsQuery.Where(s => s.Class.MajorID == majorId);
            }

            var summaries = studentsQuery
                .Select(s => new StudentGradeSummary
                {
                    StudentID = s.StudentID,
                    FullName = s.FullName,
                    ClassName = s.Class.ClassName,
                    ClassID = s.Class.ClassID,
                    MajorName = s.Class.Major.MajorName,
                    MajorID = s.Class.MajorID,
                    AcademicYear = academicYear,
                    Semester = semester,
                    AverageScore = _context.Grades
                        .Where(g => g.StudentID == s.StudentID &&
                                    (string.IsNullOrEmpty(academicYear) || g.AcademicYear == academicYear) &&
                                    (string.IsNullOrEmpty(semester) || g.Semester == semester))
                        .Select(g => (float?)g.Score)
                        .Average()
                })
                .Where(s => s.AverageScore.HasValue)
                .OrderByDescending(s => s.AverageScore)
                .ToList();

            return summaries;
        }

    }
}