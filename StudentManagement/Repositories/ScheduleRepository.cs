using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;

namespace StudentManagement.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly StudentDbContext _context;

        public ScheduleRepository(StudentDbContext context)
        {
            _context = context;
        }


        public IEnumerable<Schedule> GetAll()
        {
            return _context.Schedules
                .Include(s => s.Course)
                .ThenInclude(s => s.Teacher)
                .Include(s => s.Student)
                .OrderBy(s => s.ClassDate)
                .ThenBy(s => s.StartTime)
                .ToList();
        }

        public IEnumerable<Schedule> GetScheduleByClass(string classId, DateTime startDate)
        {
            DateTime endOfWeek = startDate.AddDays(6);
            return _context.Schedules
             .Include(s => s.Student) // Bao gồm thông tin Student để lấy ClassID
             .Include(s => s.Course)  // Bao gồm thông tin Course để hiển thị CourseName
                .ThenInclude(c => c.Teacher) // Bao gồm thông tin Teacher để hiển thị tên giáo viên
             .Where(s => s.Student.ClassID == classId && s.ClassDate >= startDate && s.ClassDate <= endOfWeek) // Lọc theo ClassID từ Student
             .OrderBy(s => s.ClassDate)
                .ThenBy(s => s.StartTime)
             .ToList();
        }
        public IEnumerable<Schedule> GetScheduleByStudent(string studentId, DateTime startDate, DateTime endDate)
        {
            return _context.Schedules
                .Where(s => s.StudentID == studentId)
                .Include(s => s.Course)
                .ThenInclude(s => s.Teacher)
                .OrderBy(s => s.ClassDate)
                .ThenBy(s => s.StartTime)
                .ToList();
        }

        public IEnumerable<Schedule> GetSchedulesByWeek(string studentId, DateTime startOfWeek)
        {
            DateTime endOfWeek = startOfWeek.AddDays(6);

            return _context.Schedules
                .Where(s => s.StudentID == studentId && s.ClassDate >= startOfWeek && s.ClassDate <= endOfWeek)
                .Include(s => s.Course)
                .ThenInclude(s => s.Teacher)
                .OrderBy(s => s.ClassDate)
                .ThenBy(s => s.StartTime)
                .ToList();
        }

        public IEnumerable<Schedule> GetScheduleByTeacher(string teacherId, DateTime startOfWeek)
        {
            DateTime endOfWeek = startOfWeek.AddDays(6);

            return _context.Schedules
                .Include(s => s.Course)  // 👈 Thêm Include để load Course
                .Where(s => s.Course.TeacherID == teacherId && s.ClassDate >= startOfWeek && s.ClassDate <= endOfWeek)
                .OrderBy(s => s.ClassDate)
                .ThenBy(s => s.StartTime)
                .ToList();
        }

        public Schedule GetById(int id)
        {
            return _context.Schedules
                .Include(s => s.Course)
                .ThenInclude(s => s.Teacher)
                .Include(s => s.Student)
                .FirstOrDefault(s => s.ScheduleID == id);
        }

        public bool IsScheduleConflict(string studentId, int scheduleID, DateTime classDate, TimeSpan startTime, TimeSpan endTime)
        {
            var minStart = new TimeSpan(6, 30, 0); // 06:30
            var maxEnd = new TimeSpan(20, 15, 0);  // 20:15

            // Kiểm tra thời gian có hợp lệ không
            if (startTime < minStart || endTime > maxEnd || startTime >= endTime)
            {
                return true; // Sai điều kiện => xung đột
            }

            return _context.Schedules.Any(s =>
                s.ScheduleID != scheduleID &&
                s.StudentID == studentId &&
                s.ClassDate == classDate &&
                ((s.StartTime < endTime && s.EndTime > startTime)));
        }
        public void Add(Schedule schedule)
        {
            _context.Schedules.Add(schedule);
            _context.SaveChanges();
        }


        public void Update(Schedule schedule)
        {
            _context.Schedules.Update(schedule);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var schedule = GetById(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
                _context.SaveChanges();
            }
        }
    }
}