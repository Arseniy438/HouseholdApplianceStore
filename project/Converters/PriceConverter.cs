using System.Globalization;
using System.Windows.Data;

namespace project.Converters;

public class PriceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal price)
            return $"{price:N2} ₽";
        return "0,00 ₽";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}