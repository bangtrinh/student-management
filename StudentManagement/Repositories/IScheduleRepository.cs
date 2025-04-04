using System;
using System.Collections.Generic;
using StudentManagement.Models;

namespace StudentManagement.Repositories
{
    public interface IScheduleRepository
    {
        IEnumerable<Schedule> GetAll();
        public Schedule GetById(int id);

        public void Add(Schedule schedule);
        public void Update(Schedule schedule);
        public void Delete(int id);

        IEnumerable<Schedule> GetScheduleByClass(string classId, DateTime startDate);

        IEnumerable<Schedule> GetScheduleByStudent(string studentId, DateTime startDate, DateTime endDate);
        IEnumerable<Schedule> GetSchedulesByWeek(string studentId, DateTime startOfWeek);
        IEnumerable<Schedule> GetScheduleByTeacher(string teacherId, DateTime startOfWeek);
        bool IsScheduleConflict(string studentId, DateTime classDate, TimeSpan startTime, TimeSpan endTime);
    }
}
