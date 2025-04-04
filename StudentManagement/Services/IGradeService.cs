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

        public List<StudentGradeSummary> GetStudentGradeSummaries(string academicYear, string semester, string classId = null, string majorId = null)
        {
            if (!string.IsNullOrEmpty(academicYear) && !Regex.IsMatch(academicYear, @"^\d{4}-\d{4}$"))
            {
                throw new ArgumentException("Năm học phải có định dạng YYYY-YYYY");
            }

            var query = _context.Students
                        .Include(s => s.Class)
                        .ThenInclude(c => c.Major)
                    .AsQueryable();

            // Áp dụng filter
            if (!string.IsNullOrEmpty(classId))
            {
                query = query.Where(s => s.ClassID == classId);
            }

            if (!string.IsNullOrEmpty(majorId))
            {
                query = query.Where(s => s.Class.MajorID == majorId);
            }

            var summaries = query.Select(s => new StudentGradeSummary
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
                              g.AcademicYear == academicYear &&
                              g.Semester == semester)
                    .Average(g => g.Score)
            })
            .Where(s => s.AverageScore.HasValue)
            .OrderByDescending(s => s.AverageScore)
            .ToList();

            return summaries;
        }
    }
}