using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration.UserSecrets;
using StudentManagement.Helper;
using StudentManagement.Models;
using StudentManagement.Repositories;

namespace StudentManagement.Controllers
{
    [Authorize]
    [Authorize(Roles = "Student")]
    public class CourseRegisterController : Controller
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public CourseRegisterController(
            UserManager<IdentityUser> userManager,
            IGradeRepository gradeRepository,
            IStudentRepository studentRepository,
            ICourseRepository courseRepository)
        {
            _gradeRepository = gradeRepository;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var student = _studentRepository.GetStudentByEmail(user.Email);
            if (student == null)
            {
                return NotFound();
            }

            var allCourses = _courseRepository.GetCoursesByMajor(student.Class.MajorID)
                ?? new List<Course>();
            var registeredCourses = _gradeRepository.GetCoursesByStudentId(student.StudentID)
                ?? new List<Grade>();

            // Chuyển thành danh sách CourseRegistrationItem
            var model = allCourses.Select(c =>
            {
                var reg = registeredCourses.FirstOrDefault(r => r.CourseID == c.CourseID);
                return new CourseRegistrationItem
                {
                    CourseID = c.CourseID,
                    CourseName = c.CourseName,
                    TeacherName = c.Teacher?.FullName,
                    Room = c.Room,
                    IsSelected = reg != null && reg.Score == null
                };
            }).ToList();

            ViewBag.StudentId = student.StudentID;
            return View(model); // Đúng kiểu List<CourseRegistrationItem>
        }

        // Xử lý đăng ký môn học
        [HttpPost]
        public async Task <IActionResult> Index(List<CourseRegistrationItem> model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var student = _studentRepository.GetStudentByEmail(user.Email);
            if (student == null)
            {
                return NotFound();
            }

            var selectedCourseIds = model
                .Where(x => x.IsSelected)
                .Select(x => x.CourseID)
                .ToList();

            _gradeRepository.UpdateRegistrations(student.StudentID, selectedCourseIds);

            TempData["Success"] = "Cập nhật đăng ký thành công.";
            return RedirectToAction("Index");
        }
    }
}
