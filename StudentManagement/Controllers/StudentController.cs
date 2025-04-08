using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;
using StudentManagement.Repositories;

namespace StudentManagement.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IStudentRepository _studentRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IClassRepository _classRepository;
        private readonly ICourseRepository _courseRepository;
        public StudentController(IStudentRepository studentRepository, IGradeRepository gradeRepository, UserManager<IdentityUser> userManager, IScheduleRepository scheduleRepository, IClassRepository classRepository, ICourseRepository courseRepository)
        {
            _studentRepository = studentRepository;
            _gradeRepository = gradeRepository;
            _userManager = userManager;
            _scheduleRepository = scheduleRepository;
            _classRepository = classRepository;
            _courseRepository = courseRepository;
        }


        [Authorize(Policy = "RequireAdminOrStudent")]
        public async Task<IActionResult> Profile()
        {
            // Lấy user hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy email từ user
            string userEmail = user.Email;

            // Tìm sinh viên có email trùng với user hiện tại
            var student = _studentRepository.GetStudentByEmail(userEmail);

            if (student == null)
            {
                ViewBag.Message = "Chưa có thông tin sinh viên";
                return View();
            }

            return View(student);
        }

        [Authorize(Policy = "RequireStudentRole")]
        public async Task<IActionResult> MyCourses()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Tìm sinh viên theo email
            var student = _studentRepository.GetStudentByEmail(user.Email);
            if (student == null)
            {
                ViewBag.Message = "Không tìm thấy thông tin sinh viên.";
                return View(new List<Grade>());
            }

            // Lấy danh sách môn học từ Repository
            var courses = _gradeRepository.GetCoursesByStudentId(student.StudentID);

            return View(courses);
        }

        [Authorize(Policy = "RequireStudentRole")]
        public async Task<IActionResult> Schedule(DateTime? weekStart)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var student = _studentRepository.GetStudentByEmail(user.Email);
            if (student == null) return View("Error", "Không tìm thấy thông tin sinh viên");

            DateTime today = DateTime.Today;
            DateTime startOfWeek = weekStart ?? today.AddDays(-(int)today.DayOfWeek + 1); // Mặc định lấy đầu tuần hiện tại (Thứ 2)

            var schedules = _scheduleRepository.GetSchedulesByWeek(student.StudentID, startOfWeek);

            ViewBag.StartOfWeek = startOfWeek;
            ViewBag.EndOfWeek = startOfWeek.AddDays(6);
            ViewBag.PrevWeek = startOfWeek.AddDays(-7);
            ViewBag.NextWeek = startOfWeek.AddDays(7);

            return View(schedules);
        }



        [Authorize(Policy = "RequireAdminRole")]
        // TÌM KIẾM
        public IActionResult Index(string searchString, string sortOrder)
        {
            var students = _studentRepository.GetAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                               s.StudentID.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                               s.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            students = sortOrder == "desc"
                ? students.OrderByDescending(s => s.FullName).ToList()
                : students.OrderBy(s => s.FullName).ToList();

            return View(students);
        }

        [Authorize(Policy = "RequireStudentRole")]
        public async Task<IActionResult> MyGrades()
        {
            // Lấy user hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy email của user
            string userEmail = user.Email;

            // Tìm StudentID của sinh viên có email trùng với user đăng nhập
            var student = _studentRepository.GetStudentByEmail(userEmail);
            if (student == null)
            {
                ViewBag.Message = "Bạn chưa có thông tin sinh viên.";
                return View();
            }

            // Lấy danh sách điểm của sinh viên theo StudentID
            var grades = _gradeRepository.GetGradesByStudent(student.StudentID);

            return View(grades);
        }


        [Authorize(Policy = "RequireAdminRole")]

        // GET: Student/Details/5
        public IActionResult Details(string id)
        {
            var student = _studentRepository.GetById(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // GET: Student/Create
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult Create()
        {
            // Lấy danh sách các lớp học
            var classes = _classRepository.GetAll();
            ViewBag.Classes = new SelectList(classes, "ClassID", "ClassName");
            return View();
        }

        // POST: Student/Create
        [Authorize(Policy = "RequireAdminRole")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = student.Email,
                    Email = student.Email,
                    PhoneNumber = student.PhoneNumber,
                    EmailConfirmed = true // Xác nhận email tự động
                };

                // Mật khẩu mặc định
                string defaultPassword = "Abc@123"; // Hoặc có thể tạo password ngẫu nhiên

                // Tạo user trong Identity
                var result = await _userManager.CreateAsync(user, defaultPassword);

                if (result.Succeeded)
                {
                    // Thêm role Student cho user
                    await _userManager.AddToRoleAsync(user, "Student");
                    _studentRepository.Add(student);
                    return RedirectToAction(nameof(Index));
                }
            }
            var classes = _classRepository.GetAll();
            ViewBag.Classes = new SelectList(classes, "ClassID", "ClassName");
            return View(student);
        }


        // GET: Student/Edit/5
        [Authorize(Policy = "RequireAdminRole")]

        public IActionResult Edit(string id)
        {
            var student = _studentRepository.GetById(id);
            if (student == null) return NotFound();
            var classes = _classRepository.GetAll();
            ViewBag.Classes = new SelectList(classes, "ClassID", "ClassName", student.ClassID);
            return View(student);
        }

        // POST: Student/Edit/5
        [Authorize(Policy = "RequireAdminRole")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Student student)
        {
            if (id != student.StudentID) return BadRequest();
            if (ModelState.IsValid)
            {
                _studentRepository.Update(student);
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Student/Delete/5
        [Authorize(Policy = "RequireAdminRole")]

        public IActionResult Delete(string id)
        {
            var student = _studentRepository.GetById(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // POST: Student/Delete/5
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            _studentRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Policy = "RequireStudentRole")]
        public IActionResult SearchCourse(string keyword)
        {
            var courses = _courseRepository.GetAll(); // Lấy danh sách môn học đang mở

            if (!string.IsNullOrEmpty(keyword))
            {
                // Lọc kết quả theo từ khóa
                courses = courses.Where(c => c.CourseName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                                             c.CourseID.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View("Register", courses); // Hiển thị kết quả tìm kiếm trên giao diện đăng ký môn học
        }

    }
}