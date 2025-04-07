using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Localization;


using StudentManagement.Helper;
using StudentManagement.Models;
using StudentManagement.Repositories;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace StudentManagement.Controllers
{
    [Authorize]
    [Authorize(Policy = "RequireStudentRole")]
    public class CourseRegisterController : Controller
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CourseRegisterController(
            UserManager<IdentityUser> userManager,
            IGradeRepository gradeRepository,
            IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            IStringLocalizer<SharedResources> localizer)
        {
            _gradeRepository = gradeRepository;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
            _localizer = localizer;
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
            

            // Lấy tất cả các môn học trong hệ thống
            var allCourses = _courseRepository.GetAll() ?? new List<Course>();

            // Lấy danh sách các môn học mà sinh viên đã đăng ký
            var registeredCourses = _gradeRepository.GetCoursesByStudentId(student.StudentID)
                ?? new List<Grade>();

            // Chuyển thành danh sách CourseRegistrationItem
            var model = allCourses
                .Where(c =>
                {
                    var reg = registeredCourses.FirstOrDefault(r => r.CourseID == c.CourseID);
                    return reg == null || reg.Score == null;
                })
                .Select(c =>
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
                })
                .ToList();

            ViewBag.StudentId = student.StudentID;
            return View(model);
        }

        // Xử lý đăng ký môn học
        [HttpPost]
        public async Task<IActionResult> Index(List<CourseRegistrationItem> model)
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

            // Gửi email xác nhận
            await SendConfirmationEmail(student.Email, student.FullName, model.Where(x => x.IsSelected).ToList());

            TempData["Success"] = _localizer["RegisterSuccess"].Value;
            return RedirectToAction("Index");
        }


        private async Task SendConfirmationEmail(string toEmail, string studentName, List<CourseRegistrationItem> courses)
        {
            if (string.IsNullOrEmpty(toEmail) || !courses.Any()) return;

            string subject = "Xác nhận đăng ký môn học";
            StringBuilder body = new StringBuilder();
            body.AppendLine($"Chào {studentName},");
            body.AppendLine("\nBạn đã đăng ký thành công các môn học sau:");
            body.AppendLine("-------------------------------------------");

            foreach (var course in courses)
            {
                body.AppendLine($"- {course.CourseName} | Giảng viên: {course.TeacherName} | Phòng: {course.Room}");
            }

            body.AppendLine("\nCảm ơn bạn!");
            body.AppendLine("Student Management System");

            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new NetworkCredential("quanlisvhs@gmail.com", "kjak ucgm bbpj mgdo");
                client.EnableSsl = true;

                var mailMessage = new MailMessage("your-email@gmail.com", toEmail, subject, body.ToString());
                await client.SendMailAsync(mailMessage);
            }
        }
        public IActionResult Search(string keyword)
        {
            var courses = _courseRepository.GetAll()
                .Where(c => c.CourseName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            c.CourseID.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .Select(c => new CourseRegistrationItem
                {
                    CourseID = c.CourseID,
                    CourseName = c.CourseName,
                    TeacherName = c.Teacher?.FullName,
                    Room = c.Room,
                    IsSelected = false
                }).ToList();

            return View("Index", courses);
        }


    }
}
