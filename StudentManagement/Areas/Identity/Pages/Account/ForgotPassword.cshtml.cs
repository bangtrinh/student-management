// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable


using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Controllers;
using StudentManagement.Models;
using StudentManagement.Services;

namespace StudentManagement.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailService _emailService;
        private readonly StudentDbContext _context;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, EmailService emailService, StudentDbContext context)
        {
            _userManager = userManager;
            _emailService = emailService;
            _context = context;
        }

        [BindProperty]
        public ForgotPasswordRequest Input { get; set; }

        public class ForgotPasswordRequest
        {
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy tài khoản với email này.");
                return Page();
            }

            // Tạo mã OTP
            string otp = _emailService.GenerateOTP();
            var otpRecord = new OTP
            {
                Email = Input.Email,
                Code = SecurityHelper.HashSHA256(otp),
                ExpiryTime = DateTime.Now.AddMinutes(5)
            };

            // Lưu OTP vào cơ sở dữ liệu
            _context.OTPS.Add(otpRecord);
            await _context.SaveChangesAsync();

            // Gửi OTP qua email
            string subject = "Mã OTP xác nhận đặt lại mật khẩu";
            string body = $"Mã OTP của bạn là: {otp}. Mã này sẽ hết hạn sau 5 phút.";
            bool isSent = await _emailService.SendEmailAsync(Input.Email, subject, body);

            if (!isSent)
            {
                ModelState.AddModelError(string.Empty, "Không thể gửi email.");
                return Page();
            }

            return RedirectToPage("/Account/VerifyOTP", new { email = Input.Email });
        }
    }
}
