using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Net.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace StudentManagement.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Student Management", emailSettings["Username"]));
                email.To.Add(new MailboxAddress("", toEmail));
                email.Subject = subject;

                var bodyBuilder = new BodyBuilder { TextBody = body };
                email.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(emailSettings["Host"], Convert.ToInt32(emailSettings["Port"]), false);
                    await client.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
                    await client.SendAsync(email);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
