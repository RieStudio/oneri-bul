namespace OneriBul.Services;
using System.Text.Json;
using System.Diagnostics;

public class BookImageService
{
    private static readonly HttpClient _httpClient = new();
    private const string BaseUrl = "https://www.googleapis.com/books/v1/volumes";
    // Placeholder linkini de güncelledim, bazen placeholder servisleri de takılabiliyor
    private const string PlaceholderImage = "https://raw.githubusercontent.com/RieStudio/Assets/main/no-cover.png"; 

    public async Task<(string Url, double Rating)> GetDetailsAsync(string bookTitle)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(bookTitle)) return (PlaceholderImage, 0);

            string cleanedTitle = bookTitle.Trim('\"', '\'', ' ', '.', ':', '-');
            // Parantez içindeki (Kitap) gibi ekleri temizle
            int bracketIndex = cleanedTitle.IndexOf('(');
            if (bracketIndex > 0) cleanedTitle = cleanedTitle.Substring(0, bracketIndex).Trim();

            string escapedTitle = Uri.EscapeDataString(cleanedTitle);
            string requestUrl = $"{BaseUrl}?q={escapedTitle}&maxResults=1&printType=books";

            Debug.WriteLine($"[RieStudio Log] Kitap aranıyor: {cleanedTitle}");

            var response = await _httpClient.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

                if (result.TryGetProperty("items", out var items) && items.GetArrayLength() > 0)
                {
                    var volumeInfo = items[0].GetProperty("volumeInfo");
                    
                    double rating = 0;
                    if (volumeInfo.TryGetProperty("averageRating", out var r)) rating = r.GetDouble();

                    if (volumeInfo.TryGetProperty("imageLinks", out var imageLinks))
                    {
                        string? url = null;
                        if (imageLinks.TryGetProperty("thumbnail", out var t)) url = t.GetString();
                        else if (imageLinks.TryGetProperty("smallThumbnail", out var st)) url = st.GetString();

                        if (!string.IsNullOrEmpty(url))
                        {
                            // Google'ın o meşhur http linkini jilet gibi https yapıyoruz
                            url = url.Replace("http://", "https://").Replace("&edge=curl", "");
                            
                            Debug.WriteLine($"[RieStudio Log] Görsel Bulundu: {url}");
                            return (url, rating);
                        }
                    }
                }
            }
            
            Debug.WriteLine("[RieStudio Log] Görsel bulunamadı, placeholder dönülüyor.");
            return (PlaceholderImage, 0);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[RieStudio Log] KRİTİK HATA: {ex.Message}");
            return (PlaceholderImage, 0);
        }
    }
}