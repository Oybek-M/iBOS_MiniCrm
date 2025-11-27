using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace SmartCrm.Service.Helpers
{
    public class TelegramBotHelper
    {
        private readonly string _botToken;
        private readonly string _chatId;
        private readonly HttpClient _httpClient;

        public TelegramBotHelper(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _botToken = configuration.GetValue<string>("Telegram:BotToken")
                        ?? throw new ArgumentNullException("Telegram:BotToken");
            _chatId = configuration.GetValue<string>("Telegram:SuperAdminChatId")
                      ?? throw new ArgumentNullException("Telegram:SuperAdminChatId");
        }

        
        public async Task SendBackupFileAsync(string filePath, string caption)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Fayl topilmadi: {filePath}");

            var url = $"https://api.telegram.org/bot{_botToken}/sendDocument";

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(_chatId), "chat_id");

            // Faylni o‘qib, multipart form data sifatida qo‘shamiz
            var fileStream = File.OpenRead(filePath);
            var fileName = Path.GetFileName(filePath);

            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            content.Add(fileContent, "document", fileName);
            if (!string.IsNullOrWhiteSpace(caption))
                content.Add(new StringContent(caption), "caption");

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }
    }
}
