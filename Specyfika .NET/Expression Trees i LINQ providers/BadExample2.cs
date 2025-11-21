using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecyfikaDotNet.ExpressionTrees
{
    // ❌ BAD: Hardcoded warunki LINQ bez możliwości dynamicznej konfiguracji
    public class BadDynamicQuery
    {
        private readonly List<Product> _products;

        public BadDynamicQuery(List<Product> products)
        {
            _products = products;
        }

        // Osobna metoda dla każdej kombinacji filtrów - duplikacja kodu
        public List<Product> FilterByPrice(decimal minPrice)
        {
            return _products.Where(p => p.Price >= minPrice).ToList();
        }

        public List<Product> FilterByCategory(string category)
        {
            return _products.Where(p => p.Category == category).ToList();
        }

        public List<Product> FilterByPriceAndCategory(decimal minPrice, string category)
        {
            return _products.Where(p => p.Price >= minPrice && p.Category == category).ToList();
        }

        public List<Product> FilterByName(string name)
        {
            return _products.Where(p => p.Name.Contains(name)).ToList();
        }

        // Problem: dla n filtrów potrzeba 2^n metod!
        // Nie da się łatwo kombinować filtrów w runtime
        public List<Product> FilterByAll(decimal? minPrice, string category, string name)
        {
            // Brzydkie if-y i łączenie warunków
            var query = _products.AsQueryable();

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category == category);

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name));

            return query.ToList();
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }

    public class BadDynamicQueryUsage
    {
        public static void Main()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Price = 1000, Category = "Electronics" },
                new Product { Id = 2, Name = "Mouse", Price = 20, Category = "Electronics" },
                new Product { Id = 3, Name = "Desk", Price = 300, Category = "Furniture" }
            };

            var query = new BadDynamicQuery(products);

            // Dla każdego case trzeba osobnej metody
            var result1 = query.FilterByPrice(100);
            var result2 = query.FilterByCategory("Electronics");
            var result3 = query.FilterByPriceAndCategory(100, "Electronics");

            // Brak elastyczności - co jeśli user chce sortować?
            // Brak możliwości łączenia filtrów w runtime
            // Kod jest nieczytelny i trudny w utrzymaniu
        }
    }
}
