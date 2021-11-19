using System.Windows.Media;

namespace WPF;

public class AStarSetToColorValueConverter : BaseValueConverter<AStarSetToColorValueConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            Enum.AStarSet.Start => new SolidColorBrush(Colors.LightGreen),
            Enum.AStarSet.End => new SolidColorBrush(Colors.Blue),
            Enum.AStarSet.Closed => new SolidColorBrush(Colors.LightPink),
            Enum.AStarSet.Open => new SolidColorBrush(Colors.Yellow),
            Enum.AStarSet.Wall => new SolidColorBrush(Colors.Black),
            Enum.AStarSet.Road => new SolidColorBrush(Colors.DarkGray),
            Enum.AStarSet.Path => new SolidColorBrush(Colors.LightBlue),
            Enum.AStarSet.Undefined => new SolidColorBrush(Colors.LightGray),

            _ => new SolidColorBrush(Colors.White),
        };
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
