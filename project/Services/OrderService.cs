using project.Data;
using project.Models;
using Microsoft.EntityFrameworkCore;

namespace project.Services;

public class OrderService
{
    public (bool Success, string ErrorMessage, int OrderId) CreateOrder(int userId, List<CartItem> items)
    {
        if (items == null || items.Count == 0)
            return (false, "Корзина пуста", 0);

        using var context = new AppDbContext();

        foreach (var item in items)
        {
            var product = context.Products.Find(item.Product.Id);
            if (product == null)
                return (false, $"Товар \"{item.Product.Name}\" не найден", 0);
            if (product.Quantity < item.Quantity)
                return (false, $"Недостаточно товара \"{product.Name}\". Доступно: {product.Quantity} шт.", 0);
        }

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.Now,
            TotalPrice = items.Sum(i => i.Subtotal),
            Status = "Новый",
            Items = items.Select(i => new OrderItem
            {
                ProductId = i.Product.Id,
                Quantity = i.Quantity,
                Price = i.Product.Price
            }).ToList()
        };

        context.Orders.Add(order);

        foreach (var item in items)
        {
            var product = context.Products.Find(item.Product.Id);
            if (product != null)
                product.Quantity -= item.Quantity;
        }

        context.SaveChanges();
        return (true, string.Empty, order.Id);
    }

    public List<Order> GetOrdersByUser(int userId)
    {
        using var context = new AppDbContext();
        return context.Orders
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToList();
    }

    public List<Order> GetAllOrders()
    {
        using var context = new AppDbContext();
        return context.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToList();
    }

    public (bool Success, string ErrorMessage) UpdateOrderStatus(int orderId, string newStatus)
    {
        using var context = new AppDbContext();
        var order = context.Orders.Find(orderId);
        if (order == null)
            return (false, "Заказ не найден");

        order.Status = newStatus;
        context.SaveChanges();
        return (true, string.Empty);
    }

    public List<string> GetStatuses()
    {
        return new List<string> { "Новый", "В обработке", "Выполнен", "Отменён" };
    }
}