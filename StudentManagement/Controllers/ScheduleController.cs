using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagement.Models;
using StudentManagement.Repositories;

namespace StudentManagement.Controllers
{
    [Authorize]
    [Authorize(Policy = "RequireAdminRole")]
    public class ScheduleController : Controller
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;

        public ScheduleController(IScheduleRepository scheduleRepository, ICourseRepository courseRepository, ITeacherRepository teacherRepository, IStudentRepository studentRepository, IClassRepository classRepository)
        {
            _scheduleRepository = scheduleRepository;
            _courseRepository = courseRepository;
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
            _classRepository = classRepository;
        }
        public IActionResult Index(string? studentId, DateTime? weekStart)
        {
            DateTime today = DateTime.Today;
            DateTime startOfWeek = weekStart ?? today.AddDays(-(int)today.DayOfWeek + 1); // Mặc định lấy đầu tuần hiện tại (Thứ 2)
            var schedules = new List<Schedule>();

            if (!string.IsNullOrEmpty(studentId))
            {
                schedules = _scheduleRepository.GetSchedulesByWeek(studentId, startOfWeek).ToList();
            }

            ViewBag.StartOfWeek = startOfWeek;
            ViewBag.EndOfWeek = startOfWeek.AddDays(6);
            ViewBag.PrevWeek = startOfWeek.AddDays(-7);
            ViewBag.NextWeek = startOfWeek.AddDays(7);
            ViewBag.StudentId = studentId; 

            return View(schedules);
        }

        public IActionResult Details(int id)
        {
            var schedule = _scheduleRepository.GetById(id);
            if (schedule == null) return NotFound();
            return View(schedule);
        }

        public IActionResult Create(string studentId, string classDate, int? lesson)
        {
            var courses = _courseRepository.GetAll();
            ViewBag.Courses = new SelectList(courses, "CourseID", "CourseName");
            if (string.IsNullOrEmpty(studentId))
            {
                return RedirectToAction(nameof(Index));
            }

            var schedule = new Schedule
            {
                StudentID = studentId,
                ClassDate = DateTime.Parse(classDate)
            };

            // Tự động điền StartTime và EndTime dựa trên tiết học
            if (lesson.HasValue)
            {
                TimeSpan startTime = GetStartTimeFromLesson(lesson.Value);
                schedule.StartTime = startTime;
                schedule.EndTime = startTime.Add(TimeSpan.FromMinutes(45)); // Mặc định 1 tiết = 45 phút
            }

            ViewBag.StudentID = studentId;
            ViewBag.ClassDate = classDate;
            ViewBag.StartTime = schedule.StartTime.ToString(@"hh\:mm");
            ViewBag.EndTime = schedule.EndTime.ToString(@"hh\:mm");
            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                _scheduleRepository.Add(schedule);
                Console.WriteLine(schedule);

                return RedirectToAction(nameof(Index), new { studentId = schedule.StudentID });
            }

            // Lấy danh sách khóa học sinh viên học và có điểm hoặc điểm = null

            var courses = _courseRepository.GetAll();
            ViewBag.Courses = new SelectList(courses, "CourseID", "CourseName");
            ViewBag.StudentID = schedule.StudentID;
            ViewBag.ClassDate = schedule.ClassDate.ToString("yyyy-MM-dd");
            ViewBag.StartTime = schedule.StartTime.ToString(@"hh\:mm");
            ViewBag.EndTime = schedule.EndTime.ToString(@"hh\:mm");
            
            return View(schedule);
        }

        // Hàm helper để tính StartTime từ tiết học
        private TimeSpan GetStartTimeFromLesson(int lesson)
        {
            if (lesson >= 1 && lesson <= 3)
            {
                return TimeSpan.FromMinutes(6 * 60 + 45 + (lesson - 1) * 45); // 6h45 + (lesson-1)*45
            }
            else if (lesson >= 4 && lesson <= 6)
            {
                return TimeSpan.FromMinutes(9 * 60 + 20 + (lesson - 4) * 45); // 9h20 + (lesson-4)*45
            }
            else if (lesson >= 7 && lesson <= 12)
            {
                return TimeSpan.FromMinutes(12 * 60 + 30 + (lesson - 7) * 45); // 12h30 + (lesson-7)*45
            }
            else if (lesson >= 13 && lesson <= 15)
            {
                return TimeSpan.FromMinutes(18 * 60 + (lesson - 13) * 45); // 18h + (lesson-13)*45
            }
            return TimeSpan.Zero;
        }

        public IActionResult Edit(int id)
        {
            var schedule = _scheduleRepository.GetById(id);
            if (schedule == null) return NotFound();

            var courses = _courseRepository.GetAll();
            ViewBag.Courses = new SelectList(courses, "CourseID", "CourseName");

            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Schedule schedule)
        {
            if (id != schedule.ScheduleID) return BadRequest();

            if (ModelState.IsValid)
            {
                _scheduleRepository.Update(schedule);
                return RedirectToAction(nameof(Index), new { studentId = schedule.StudentID });
            }

            var courses = _courseRepository.GetAll();
            ViewBag.Courses = new SelectList(courses, "CourseID", "CourseName");
            return View(schedule);
        }

        public IActionResult Delete(int id)
        {
            var schedule = _scheduleRepository.GetById(id);
            if (schedule == null) return NotFound();
            return View(schedule);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Student student = _scheduleRepository.GetById(id).Student;
            _scheduleRepository.Delete(id);
            return RedirectToAction(nameof(Index), new { studentId = student.StudentID });
        }
    }
}
