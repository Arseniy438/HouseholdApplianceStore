using project.Data;
using project.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace project.Services
{
    public class ProductService
    {
        public List<Product> GetAllProducts()
        {
            using var context = new AppDbContext();
            return context.Products.ToList();
        }

        public Product? GetProductById(int id)
        {
            using var context = new AppDbContext();
            return context.Products.Find(id);
        }

        public (bool Success, string ErrorMessage) AddProduct(Product product)
        {
            var validation = ValidateProduct(product);
            if (!validation.Success)
                return validation;

            using var context = new AppDbContext();
            context.Products.Add(product);
            context.SaveChanges();
            return (true, string.Empty);
        }

        public (bool Success, string ErrorMessage) UpdateProduct(Product product)
        {
            var validation = ValidateProduct(product);
            if (!validation.Success)
                return validation;

            using var context = new AppDbContext();
            var existing = context.Products.Find(product.Id);
            if (existing == null)
                return (false, "Товар не найден в базе данных");

            existing.Name = product.Name;
            existing.Category = product.Category;
            existing.Manufacturer = product.Manufacturer;
            existing.Price = product.Price;
            existing.Quantity = product.Quantity;
            existing.Description = product.Description;
            existing.Specifications = product.Specifications;

            context.SaveChanges();
            return (true, string.Empty);
        }

        public (bool Success, string ErrorMessage) DeleteProduct(int id)
        {
            using var context = new AppDbContext();
            var product = context.Products.Find(id);
            if (product == null)
                return (false, "Товар не найден в базе данных");

            context.Products.Remove(product);
            context.SaveChanges();
            return (true, string.Empty);
        }

        private (bool Success, string ErrorMessage) ValidateProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                return (false, "Название товара обязательно для заполнения");

            if (string.IsNullOrWhiteSpace(product.Category))
                return (false, "Категория товара обязательна для заполнения");

            if (string.IsNullOrWhiteSpace(product.Manufacturer))
                return (false, "Производитель товара обязателен для заполнения");

            if (product.Price <= 0)
                return (false, "Цена должна быть положительным числом");

            if (product.Quantity < 0)
                return (false, "Количество не может быть отрицательным");

            return (true, string.Empty);
        }

        public List<string> GetCategories()
        {
            return new List<string>
        {
            "Холодильники",
            "Стиральные машины",
            "Пылесосы",
            "Микроволновые печи",
            "Электрические плиты",
            "Телевизоры",
            "Кофемашины",
            "Кондиционеры",
            "Мелкая бытовая техника"
        };
        }
    }
}
