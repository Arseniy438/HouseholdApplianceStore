using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    [NotMapped]
    public string AvailabilityStatus => Quantity > 0 ? "В наличии" : "Нет в наличии";

    [NotMapped]
    public string PriceFormatted => $"{Price:N2} ₽";
}