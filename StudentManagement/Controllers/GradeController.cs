using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;
using StudentManagement.Repositories;

namespace StudentManagement.Controllers
{
    public class GradeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IStudentRepository _studentRepository;
        private readonly IGradeRepository _gradeRepository;

        public GradeController(UserManager<IdentityUser> userManager, IStudentRepository studentRepository, IGradeRepository gradeRepository)
        {
            _userManager = userManager;
            _studentRepository = studentRepository;
            _gradeRepository = gradeRepository;
        }

        [Authorize(Roles = "Student")]
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

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var grades = _gradeRepository.GetAll();
            return View(grades);
        }

        [Authorize(Roles = "Admin")]

        public IActionResult Details(string id)
        {
            var grade = _gradeRepository.GetById(id);
            if (grade == null) return NotFound();
            return View(grade);
        }

        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Grade grade)
        {
            if (ModelState.IsValid)
            {
                _gradeRepository.Add(grade);
                return RedirectToAction(nameof(Index));
            }
            return View(grade);
        }

        [Authorize(Roles = "Admin, Teacher")]

        public IActionResult EditGrade(string id)
        {
            var grade = _gradeRepository.GetById(id);
            if (grade == null) return NotFound();
            return View(grade);
        }

        [Authorize(Roles = "Admin, Teacher")]
        // Action để lưu điểm đã chỉnh sửa (POST)
        [HttpPost]
        public IActionResult EditGrade([FromBody] GradeUpdateModel model)
        {
            if (model == null || model.Grade < 0 || model.Grade > 10)
            {
                return BadRequest(new { success = false, message = "Điểm không hợp lệ! Điểm phải từ 0 đến 10." });
            }

            var gradeToUpdate = _gradeRepository.GetById(model.GradeId);
            if (gradeToUpdate == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy điểm cần sửa." });
            }

            gradeToUpdate.Score = model.Grade;
            _gradeRepository.Update(gradeToUpdate);

            return Json(new { success = true, message = "Điểm đã được cập nhật thành công!" });
        }

        // Model nhận dữ liệu từ AJAX
        public class GradeUpdateModel
        {
            public int GradeId { get; set; }
            public float Grade { get; set; }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            var grade = _gradeRepository.GetById(id);
            if (grade == null) return NotFound();
            return View(grade);
        }

        [Authorize(Roles = "Admin")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            _gradeRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

      

    }
}