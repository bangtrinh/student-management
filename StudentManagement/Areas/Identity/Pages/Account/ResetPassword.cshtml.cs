using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ResetPasswordModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public ResetPasswordModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty(SupportsGet = true)]
    public string Email { get; set; }

    [BindProperty]
    public string NewPassword { get; set; }

    [BindProperty]
    public string ConfirmPassword { get; set; }

    public void OnGet(string email)
    {
        Email = email;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(NewPassword) || NewPassword != ConfirmPassword)
        {
            ModelState.AddModelError("", "Mật khẩu xác nhận không khớp hoặc trống.");
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Email);
        if (user == null)
        {
            ModelState.AddModelError("", "Người dùng không tồn tại.");
            return Page();
        }

        // Xóa mật khẩu cũ trước khi thêm mới
        var removeResult = await _userManager.RemovePasswordAsync(user);
        if (!removeResult.Succeeded)
        {
            ModelState.AddModelError("", "Không thể xóa mật khẩu cũ.");
            return Page();
        }

        var addPasswordResult = await _userManager.AddPasswordAsync(user, NewPassword);
        if (!addPasswordResult.Succeeded)
        {
            ModelState.AddModelError("", "Không thể đặt lại mật khẩu.");
            return Page();
        }

        TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công!";
        return RedirectToPage("/Account/Login");
    }
}
