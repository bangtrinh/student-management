using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagement.Models;
using StudentManagement.Repositories;

namespace StudentManagement.Controllers
{
    [Authorize]
    public class TeacherController : Controller
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IDepartmentRepository _departmentRepository;


        public TeacherController(ITeacherRepository teacherRepository, ICourseRepository courseRepository, IScheduleRepository scheduleRepository, UserManager<IdentityUser> userManager, IDepartmentRepository departmentRepository)
        {
            _courseRepository = courseRepository;
            _userManager = userManager;
            _teacherRepository = teacherRepository;
            _scheduleRepository = scheduleRepository;
            _departmentRepository = departmentRepository;
        }

     
        //Tìm kiếm
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult Index(string searchString)
        {
            var teachers = _teacherRepository.GetAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                teachers = teachers.Where(t => t.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                               t.TeacherID.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                               t.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(teachers);
        }
        [Authorize(Policy = "RequireTeacherRole")]

        public async Task<IActionResult> MyCourses()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var teacher = _teacherRepository.GetTeacherByEmail(user.Email);
            if (teacher == null)
            {
                ViewBag.Message = "Không tìm thấy thông tin giảng viên.";
                return View(new List<Grade>());
            }

            var courses = _courseRepository.GetCoursesByTeacher(teacher.TeacherID);

            return View(courses);
        }
        
        [Authorize(Policy = "RequireAdminOrTeacher")]
        public IActionResult Details(string id)
        {
            var teacher = _teacherRepository.GetById(id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }

        [Authorize(Policy = "RequireTeacherRole")]
        public async Task<IActionResult> Schedule(DateTime? weekStart)
        {
            // Xác định tuần bắt đầu
            var startOfWeek = weekStart ?? DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            var endOfWeek = startOfWeek.AddDays(6);

            // Lấy thông tin người dùng
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy mã giáo viên từ email của người dùng
            var teacher = _teacherRepository.GetTeacherByEmail(user.Email);
            if (teacher == null)
            {
                return NotFound("Không tìm thấy giáo viên với email này.");
            }

            string teacherId = teacher.TeacherID;

            // Lấy thời khóa biểu cho giáo viên trong tuần được chọn
            var schedule = _scheduleRepository.GetScheduleByTeacher(teacherId, startOfWeek);
                                              
            // Tạo ViewBag cho điều hướng tuần
            ViewBag.StartOfWeek = startOfWeek;
            ViewBag.EndOfWeek = endOfWeek;
            ViewBag.PrevWeek = startOfWeek.AddDays(-7);
            ViewBag.NextWeek = startOfWeek.AddDays(7);

            return View(schedule);
        }

        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult Create()
        {
            var departments = _departmentRepository.GetAll();
            ViewBag.Departments = new SelectList(departments, "DepartmentID", "DepartmentName");
            return View();
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            var departments = _departmentRepository.GetAll();
            ViewBag.Departments = new SelectList(departments, "DepartmentID", "DepartmentName");
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = teacher.Email,
                    Email = teacher.Email,
                    PhoneNumber = teacher.PhoneNumber,
                    EmailConfirmed = true // Xác nhận email tự động
                };

                // Mật khẩu mặc định
                string defaultPassword = "Abc@123"; // Hoặc có thể tạo password ngẫu nhiên

                // Tạo user trong Identity
                var result = await _userManager.CreateAsync(user, defaultPassword);

                if (result.Succeeded)
                {
                    // Thêm role Student cho user
                    await _userManager.AddToRoleAsync(user, "Teacher");
                    _teacherRepository.Add(teacher);
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(teacher);
        }

        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult Edit(string id)
        {
          
            var teacher = _teacherRepository.GetById(id);
            if (teacher == null) return NotFound();
            var departments = _departmentRepository.GetAll();
            ViewBag.Departments = new SelectList(departments, "DepartmentID", "DepartmentName");
            return View(teacher);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Teacher teacher)
        {
            var departments = _departmentRepository.GetAll();
            ViewBag.Departments = new SelectList(departments, "DepartmentID", "DepartmentName");
            if (id != teacher.TeacherID) return BadRequest();
            if (ModelState.IsValid)
            {
                _teacherRepository.Update(teacher);
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult Delete(string id)
        {
            var teacher = _teacherRepository.GetById(id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }

        [Authorize(Policy = "RequireAdminRole")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            _teacherRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}