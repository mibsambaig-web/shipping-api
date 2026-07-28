using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shipping_api.Data;
using System.Text;
using System.Text.Json;

namespace shipping_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _http;

        public AIController(AppDbContext db)
        {
            _db = db;
            _http = new HttpClient();
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            var shipments = _db.Shipments.ToList();
            var shipmentJson = JsonSerializer.Serialize(shipments);

            var body = new
            {
                model = "openrouter/auto",
                messages = new[]
                {
                    new { role = "system", content = $"You are a shipping assistant for Swift Cargo. Answer questions based ONLY on this data: {shipmentJson}. Be concise, one or two lines max." },
                    new { role = "user", content = request.Message }
                }
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {Environment.GetEnvironmentVariable("OPENROUTER_API_KEY")}");
            var response = await _http.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("OpenRouter response: " + responseBody);

            var parsed = JsonDocument.Parse(responseBody);
            var reply = parsed.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return Ok(new { reply });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}