using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;
using System.Linq;

public class VerifyOTPModel : PageModel
{
    private readonly StudentDbContext _context;

    public VerifyOTPModel(StudentDbContext context)
    {
        _context = context;
    }

    // Dùng BindProperty cho OTP và Email
    [BindProperty]
    public string OTP { get; set; }

    [BindProperty(SupportsGet = true)] // Bind Email từ query string
    public string Email { get; set; }

    // OnGet sẽ lấy Email từ URL
    public IActionResult OnGet(string email)
    {
        Email = email; // Gán giá trị email
        return Page();
    }

    // OnPost sẽ xác thực OTP
    public IActionResult OnPost()
    {
        if (string.IsNullOrEmpty(OTP))
        {
            ModelState.AddModelError("", "Vui lòng nhập mã OTP.");
            return Page();
        }

        // Tìm OTP trong cơ sở dữ liệu
        var storedOtp = _context.OTPS
            .FirstOrDefault(o => o.Email == Email && o.Code == OTP);

        // Kiểm tra xem OTP có hợp lệ không và thời gian hết hạn
        if (storedOtp != null && storedOtp.ExpiryTime > DateTime.Now)
        {
            // Nếu OTP hợp lệ, chuyển hướng tới trang reset mật khẩu
            return RedirectToPage("/Account/ResetPassword", new { email = Email });
        }
        else
        {
            ModelState.AddModelError("", "Mã OTP không hợp lệ hoặc đã hết hạn.");
            return Page();
        }
    }
}
