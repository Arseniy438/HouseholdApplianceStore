using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace project.Converters;

public class ImagePathConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string? path = value as string;

        // Определяем базовую папку
        var baseDir = AppContext.BaseDirectory;

        // Если путь пустой — загружаем default
        if (string.IsNullOrWhiteSpace(path))
            return LoadFromFile(Path.Combine(baseDir, "Images", "default.png"));

        // Если путь относительный — делаем абсолютным
        if (!Path.IsPathRooted(path))
            path = Path.Combine(baseDir, path);

        // Если файл существует — загружаем его
        if (File.Exists(path))
            return LoadFromFile(path);

        // Иначе — default
        return LoadFromFile(Path.Combine(baseDir, "Images", "default.png"));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static BitmapImage? LoadFromFile(string fullPath)
    {
        if (!File.Exists(fullPath))
            return null;

        try
        {
            // Читаем файл в память, чтобы не блокировать его на диске
            var bytes = File.ReadAllBytes(fullPath);
            var stream = new MemoryStream(bytes);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }
        catch
        {
            return null;
        }
    }
}