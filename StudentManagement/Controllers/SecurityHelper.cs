using System.Text;
using System.Security.Cryptography;


namespace StudentManagement.Controllers
{
    public static class SecurityHelper
    {
        public static string HashSHA256(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha.ComputeHash(bytes);
                return Convert.ToHexString(hashBytes); // .NET 5+ dùng .ToHexString
            }
        }
    }
}