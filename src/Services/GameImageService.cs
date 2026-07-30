using System.Text.Json;

namespace OneriBul.Services;

public class GameImageService
{
    private static readonly HttpClient _httpClient = new();
    private const string BaseUrl = "https://api.rawg.io/api/games";
    private const string PlaceholderImage = "https://via.placeholder.com/500x750/111111/5E2BFF?text=Oyun+Gorseli+Yok";

    public async Task<(string Url, double Rating)> GetDetailsAsync(string gameName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(gameName)) return (PlaceholderImage, 0);

            string escapedName = Uri.EscapeDataString(gameName);
            string requestUrl = $"{BaseUrl}?key={Config.RawgApiKey}&search={escapedName}&page_size=1";

            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode) return (PlaceholderImage, 0);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

            if (result.TryGetProperty("results", out var results) && results.GetArrayLength() > 0)
            {
                var firstGame = results[0];
                double rating = 0;
                if (firstGame.TryGetProperty("rating", out var ratingElement))
                {
                    rating = ratingElement.GetDouble();
                }

                if (firstGame.TryGetProperty("background_image", out var imageUrlElement) && imageUrlElement.ValueKind != JsonValueKind.Null)
                {
                    return (imageUrlElement.GetString() ?? PlaceholderImage, rating);
                }
            }
        }
        catch
        {
            // Fallback
        }

        return (PlaceholderImage, 0);
    }
}
