using System.Globalization;
using OneriBul.Models;

namespace OneriBul.Converters;

public class SelectedMoodToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Mood selectedMood && parameter is Mood currentMood)
        {
            if (selectedMood == currentMood)
            {
                // Selected color (purple in image)
                return Color.FromArgb("#8A2BE2");
            }
        }
        
        // Default color (dark surface)
        return Color.FromArgb("#1E1E24");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
