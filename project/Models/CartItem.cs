using System;
using System.Collections.Generic;
using System.Text;

namespace project.Models
{
    public class CartItem
    {
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; } = 1;
        public decimal Subtotal => Product.Price * Quantity;
        public string SubtotalFormatted => $"{Subtotal:N2} ₽";
    }
}
