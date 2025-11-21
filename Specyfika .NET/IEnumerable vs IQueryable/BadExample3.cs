using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecyfikaDotNet.EntityFramework.Bad3
{
    // ❌ BAD: Zagnieżdżone zapytania z wielokrotnym ładowaniem do pamięci
    public class ReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public SalesReport GenerateMonthlySalesReport(int year, int month)
        {
            // Ładuje WSZYSTKIE zamówienia do pamięci
            var allOrders = _context.Orders.ToList();

            // Filtrowanie po dacie w pamięci
            var monthOrders = allOrders
                .Where(o => o.OrderDate.Year == year && o.OrderDate.Month == month)
                .ToList();

            // Dla każdego zamówienia ładuje produkty do pamięci
            var report = new SalesReport
            {
                Year = year,
                Month = month,
                TotalOrders = monthOrders.Count,
                TotalRevenue = monthOrders.Sum(o => o.TotalAmount)
            };

            // N+1 problem: dla każdego zamówienia osobne zapytanie
            foreach (var order in monthOrders)
            {
                var items = _context.OrderItems.ToList()
                    .Where(i => i.OrderId == order.Id)
                    .ToList();
                    
                report.ItemsSold += items.Sum(i => i.Quantity);
            }

            return report;
        }

        public List<ProductStats> GetTopSellingProducts(int count)
        {
            // Ładuje wszystkie produkty i zamówienia do pamięci
            var allProducts = _context.Products.ToList();
            var allOrderItems = _context.OrderItems.ToList();

            // Grupowanie i sortowanie w pamięci
            var stats = allOrderItems
                .GroupBy(i => i.ProductId)
                .Select(g => new ProductStats
                {
                    ProductName = allProducts.First(p => p.Id == g.Key).Name,
                    TotalSold = g.Sum(i => i.Quantity),
                    Revenue = g.Sum(i => i.Price * i.Quantity)
                })
                .OrderByDescending(s => s.TotalSold)
                .Take(count)
                .ToList();

            return stats;
        }
    }

    public class SalesReport
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ItemsSold { get; set; }
    }

    public class ProductStats
    {
        public string ProductName { get; set; }
        public int TotalSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AppDbContext
    {
        public IQueryable<Order> Orders { get; set; }
        public IQueryable<OrderItem> OrderItems { get; set; }
        public IQueryable<Product> Products { get; set; }
    }
}
