using System.Text.Json;

namespace OneriBul.Services;

public class TmdbService
{
    private static readonly HttpClient _httpClient = new();
    private const string BaseUrl = "https://api.themoviedb.org/3";
    private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
    private const string PlaceholderImage = "https://via.placeholder.com/500x750/111111/5E2BFF?text=Gorsel+Bulunamadi";

    public async Task<(string Url, double Rating)> GetDetailsAsync(string title)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(title)) return (PlaceholderImage, 0);

            string escapedTitle = Uri.EscapeDataString(title);
            string requestUrl = $"{BaseUrl}/search/multi?api_key={Config.TmdbApiKey}&query={escapedTitle}&language=tr-TR&page=1&include_adult=false";

            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode) return (PlaceholderImage, 0);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

            if (result.TryGetProperty("results", out var results) && results.GetArrayLength() > 0)
            {
                foreach(var item in results.EnumerateArray())
                {
                    double rating = 0;
                    if (item.TryGetProperty("vote_average", out var ratingElement))
                    {
                        rating = ratingElement.GetDouble();
                    }

                    if (item.TryGetProperty("poster_path", out var posterPathElement) && posterPathElement.ValueKind != JsonValueKind.Null)
                    {
                        string? posterPath = posterPathElement.GetString();
                        if (!string.IsNullOrWhiteSpace(posterPath))
                        {
                            return (ImageBaseUrl + posterPath, rating);
                        }
                    }
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
