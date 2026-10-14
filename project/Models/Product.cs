using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace project.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Manufacturer { get; set; } = string.Empty;

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int Quantity { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Specifications { get; set; } = string.Empty;

    // Путь к изображению (относительный, например "Images/product_1.jpg")
    [MaxLength(300)]
    public string ImagePath { get; set; } = string.Empty;

    [NotMapped]
    public string AvailabilityStatus => Quantity > 0 ? "В наличии" : "Нет в наличии";

    [NotMapped]
    public string PriceFormatted => $"{Price:N2} ₽";

    // Полный путь к изображению для отображения в WPF
    [NotMapped]
    public string FullImagePath
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(ImagePath))
            {
                var full = Path.Combine(AppContext.BaseDirectory, ImagePath);
                if (File.Exists(full))
                    return full;
            }

            return Path.Combine(AppContext.BaseDirectory, "Images", "default.png");
        }
    }
}