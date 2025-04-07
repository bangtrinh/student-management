using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using StudentManagement.Models;
using StudentManagement.Repositories;
using StudentManagement.Resources; 
using StudentManagement.Helpers;


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
        private readonly IStringLocalizer<SharedResources> _localizer;


        public ScheduleController(
            IScheduleRepository scheduleRepository, 
            ICourseRepository courseRepository, 
            ITeacherRepository teacherRepository, 
            IStudentRepository studentRepository, 
            IClassRepository classRepository,
            IStringLocalizer<SharedResources> localizer)
        {
            _scheduleRepository = scheduleRepository;
            _courseRepository = courseRepository;
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _localizer = localizer;

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
            var courses = _courseRepository.GetCoursesByStudent(studentId);
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
                TimeSpan startTime = ScheduleHelper.GetStartTimeFromLesson(lesson.Value);
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
            // Kiểm tra hợp lệ và không trùng lịch
            if (ModelState.IsValid && !_scheduleRepository.IsScheduleConflict(
                schedule.StudentID, schedule.ScheduleID, schedule.ClassDate, schedule.StartTime, schedule.EndTime))
            {
                _scheduleRepository.Add(schedule);
                return RedirectToAction(nameof(Index), new { studentId = schedule.StudentID });
            }

            // Nếu bị trùng hoặc không hợp lệ
            TempData["ErrorMessage"] = _localizer["ConflictSchedule"].Value;

            // Gửi lại dữ liệu cần cho View để render lại form
            var courses = _courseRepository.GetCoursesByStudent(schedule.StudentID);
            ViewBag.Courses = new SelectList(courses, "CourseID", "CourseName");
            ViewBag.StudentID = schedule.StudentID;
            ViewBag.ClassDate = schedule.ClassDate.ToString("yyyy-MM-dd");
            ViewBag.StartTime = schedule.StartTime.ToString(@"hh\:mm");
            ViewBag.EndTime = schedule.EndTime.ToString(@"hh\:mm");

            return RedirectToAction(nameof(Index), new { studentId = schedule.StudentID });
        }

        public IActionResult Edit(int id)
        {
            var schedule = _scheduleRepository.GetById(id);
            if (schedule == null) return NotFound();

            var courses = _courseRepository.GetCoursesByStudent(schedule.StudentID);
            ViewBag.Courses = new SelectList(courses, "CourseID", "CourseName");

            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Schedule schedule)
        {
            if (id != schedule.ScheduleID) return BadRequest();

            if (ModelState.IsValid && !_scheduleRepository.IsScheduleConflict(
                schedule.StudentID, schedule.ScheduleID, schedule.ClassDate, schedule.StartTime, schedule.EndTime))
            {
                _scheduleRepository.Update(schedule);
                return RedirectToAction(nameof(Index), new { studentId = schedule.StudentID });
            }

            TempData["ErrorMessage"] = _localizer["ConflictSchedule"].Value;

            var courses = _courseRepository.GetCoursesByStudent(schedule.StudentID);
            ViewBag.Courses = new SelectList(courses, "CourseID", "CourseName");
            return RedirectToAction(nameof(Index), new { studentId = schedule.StudentID });
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
