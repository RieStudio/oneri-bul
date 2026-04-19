using System.Text.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using OneriBul.Models;

namespace OneriBul.Services;

public static class StorageService
{
    private static readonly string SavedFilePath = Path.Combine(FileSystem.AppDataDirectory, "saved.json");
    private static readonly string HistoryFilePath = Path.Combine(FileSystem.AppDataDirectory, "history.json");

    public static ObservableCollection<Recommendation> Saved { get; private set; } = new();
    public static ObservableCollection<Recommendation> History { get; private set; } = new();

    public static void Load()
    {
        try 
        {
            if (File.Exists(SavedFilePath))
            {
                var savedJson = File.ReadAllText(SavedFilePath);
                var savedList = JsonSerializer.Deserialize<List<Recommendation>>(savedJson) ?? new List<Recommendation>();
                Saved.Clear();
                foreach (var item in savedList) Saved.Add(item);
            }

            if (File.Exists(HistoryFilePath))
            {
                var historyJson = File.ReadAllText(HistoryFilePath);
                var historyList = JsonSerializer.Deserialize<List<Recommendation>>(historyJson) ?? new List<Recommendation>();
                History.Clear();
                foreach (var item in historyList) History.Add(item);
            }
        }
        catch (Exception ex)
        {
            // Simple failure handling
        }
    }

    public static string GetRecentTitles(int hours)
    {
        var limitDate = DateTime.Now.AddHours(-hours);
        var titles = History
            .Where(x => x.Date >= limitDate)
            .Take(15) // Limit to last 15 items for token optimization
            .Select(x => x.Title)
            .Distinct();
            
        return string.Join(", ", titles);
    }

    public static void AddToHistory(Recommendation item)
    {
        item.Date = DateTime.Now;
        History.Insert(0, item);
        SaveHistory();
    }

    public static void ToggleSave(Recommendation item)
    {
        if (item.IsSaved)
        {
            var existing = Saved.FirstOrDefault(x => x.Title == item.Title && x.Category == item.Category);
            if (existing == null)
            {
                Saved.Insert(0, item);
            }
        }
        else
        {
            var existing = Saved.FirstOrDefault(x => x.Title == item.Title && x.Category == item.Category);
            if (existing != null)
            {
                Saved.Remove(existing);
            }
            
            var historyItem = History.FirstOrDefault(x => x.Title == item.Title && x.Category == item.Category);
            if (historyItem != null)
            {
                historyItem.IsSaved = false;
            }
        }
        SaveSaved();
        SaveHistory();
    }

    private static void SaveHistory()
    {
        try 
        {
            var json = JsonSerializer.Serialize(History.ToList());
            File.WriteAllText(HistoryFilePath, json);
        }
        catch (Exception) { }
    }

    private static void SaveSaved()
    {
        try 
        {
            var json = JsonSerializer.Serialize(Saved.ToList());
            File.WriteAllText(SavedFilePath, json);
        }
        catch (Exception) { }
    }
}
