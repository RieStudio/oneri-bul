using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OneriBul.Models;
using OneriBul.Services;
using System.Collections.ObjectModel;

namespace OneriBul.ViewModels;

public partial class SavedViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Recommendation> _savedItems;

    public SavedViewModel()
    {
        Refresh();
    }

    public void Refresh()
    {
        StorageService.Load(); 
        SavedItems = StorageService.Saved;
    }
}

public class HistoryGroup : ObservableCollection<Recommendation>
{
    public string Name { get; private set; }
    public HistoryGroup(string name, IEnumerable<Recommendation> items) : base(items)
    {
        Name = name;
    }
}

public partial class HistoryViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<HistoryGroup> _groupedHistory = new();

    public HistoryViewModel()
    {
        Refresh();
    }

    [RelayCommand]
    public void Refresh()
    {
        StorageService.Load();
        var allItems = StorageService.History.OrderByDescending(x => x.Date).ToList();
        
        var groups = allItems.GroupBy(x => x.Date.Date)
            .Select(g => 
            {
                string header;
                if (g.Key == DateTime.Today)
                    header = LanguageService.Instance.Translations.ContainsKey("Today") ? LanguageService.Instance.Translations["Today"] : "Bugün";
                else if (g.Key == DateTime.Today.AddDays(-1))
                    header = LanguageService.Instance.CurrentLang == "EN" ? "Yesterday" : "Dün";
                else
                    header = g.Key.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo(LanguageService.Instance.CurrentLang == "EN" ? "en-US" : "tr-TR"));

                return new HistoryGroup(header, g);
            });

        GroupedHistory = new ObservableCollection<HistoryGroup>(groups);
    }
}
