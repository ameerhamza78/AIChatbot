using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AIChatbotPro.Models;

namespace AIChatbotPro.Services
{
    public class AIService
    {
        private readonly HttpClient client = new HttpClient();
        private const string API_KEY = "YOUR_API_KEY_HERE";

        public async Task<AIResponse> GetResponse(string prompt)
        {
            try
            {
                var url = "https://api.groq.com/openai/v1/chat/completions";

                var requestData = new
                {
                    model = "llama-3.1-8b-instant",  // ✅ Updated model
                    messages = new[]
     {
        new { role = "user", content = prompt }
    }
                };

                var json = JsonConvert.SerializeObject(requestData);

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Authorization", $"Bearer {API_KEY}");
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();

                dynamic data = JsonConvert.DeserializeObject(result);

                // Handle API errors
                if (data.choices == null)
                {
                    return new AIResponse
                    {
                        Role = "Error",
                        Content = "⚠ API Error: " + result
                    };
                }

                return new AIResponse
                {
                    Role = "AI",
                    Content = data.choices[0].message.content.ToString()
                };
            }
            catch (Exception ex)
            {
                return new AIResponse
                {
                    Role = "Error",
                    Content = "⚠ Exception: " + ex.Message
                };
            }
        }
    }
}