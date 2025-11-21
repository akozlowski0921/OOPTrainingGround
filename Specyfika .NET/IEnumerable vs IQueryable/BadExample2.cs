using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecyfikaDotNet.EntityFramework.Bad2
{
    // ❌ BAD: Używanie IEnumerable zamiast IQueryable dla złożonych zapytań
    public class OrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<OrderSummary> GetLargeOrdersWithDetails(decimal minAmount)
        {
            // ToList() ładuje wszystkie zamówienia do pamięci
            var allOrders = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ToList(); // <-- BŁĄD: Ładujemy wszystko do RAM

            // Filtrowanie w pamięci aplikacji
            var filtered = allOrders.Where(o => o.TotalAmount >= minAmount);

            // Transformacja w pamięci aplikacji
            return filtered.Select(o => new OrderSummary
            {
                OrderId = o.Id,
                CustomerName = o.Customer.Name,
                ItemCount = o.OrderItems.Count,
                TotalAmount = o.TotalAmount
            }).ToList();
        }

        public int CountOrdersByStatus(string status)
        {
            // Pobiera wszystkie zamówienia do pamięci, potem liczy
            return _context.Orders.ToList().Count(o => o.Status == status);
        }

        public decimal GetAverageOrderValue()
        {
            // Ładuje wszystkie zamówienia aby obliczyć średnią
            return _context.Orders.ToList().Average(o => o.TotalAmount);
        }
    }

    public class Order
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public Customer Customer { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderSummary
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class AppDbContext
    {
        public IQueryable<Order> Orders { get; set; }
    }

    public static class QueryableExtensions
    {
        public static IQueryable<T> Include<T>(this IQueryable<T> source, Func<T, object> func) => source;
    }
}
