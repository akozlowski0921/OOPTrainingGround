using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecyfikaDotNet.EntityFramework.Good2
{
    // ✅ GOOD: Używanie IQueryable dla odroczenia wykonania zapytania
    public class OrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<OrderSummary> GetLargeOrdersWithDetails(decimal minAmount)
        {
            // IQueryable pozwala budować zapytanie
            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Where(o => o.TotalAmount >= minAmount) // Filtrowanie na bazie
                .Select(o => new OrderSummary          // Projekcja na bazie
                {
                    OrderId = o.Id,
                    CustomerName = o.Customer.Name,
                    ItemCount = o.OrderItems.Count,
                    TotalAmount = o.TotalAmount
                });

            // Zapytanie wykonuje się dopiero tutaj, z optymalizacjami
            return query.ToList();
        }

        public int CountOrdersByStatus(string status)
        {
            // Count() wykonywany na bazie danych (SELECT COUNT(*))
            return _context.Orders.Count(o => o.Status == status);
        }

        public decimal GetAverageOrderValue()
        {
            // Average() wykonywany na bazie danych (SELECT AVG())
            return _context.Orders.Average(o => o.TotalAmount);
        }

        public IQueryable<Order> GetOrdersQuery()
        {
            // Zwracamy IQueryable, pozwalając wywołującemu na dalsze budowanie zapytania
            return _context.Orders;
        }

        public List<Order> SearchOrders(string customerName = null, decimal? minAmount = null, string status = null)
        {
            IQueryable<Order> query = _context.Orders;

            // Dynamiczne budowanie zapytania
            if (!string.IsNullOrEmpty(customerName))
                query = query.Where(o => o.Customer.Name.Contains(customerName));

            if (minAmount.HasValue)
                query = query.Where(o => o.TotalAmount >= minAmount.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            // Zapytanie wykonuje się dopiero tutaj, z wszystkimi warunkami
            return query.ToList();
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
