using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
    // Controller dùng để xử lý trang ChatBot
    public class ChatBotController : Controller
    {
        // Lưu cấu hình (để lấy API Key và Model)
        private readonly IConfiguration _configuration;

        // HttpClient để gọi API bên ngoài (Hugging Face)
        private readonly HttpClient _httpClient;

        // Constructor: Inject IConfiguration
        public ChatBotController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        // Action hiển thị view ChatBot (GET /ChatBot)
        public IActionResult Index()
        {
            return View();
        }

        // Action nhận tin nhắn từ người dùng (POST /ChatBot/SendMessage)
        [HttpPost]
        public async Task<IActionResult> SendMessage(string message)
        {
            // Lấy API key và model từ appsettings.json
            var apiKey = _configuration["HuggingFace:ApiKey"];
            var model = _configuration["HuggingFace:Model"];

            // Gắn Bearer token để gọi API Hugging Face
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            // ===== XỬ LÝ LOGIC ĐỊNH SẴN (nếu câu hỏi khớp nội dung cố định) =====
            string normalized = message.ToLower(); // Chuẩn hóa chữ thường để so sánh

            if (normalized.Contains("đăng ký môn") || normalized.Contains("dang ky mon"))
            {
                return Json(new
                {
                    success = true,
                    reply = "Hướng dẫn đăng ký môn học:\n1. Vào mục 'Đăng ký môn' ở menu bên trái.\n2. Chọn học kỳ bạn muốn.\n3. Tích vào môn cần đăng ký rồi nhấn 'Đăng ký'.\n4. Kiểm tra lại trong mục 'Môn học'."
                });
            }

            if (normalized.Contains("xem điểm") || normalized.Contains("diem so"))
            {
                return Json(new
                {
                    success = true,
                    reply = "Để xem điểm, bạn chọn mục 'Điểm số' trong menu bên trái. Hệ thống sẽ hiển thị điểm các môn học bạn đã đăng ký."
                });
            }

            if (normalized.Contains("đổi mật khẩu") || normalized.Contains("doi mat khau"))
            {
                return Json(new
                {
                    success = true,
                    reply = "Để đổi mật khẩu, bạn click vào tên tài khoản (góc trên bên phải) → Chọn 'Đổi mật khẩu'."
                });
            }
            // lam on hoi nhung cau sau : Làm sao để đăng ký môn học?
                                        // Em muốn xem điểm
                                        //Đổi mật khẩu ở đâu vậy?

            // GỬI CÂU HỎI QUA MÔ HÌNH AI (nếu không khớp logic định sẵn)
            var requestBody = new
            {
                // Định nghĩa đầu vào cho mô hình AI (prompt)
                inputs = $"Tôi là trợ lý ảo câu trả lời của câu hỏi: {message} là"
            };

            // Chuyển request thành JSON để gửi đi
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            // Gửi POST request đến API Hugging Face
            var response = await _httpClient.PostAsync($"https://api-inference.huggingface.co/models/{model}", content);

            // Kiểm tra lỗi từ phía API
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = $"Lỗi Hugging Face: {response.StatusCode} - {err}" });
            }

            // Đọc phản hồi JSON từ AI
            var responseString = await response.Content.ReadAsStringAsync();

            try
            {
                // Phân tích JSON để lấy đoạn trả lời từ trường `generated_text`
                using var doc = JsonDocument.Parse(responseString);
                var result = doc.RootElement[0].GetProperty("generated_text").GetString();

                // Trả kết quả về cho client
                return Json(new { success = true, reply = result });
            }
            catch
            {
                // Trả lỗi nếu không thể phân tích JSON trả về
                return Json(new { success = false, message = "Lỗi xử lý phản hồi từ mô hình AI!" });
            }
        }
    }
}
