using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecyfikaDotNet.EntityFramework
{
    // ❌ BAD: Pobieranie wszystkich danych z bazy do pamięci RAM, a potem filtrowanie
    public class BadProductRepository
    {
        private readonly AppDbContext _context;

        public BadProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Product> GetExpensiveProducts(decimal minPrice)
        {
            // ToList() wymusza wykonanie zapytania i załadowanie WSZYSTKICH produktów do RAM
            var allProducts = _context.Products.ToList();
            
            // Filtrowanie w pamięci aplikacji zamiast na bazie danych
            return allProducts.Where(p => p.Price >= minPrice).ToList();
        }

        public List<Product> GetProductsByCategory(string category)
        {
            // Ładowanie wszystkich produktów do pamięci
            var allProducts = _context.Products.ToList();
            
            // Filtrowanie po stronie aplikacji - nieefektywne
            return allProducts.Where(p => p.Category == category).ToList();
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
