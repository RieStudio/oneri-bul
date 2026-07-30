using System.Text;
using System.Text.Json;
using System.Diagnostics;
using OneriBul.Models;

namespace OneriBul.Services;

public class GeminiService
{
    private readonly HttpClient _httpClient;

    public GeminiService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<Recommendation>> GetRecommendationsAsync(string mood, string category, string blacklist = "", string? platform = null)
    {
        if (string.IsNullOrEmpty(Config.GeminiApiKey) || Config.GeminiApiKey == "YOUR_GEMINI_API_KEY_HERE")
        {
            throw new Exception("API Anahtarı eksik! Lütfen Config.cs dosyasına geçerli bir Gemini API anahtarı ekleyin.");
        }

        string lang = LanguageService.Instance.CurrentLang;
        string blacklistCmd = !string.IsNullOrEmpty(blacklist) 
            ? $" Daha önce şu içerikleri önerdin, lütfen bunları tekrar önerme: {blacklist}." 
            : "";
            
        // Token Optimizasyonu: Sadece Kategori 'Oyunlar' ise ve platform seçiliyse ekle
        string platformCmd = "";
        bool isGaming = category.Equals("Games", StringComparison.OrdinalIgnoreCase) || category.Equals("Oyunlar", StringComparison.OrdinalIgnoreCase);
        
        if (isGaming && !string.IsNullOrEmpty(platform))
        {
            platformCmd = lang == "EN" 
                ? $", only suggest for the {platform} platform." 
                : $", sadece {platform} platformu için öneri yap.";
        }

        string prompt = lang == "EN"
            ? $"Lütfen önerinin 'Description' kısmını (hikayesini) İNGİLİZCE dilinde döndür. 'CategoryId' alanını mutlaka şu listeden bir tam sayı (integer) olarak döndür: 1: Action, 2: Drama, 3: Comedy, 4: Sci-Fi, 5: Horror, 6: Documentary, 7: Animation, 8: Romance, 9: Thriller, 0: Other.{blacklistCmd}{platformCmd} You are an entertainment assistant. User mood is '{mood}' and category is '{category}'. Provide 3 JSON recommendations (Title, CategoryId, Description, Year, ImageUrl, Genre). All values must be strings except CategoryId. Output must be a pure JSON array."
            : $"Lütfen önerinin 'Description' kısmını (hikayesini) TÜRKÇE dilinde döndür. 'CategoryId' alanını mutlaka şu listeden bir tam sayı (integer) olarak döndür: 1: Aksiyon, 2: Dram, 3: Komedi, 4: Bilim Kurgu, 5: Korku, 6: Belgesel, 7: Animasyon, 8: Romantik, 9: Gerilim, 0: Diğer.{blacklistCmd}{platformCmd} Eğlence asistanısın. Kullanıcı ruh hali: '{mood}', kategori: '{category}'. 3 JSON önerisi ver (Title, CategoryId, Description, Year, ImageUrl, Genre). Tüm değerler (Year dahil) string (metin) olmalı, sadece CategoryId integer (sayı) olmalı. Sadece JSON array olmalı.";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.7,
                responseMimeType = "application/json"
            }
        };

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={Config.GeminiApiKey}";

        int maxRetries = 2;
        HttpResponseMessage response = null;

        for (int retry = 0; retry < maxRetries; retry++)
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var content = new StringContent(JsonSerializer.Serialize(requestBody, jsonOptions), Encoding.UTF8, "application/json");
            response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                break;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                throw new HttpRequestException("RATE_LIMIT_EXCEEDED");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                if (retry < maxRetries - 1)
                {
                    await Task.Delay(3000); // 3 seconds wait
                    continue;
                }
                else
                {
                    throw new HttpRequestException("SERVICE_UNAVAILABLE");
                }
            }
            
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode}\n{errorResponse}");
            }
        }

        if (response?.Content == null) throw new Exception("API yanıtı boş döndü.");
        var jsonResponse = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(jsonResponse);
        
        var candidates = jsonDoc.RootElement.GetProperty("candidates");
        if (candidates.GetArrayLength() > 0)
        {
            var textObj = candidates[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

            if (!string.IsNullOrEmpty(textObj))
            {
                var jsonText = textObj.Trim();
                
                int startIndexArray = jsonText.IndexOf('[');
                int endIndexArray = jsonText.LastIndexOf(']');
                int startIndexObj = jsonText.IndexOf('{');
                int endIndexObj = jsonText.LastIndexOf('}');

                if (startIndexArray != -1 && endIndexArray != -1 && endIndexArray > startIndexArray)
                {
                    jsonText = jsonText.Substring(startIndexArray, endIndexArray - startIndexArray + 1);
                }
                else if (startIndexObj != -1 && endIndexObj != -1 && endIndexObj > startIndexObj)
                {
                    jsonText = jsonText.Substring(startIndexObj, endIndexObj - startIndexObj + 1);
                    jsonText = $"[{jsonText}]"; // Objeyi bir Dizi içerisine sarmalıyoruz
                }

                var recommendations = JsonSerializer.Deserialize<List<Recommendation>>(jsonText, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                return recommendations ?? new List<Recommendation>();
            }
        }
        
        throw new Exception("Sunucu yanıtı beklenen formatta değil. Lütfen kısa bir süre sonra tekrar deneyiniz.");
    }
}
