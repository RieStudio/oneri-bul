namespace OneriBul.Services;

public static class CategoryMapper
{
    public static string GetLocalizedCategory(int id)
    {
        string key = $"Cat_{id}";
        var translations = LanguageService.Instance.Translations;
        
        if (translations.ContainsKey(key))
        {
            return translations[key];
        }

        // Default to 'Other' (Cat_0)
        return translations.ContainsKey("Cat_0") ? translations["Cat_0"] : "Other";
    }

    public static string GetLocalizedCategoryByLabel(string label)
    {
        // Geriye dönük uyumluluk veya hatalı metin gelirse 0 (Diğer) dön.
        return LanguageService.Instance.Translations.ContainsKey("Cat_0") 
            ? LanguageService.Instance.Translations["Cat_0"] 
            : "Other";
    }
}
