using System.Globalization;

namespace OneriBul.Converters;

public class SelectionToColorConverter : IValueConverter
{
    public Color SelectedColor { get; set; } = Color.FromArgb("#8A2BE2");
    public Color UnselectedColor { get; set; } = Color.FromArgb("#1E1E24");

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null) return UnselectedColor;

        if (value.Equals(parameter))
        {
            return SelectedColor;
        }
        
        return UnselectedColor;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
