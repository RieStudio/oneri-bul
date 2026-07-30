using CommunityToolkit.Mvvm.ComponentModel;

namespace OneriBul.Models;

public partial class Mood : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public string Subtext { get; set; } = string.Empty;

    [ObservableProperty]
    private bool _isSelected;
}

public partial class CategoryItem : ObservableObject
{
    public string Name { get; set; } = string.Empty;

    [ObservableProperty]
    private bool _isSelected;
}

public partial class Recommendation : ObservableObject
{
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _category = string.Empty;

    [ObservableProperty]
    private int _categoryId;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _imageUrl = string.Empty;

    [ObservableProperty]
    private string _year = string.Empty;

    [ObservableProperty]
    private string _genre = string.Empty;

    [ObservableProperty]
    private double _rating;
    
    [ObservableProperty]
    private bool _isSaved;

    public DateTime Date { get; set; }
}
