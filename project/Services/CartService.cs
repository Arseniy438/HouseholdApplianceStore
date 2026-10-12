using project.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace project.Services
{
    public class CartService
    {
        private readonly List<CartItem> _items = new();

        public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

        public decimal TotalPrice => _items.Sum(i => i.Subtotal);

        public string TotalPriceFormatted => $"{TotalPrice:N2} ₽";

        public int ItemCount => _items.Sum(i => i.Quantity);

        public void AddProduct(Product product, int quantity = 1)
        {
            var existing = _items.FirstOrDefault(i => i.Product.Id == product.Id);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                _items.Add(new CartItem { Product = product, Quantity = quantity });
            }
        }

        public void RemoveProduct(int productId)
        {
            var item = _items.FirstOrDefault(i => i.Product.Id == productId);
            if (item != null)
                _items.Remove(item);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = _items.FirstOrDefault(i => i.Product.Id == productId);
            if (item != null)
            {
                if (quantity <= 0)
                    _items.Remove(item);
                else
                    item.Quantity = quantity;
            }
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}
