using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;
using StudentManagement.Models.ViewModel;
using StudentManagement.Repositories;

namespace StudentManagement.Controllers
{
    [Authorize]
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

        [Authorize(Policy = "RequireAdminOrTeacher")]
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

    }
}