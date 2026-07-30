using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OneriBul.Models;
using OneriBul.Services;
using System.Collections.ObjectModel;

namespace OneriBul.ViewModels;

public partial class DiscoverViewModel : ObservableObject
{
    private readonly GeminiService _service;

    [ObservableProperty]
    private ObservableCollection<Mood> _moods = new() { new(), new(), new(), new(), new() };

    [ObservableProperty]
    private ObservableCollection<CategoryItem> _categories = new() { new(), new(), new(), new() };

    [ObservableProperty]
    private CategoryItem? _selectedCategory;

    [ObservableProperty]
    private Mood? _selectedMood;

    [ObservableProperty]
    private Recommendation? _currentRecommendation;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _showRecommendation;

    [ObservableProperty]
    private bool _isButtonEnabled = true;

    [ObservableProperty]
    private double _buttonOpacity = 1.0;

    [ObservableProperty]
    private string _buttonText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<PlatformItem> _platforms = new() 
    { 
        new PlatformItem { Name = "PC" }, 
        new PlatformItem { Name = "Mobil" }, 
        new PlatformItem { Name = "Konsol" } 
    };

    [ObservableProperty]
    private string? _selectedPlatform;

    [ObservableProperty]
    private bool _isPlatformVisible;

    [ObservableProperty]
    private double _platformOpacity;



    public DiscoverViewModel()
    {
        _service = new GeminiService();
        
        // Load data in background to prevent splash screen hang
        Task.Run(async () => 
        {
            try 
            {
                StorageService.Load();
                
                MainThread.BeginInvokeOnMainThread(() => 
                {
                    LoadLocalizedData();
                });
            }
            catch (Exception)
            {
                // Silently fail or initialize with defaults if storage is corrupted
                MainThread.BeginInvokeOnMainThread(() => 
                {
                    LoadLocalizedData();
                });
            }
        });

        LanguageService.Instance.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(LanguageService.CurrentLang))
            {
                LoadLocalizedData();
            }
        };
    }

    private void LoadLocalizedData()
    {
        bool isEn = LanguageService.Instance.CurrentLang == "EN";

        Moods = new ObservableCollection<Mood>
        {
            new Mood { Name = (isEn ? "Energetic" : "Enerjik").Trim(), Emoji = "⚡", Subtext = (isEn ? "ENERGETIC" : "ENERJİK").Trim() },
            new Mood { Name = (isEn ? "Sad" : "Üzgün").Trim(), Emoji = "☁️", Subtext = (isEn ? "SAD" : "ÜZGÜN").Trim() },
            new Mood { Name = (isEn ? "Calm" : "Sakin").Trim(), Emoji = "🌿", Subtext = (isEn ? "CALM" : "SAKİN").Trim() },
            new Mood { Name = (isEn ? "Bored" : "Sıkılmış").Trim(), Emoji = "☕", Subtext = (isEn ? "BORED" : "SIKILMIŞ").Trim() },
            new Mood { Name = (isEn ? "Thoughtful" : "Düşünceli").Trim(), Emoji = "⚙️", Subtext = (isEn ? "THOUGHTFUL" : "DÜŞÜNCELİ").Trim() }
        };

        Categories = new ObservableCollection<CategoryItem> 
        { 
            new CategoryItem { Name = (isEn ? "Movies" : "Filmler").Trim() }, 
            new CategoryItem { Name = (isEn ? "Series" : "Diziler").Trim() }, 
            new CategoryItem { Name = (isEn ? "Games" : "Oyunlar").Trim() }, 
            new CategoryItem { Name = (isEn ? "Books" : "Kitaplar").Trim() } 
        };

        Platforms = new ObservableCollection<PlatformItem>
        {
            new PlatformItem { Name = "PC" },
            new PlatformItem { Name = isEn ? "Mobile" : "Mobil" },
            new PlatformItem { Name = isEn ? "Console" : "Konsol" }
        };
        
        ButtonText = LanguageService.Instance.Translations["FindBtn"];
        SelectedMood = null;
        SelectedCategory = null;
    }

    [RelayCommand]
    private void SelectMood(Mood mood)
    {
        SelectedMood = mood;
        foreach (var m in Moods)
        {
            m.IsSelected = false;
        }
        mood.IsSelected = true;
    }

    [RelayCommand]
    private async Task SelectCategory(CategoryItem category)
    {
        SelectedCategory = category;
        foreach (var c in Categories)
        {
            c.IsSelected = false;
        }
        category.IsSelected = true;

        bool shouldShowPlatform = category.Name == "Games" || category.Name == "Oyunlar";
        if (shouldShowPlatform)
        {
            IsPlatformVisible = true;
            // Opacity animation trigger - we'll handle actual animation in XAML or code-behind
            PlatformOpacity = 1.0;
        }
        else
        {
            PlatformOpacity = 0.0;
            IsPlatformVisible = false;
            SelectedPlatform = null;
            foreach (var p in Platforms) p.IsSelected = false;
        }
    }

    [RelayCommand]
    private void SelectPlatform(PlatformItem platform)
    {
        SelectedPlatform = platform.Name;
        foreach (var p in Platforms)
        {
            p.IsSelected = false;
        }
        platform.IsSelected = true;
    }

    [RelayCommand]
    private async Task FindRecommendation()
    {
        if (SelectedMood == null || SelectedCategory == null || !IsButtonEnabled) return;

        bool shouldReEnable = true;
        IsLoading = true;
        ShowRecommendation = false;
        IsButtonEnabled = false;
        ButtonOpacity = 0.5;
        try
        {
            var blacklist = StorageService.GetRecentTitles(72);
            var recommendations = await _service.GetRecommendationsAsync(SelectedMood.Name, SelectedCategory.Name, blacklist, SelectedPlatform);
            if (recommendations != null && recommendations.Count > 0)
            {
                var tmdbService = new TmdbService();
                var gameService = new GameImageService();
                var bookService = new BookImageService();

                foreach (var rec in recommendations)
                {
                    // Category Mapping: ID'yi yerelleştirilmiş metne dönüştür
                    rec.Category = CategoryMapper.GetLocalizedCategory(rec.CategoryId);
                    
                    // Genre temizliği
                    rec.Genre = (rec.Genre ?? "").Trim().TrimEnd(',');
                    
                    // Üst kategori kontrolü için orijinal veya eşlenmiş ismi kullan
                    string catLower = rec.Category.ToLower();
                    string titleLower = rec.Title.ToLower();

                    if (catLower.Contains("game") || catLower.Contains("oyun") || SelectedCategory?.Name.ToLower().Contains("oyun") == true)
                    {
                        var details = await gameService.GetDetailsAsync(rec.Title);
                        rec.ImageUrl = details.Url;
                        rec.Rating = details.Rating;
                    }
                    else if (catLower.Contains("book") || catLower.Contains("kitap") || SelectedCategory?.Name.ToLower().Contains("kitap") == true)
                    {
                        var details = await bookService.GetDetailsAsync(rec.Title);
                        rec.ImageUrl = details.Url;
                        rec.Rating = details.Rating;
                    }
                    else
                    {
                        // Movie/Series
                        var details = await tmdbService.GetDetailsAsync(rec.Title);
                        rec.ImageUrl = details.Url;
                        rec.Rating = details.Rating;
                    }
                }

                // Ekrandaki şablona uygun olarak ilk gelen öneriyi gösteriyoruz:
                CurrentRecommendation = recommendations.FirstOrDefault();
                
                if (CurrentRecommendation != null)
                {
                    // Add to History automatically
                    StorageService.AddToHistory(CurrentRecommendation);
                }

                ShowRecommendation = true;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Bilgi", "Seçtiğiniz kriterlere uygun güncel bir öneri şu anda kayıtlarımızda bulunmamaktadır. Lütfen farklı tercihlerle tekrar deneyiniz.", "Tamam");
            }
        }
        catch (HttpRequestException httpEx)
        {
            bool isEn = LanguageService.Instance.CurrentLang == "EN";
            string title = isEn ? "Service Status" : "Hizmet Durumu";
            string message = string.Empty;

            if (httpEx.Message.Contains("RATE_LIMIT_EXCEEDED"))
            {
                message = isEn 
                    ? "Daily transaction limit reached. To maintain service quality, your request cannot be fulfilled at this time." 
                    : "Günlük işlem limitine ulaşıldı. Hizmet kalitesini korumak adına talebiniz şu an gerçekleştirilemiyor.";
            }
            else if (httpEx.Message.Contains("SERVICE_UNAVAILABLE"))
            {
                message = isEn 
                    ? "Due to system congestion, recommendations cannot be generated at this time. Please try again shortly." 
                    : "Sistem yoğunluğu nedeniyle şu anda öneri oluşturulamıyor. Lütfen kısa bir süre sonra tekrar deneyiniz.";
            }
            else
            {
                message = httpEx.Message;
            }

            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, "OK");
            }
            
            // Disable button and change text
            ButtonText = isEn ? "Service Temporarily Unavailable" : "Hizmet Geçici Olarak Kapalı";
            shouldReEnable = false;
            return; // Exit here to prevent finally block from re-enabling 
        }
        catch (Exception ex)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }
        finally
        {
            IsLoading = false;
            
            // Cooldown logic
            bool isEn = LanguageService.Instance.CurrentLang == "EN";
            for (int i = 6; i > 0; i--)
            {
                ButtonText = isEn ? $"Wait ({i}s)..." : $"Bekleyin ({i}s)...";
                await Task.Delay(1000);
            }
            
            if (shouldReEnable) 
            {
                ButtonText = LanguageService.Instance.Translations["FindBtn"];
                ButtonOpacity = 1.0;
                IsButtonEnabled = true;
            }
        }
    }

    [RelayCommand]
    private void SaveRecommendation()
    {
        if (CurrentRecommendation != null)
        {
            CurrentRecommendation.IsSaved = !CurrentRecommendation.IsSaved;
            StorageService.ToggleSave(CurrentRecommendation);
        }
    }
}
