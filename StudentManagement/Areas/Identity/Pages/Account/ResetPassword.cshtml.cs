using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

public class ResetPasswordModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IStringLocalizer<ResetPasswordModel> _localizer;

    public ResetPasswordModel(UserManager<IdentityUser> userManager, IStringLocalizer<ResetPasswordModel> localizer)
    {
        _userManager = userManager;
        _localizer = localizer;
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
            ModelState.AddModelError("", _localizer["Mật khẩu xác nhận không khớp hoặc trống."]);
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Email);
        if (user == null)
        {
            ModelState.AddModelError("", _localizer["Người dùng không tồn tại."]);
            return Page();
        }

        var removeResult = await _userManager.RemovePasswordAsync(user);
        if (!removeResult.Succeeded)
        {
            ModelState.AddModelError("", _localizer["Không thể xóa mật khẩu cũ."]);
            return Page();
        }

        var addPasswordResult = await _userManager.AddPasswordAsync(user, NewPassword);
        if (!addPasswordResult.Succeeded)
        {
            ModelState.AddModelError("", _localizer["Không thể đặt lại mật khẩu."]);
            return Page();
        }

        TempData["SuccessMessage"] = _localizer["Mật khẩu đã được thay đổi thành công!"].Value;
        return RedirectToPage("/Account/Login");
    }
}
