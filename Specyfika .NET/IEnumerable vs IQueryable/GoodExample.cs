using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecyfikaDotNet.EntityFramework
{
    // ✅ GOOD: Filtrowanie na IQueryable - SQL jest generowany z filtrem WHERE
    public class GoodProductRepository
    {
        private readonly AppDbContext _context;

        public GoodProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Product> GetExpensiveProducts(decimal minPrice)
        {
            // Where() na IQueryable buduje drzewo wyrażeń (Expression Tree)
            // ToList() wykonuje zapytanie SQL z WHERE price >= @minPrice
            return _context.Products
                .Where(p => p.Price >= minPrice)
                .ToList();
        }

        public List<Product> GetProductsByCategory(string category)
        {
            // Filtrowanie na poziomie bazy danych
            // SQL: SELECT * FROM Products WHERE Category = @category
            return _context.Products
                .Where(p => p.Category == category)
                .ToList();
        }

        public List<Product> GetPagedProducts(int pageSize, int pageNumber)
        {
            // Skip i Take także działają na IQueryable
            // SQL: SELECT * FROM Products ORDER BY Id OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY
            return _context.Products
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }

    // Uproszczona klasa DbContext dla przykładu
    public class AppDbContext
    {
        public IQueryable<Product> Products { get; set; }
    }
}
