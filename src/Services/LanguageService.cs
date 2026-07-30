using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OneriBul.Services;

public class LanguageService : INotifyPropertyChanged
{
    private static LanguageService? _instance;
    public static LanguageService Instance => _instance ??= new LanguageService();

    public string CurrentLang
    {
        get => Preferences.Get("AppLang", "TR");
        set
        {
            Preferences.Set("AppLang", value);
            OnPropertyChanged(nameof(CurrentLang));
            OnPropertyChanged(nameof(OtherLang));
            OnPropertyChanged(nameof(Translations));
        }
    }

    public string OtherLang => CurrentLang == "TR" ? "EN" : "TR";

    public System.Windows.Input.ICommand ToggleLangCommand { get; }
    public System.Windows.Input.ICommand ToggleMenuCommand { get; }
    public System.Windows.Input.ICommand SelectLangCommand { get; }

    private bool _isLanguageMenuOpen;
    public bool IsLanguageMenuOpen
    {
        get => _isLanguageMenuOpen;
        set
        {
            _isLanguageMenuOpen = value;
            OnPropertyChanged(nameof(IsLanguageMenuOpen));
        }
    }

    private LanguageService()
    {
        ToggleLangCommand = new Command(() => {
            CurrentLang = OtherLang;
        });

        ToggleMenuCommand = new Command(() => {
            IsLanguageMenuOpen = !IsLanguageMenuOpen;
        });

        SelectLangCommand = new Command<string>((lang) => {
            if (!string.IsNullOrEmpty(lang))
            {
                CurrentLang = lang;
            }
            IsLanguageMenuOpen = false;
        });
    }

    public Dictionary<string, string> Translations => GetTranslations(CurrentLang);

    private Dictionary<string, string> GetTranslations(string lang)
    {
        if (lang == "EN")
        {
            return new Dictionary<string, string>
            {
                { "DiscoverTab", "Discover" },
                { "HistoryTab", "History" },
                { "SavedTab", "Saved" },
                { "DiscoverTitle", "How do you feel\ntoday?" },
                { "FindBtn", "✨ Find Recommendation" },
                { "Why", "WHY THIS RECOMMENDATION?" },
                { "SaveBtn", "🔖 Save" },
                { "Today", "TODAY" },
                { "SavedTitle", "Saved Items" },
                { "HistoryTitle", "History" },
                { "Cat_1", "Action" },
                { "Cat_2", "Drama" },
                { "Cat_3", "Comedy" },
                { "Cat_4", "Sci-Fi" },
                { "Cat_5", "Horror" },
                { "Cat_6", "Documentary" },
                { "Cat_7", "Animation" },
                { "Cat_8", "Romance" },
                { "Cat_9", "Thriller" },
                { "Cat_0", "Other" }
            };
        }
        else
        {
            return new Dictionary<string, string>
            {
                { "DiscoverTab", "Keşfet" },
                { "HistoryTab", "Geçmiş" },
                { "SavedTab", "Kaydedilenler" },
                { "DiscoverTitle", "Bugün nasıl\nhissediyorsun?" },
                { "FindBtn", "✨ Bana Öneri Bul" },
                { "Why", "NEDEN BU ÖNERİ?" },
                { "SaveBtn", "🔖 Kaydet" },
                { "Today", "BUGÜN" },
                { "SavedTitle", "Kaydedilenler" },
                { "HistoryTitle", "Geçmiş" },
                { "Cat_1", "Aksiyon" },
                { "Cat_2", "Dram" },
                { "Cat_3", "Komedi" },
                { "Cat_4", "Bilim Kurgu" },
                { "Cat_5", "Korku" },
                { "Cat_6", "Belgesel" },
                { "Cat_7", "Animasyon" },
                { "Cat_8", "Romantik" },
                { "Cat_9", "Gerilim" },
                { "Cat_0", "Diğer" }
            };
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
