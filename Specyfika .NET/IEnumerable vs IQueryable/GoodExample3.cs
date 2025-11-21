using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecyfikaDotNet.EntityFramework.Good3
{
    // ✅ GOOD: Efektywne zapytania z IQueryable
    public class ReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public SalesReport GenerateMonthlySalesReport(int year, int month)
        {
            // Filtrowanie na bazie danych
            var monthOrders = _context.Orders
                .Where(o => o.OrderDate.Year == year && o.OrderDate.Month == month);

            // Agregacje wykonywane na bazie
            var report = new SalesReport
            {
                Year = year,
                Month = month,
                TotalOrders = monthOrders.Count(),
                TotalRevenue = monthOrders.Sum(o => o.TotalAmount),
                ItemsSold = _context.OrderItems
                    .Where(i => monthOrders.Select(o => o.Id).Contains(i.OrderId))
                    .Sum(i => i.Quantity)
            };

            return report;
        }

        public List<ProductStats> GetTopSellingProducts(int count)
        {
            // Całe zapytanie wykonywane na bazie danych
            var stats = _context.OrderItems
                .GroupBy(i => i.ProductId)
                .Select(g => new ProductStats
                {
                    ProductName = _context.Products
                        .Where(p => p.Id == g.Key)
                        .Select(p => p.Name)
                        .FirstOrDefault(),
                    TotalSold = g.Sum(i => i.Quantity),
                    Revenue = g.Sum(i => i.Price * i.Quantity)
                })
                .OrderByDescending(s => s.TotalSold)
                .Take(count)
                .ToList(); // Wykonanie zapytania

            return stats;
        }

        public List<ProductStats> GetTopSellingProductsOptimized(int count)
        {
            // Jeszcze lepsze: z explicit join
            var stats = (from item in _context.OrderItems
                        join product in _context.Products on item.ProductId equals product.Id
                        group item by new { product.Id, product.Name } into g
                        orderby g.Sum(i => i.Quantity) descending
                        select new ProductStats
                        {
                            ProductName = g.Key.Name,
                            TotalSold = g.Sum(i => i.Quantity),
                            Revenue = g.Sum(i => i.Price * i.Quantity)
                        })
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
