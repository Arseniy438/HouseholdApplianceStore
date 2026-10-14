using System.IO;

namespace project.Tools;

public static class ImageHelper
{
    // Единая базовая папка для всех операций
    private static string BaseDir => AppContext.BaseDirectory;
    private static string ImagesDir => Path.Combine(BaseDir, "Images");

    public static string SaveImage(string sourceFilePath, int productId)
    {
        if (!Directory.Exists(ImagesDir))
            Directory.CreateDirectory(ImagesDir);

        var extension = Path.GetExtension(sourceFilePath).ToLower();
        if (string.IsNullOrEmpty(extension))
            extension = ".jpg";

        var fileName = $"product_{productId}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
        var destFullPath = Path.Combine(ImagesDir, fileName);

        File.Copy(sourceFilePath, destFullPath, overwrite: true);

        // Возвращаем относительный путь: "Images/product_1_20250101.jpg"
        return Path.Combine("Images", fileName);
    }

    public static void DeleteImage(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return;

        var fullPath = Path.Combine(BaseDir, relativePath);

        if (File.Exists(fullPath) && !fullPath.Contains("default.png"))
        {
            try { File.Delete(fullPath); }
            catch { }
        }
    }

    public static bool IsImageFile(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLower();
        return ext is ".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif" or ".webp";
    }

    public static string ImageFilter =>
        "Изображения (*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp)|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp|Все файлы (*.*)|*.*";

    // Диагностика: проверить существует ли файл по сохранённому пути
    public static bool FileExists(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return false;
        return File.Exists(Path.Combine(BaseDir, relativePath));
    }

    public static string GetFullPath(string relativePath)
    {
        return Path.Combine(BaseDir, relativePath);
    }
}