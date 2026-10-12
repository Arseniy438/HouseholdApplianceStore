using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project.Models;

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [Required]
    public DateTime OrderDate { get; set; } = DateTime.Now;

    [Required]
    public decimal TotalPrice { get; set; }

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "Новый";

    public List<OrderItem> Items { get; set; } = new();
}