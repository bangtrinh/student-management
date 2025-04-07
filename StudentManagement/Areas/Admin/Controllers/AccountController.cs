using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Areas.Admin.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using static StudentManagement.Areas.Identity.Pages.Account.Manage.ChangePasswordModel;

namespace StudentManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminRole")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }


        // GET: Hiển thị danh sách người dùng
        [HttpGet]
        public async Task<IActionResult> Index(string searchString)
        {
            // Lấy toàn bộ danh sách người dùng vào bộ nhớ trước
            var users = await _userManager.Users.ToListAsync();

            // Nếu có chuỗi tìm kiếm, lọc danh sách người dùng dựa trên email
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                users = users.Where(u => u.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var userRoles = new Dictionary<string, IList<string>>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles;
            }

            ViewData["UserRoles"] = userRoles;

            return View(users);
        }
        // GET: Chuyển hướng đến trang Register để tạo tài khoản mới
        public IActionResult Create()
        {
            return RedirectToPage("/Account/Register", new { area = "Identity", returnUrl = Url.Action("Index", "Account", new { area = "Admin" }) });
        }

        // GET: Hiển thị form chỉnh sửa tài khoản
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault(),
                RoleList = _roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                })
            };

            return View(model);
        }

        // POST: Xử lý chỉnh sửa tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.RoleList = _roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                });
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Cập nhật vai trò
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!string.IsNullOrEmpty(model.Role) && await _roleManager.RoleExistsAsync(model.Role))
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                }

                TempData["SuccessMessage"] = "Tài khoản đã được cập nhật thành công.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            model.RoleList = _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name
            });
            return View(model);
        }

        // GET: Xác nhận xóa tài khoản
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Xử lý xóa tài khoản
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Tài khoản đã được xóa thành công.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user);
        }

        // GET: Hiển thị form đổi mật khẩu
        [Authorize(Policy = "AllowAll")]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: Xử lý đổi mật khẩu
        [Authorize(Policy = "AllowAll")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Người dùng không tồn tại hoặc phiên đăng nhập không hợp lệ.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Tiến hành thay đổi mật khẩu
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                // Làm mới phiên đăng nhập
                await _signInManager.RefreshSignInAsync(user);

                TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công.";

                // Kiểm tra vai trò và điều hướng phù hợp
                if (await _userManager.IsInRoleAsync(user, "Student"))
                {
                    return RedirectToAction("Profile", "Student", new { area = "" }); // Điều chỉnh area nếu cần
                }
                else if (await _userManager.IsInRoleAsync(user, "Teacher"))
                {
                    return RedirectToAction("Profile", "Teacher", new { area = "" }); // Điều chỉnh area nếu cần
                }
                else if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction(nameof(Index));
                }

                // Trường hợp không xác định vai trò
                TempData["ErrorMessage"] = "Không thể xác định vai trò của người dùng.";
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            // Xử lý lỗi nếu đổi mật khẩu thất bại
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}