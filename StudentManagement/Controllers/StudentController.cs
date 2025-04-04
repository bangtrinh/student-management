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


        [Authorize(Roles = "Admin, Student")]
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

        [Authorize(Roles = "Student")]
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

        [Authorize(Roles = "Student")]
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



        [Authorize(Roles = "Admin")]
        // GET: Student
        public IActionResult Index()
        {
            var students = _studentRepository.GetAll();
            return View(students);
        }

        [Authorize(Roles = "Admin")]

        // GET: Student/Details/5
        public IActionResult Details(string id)
        {
            var student = _studentRepository.GetById(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // GET: Student/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Lấy danh sách các lớp học
            var classes = _classRepository.GetAll();
            ViewBag.Classes = new SelectList(classes, "ClassID", "ClassName");
            return View();
        }

        // POST: Student/Create
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                _studentRepository.Add(student);
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Student/Edit/5
        [Authorize(Roles = "Admin")]

        public IActionResult Edit(string id)
        {
            var student = _studentRepository.GetById(id);
            if (student == null) return NotFound();
            var classes = _classRepository.GetAll();
            ViewBag.Classes = new SelectList(classes, "ClassID", "ClassName", student.ClassID);
            return View(student);
        }

        // POST: Student/Edit/5
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Student student)
        {
            if (id != student.StudentID) return BadRequest();
            if (!ModelState.IsValid)
            {
                _studentRepository.Update(student);
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Student/Delete/5
        [Authorize(Roles = "Admin")]

        public IActionResult Delete(string id)
        {
            var student = _studentRepository.GetById(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // POST: Student/Delete/5
        [Authorize(Roles = "Admin")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            _studentRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}