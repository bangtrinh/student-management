using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;
using System.Linq;
using StudentManagement.Controllers;

public class VerifyOTPModel : PageModel
{
    private readonly StudentDbContext _context;

    public VerifyOTPModel(StudentDbContext context)
    {
        _context = context;
    }

 
    [BindProperty]
    public string OTP { get; set; }

    [BindProperty(SupportsGet = true)] 
    public string Email { get; set; }

  
    public IActionResult OnGet(string email)
    {
        Email = email; 
        return Page();
    }

    
    public IActionResult OnPost()
    {
        if (string.IsNullOrEmpty(OTP))
        {
            ModelState.AddModelError("", "Vui lòng nhập mã OTP.");
            return Page();
        }


        string hashedOtp = SecurityHelper.HashSHA256(OTP);
        var storedOtp = _context.OTPS
            .FirstOrDefault(o => o.Email == Email && o.Code == hashedOtp);



        if (storedOtp != null && storedOtp.ExpiryTime > DateTime.Now)
        {
      
            return RedirectToPage("/Account/ResetPassword", new { email = Email });
        }
        else
        {
            ModelState.AddModelError("", "Mã OTP không hợp lệ hoặc đã hết hạn.");
            return Page();
        }
    }
}
