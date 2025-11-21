using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignPatterns.CQRS
{
    // ❌ BAD: Mixing read and write operations in the same model

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // ❌ Single service handles both commands and queries
    public class ProductService
    {
        private readonly List<Product> _products = new();

        // ❌ Command operation
        public void CreateProduct(string name, decimal price, int stock)
        {
            var product = new Product
            {
                Id = _products.Count + 1,
                Name = name,
                Price = price,
                StockQuantity = stock,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _products.Add(product);
            // ❌ Validation mixed with persistence
            // ❌ No separation of concerns
        }

        // ❌ Command operation
        public void UpdateProductPrice(int productId, decimal newPrice)
        {
            var product = _products.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                product.Price = newPrice;
                product.UpdatedAt = DateTime.UtcNow;
            }
            // ❌ Using same model for write operations
        }

        // ❌ Query operation
        public Product GetProduct(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
            // ❌ Returning full entity even if we only need name and price
            // ❌ No optimization for read-only scenarios
        }

        // ❌ Query operation
        public List<Product> GetAllProducts()
        {
            return _products.ToList();
            // ❌ No pagination, filtering, or projections
            // ❌ Returns full entities when we might only need summary
        }

        // ❌ Complex query mixed with business logic
        public List<Product> GetLowStockProducts()
        {
            return _products.Where(p => p.StockQuantity < 10).ToList();
            // ❌ Query logic mixed in service
            // ❌ Difficult to optimize separately from writes
        }
    }

    // ❌ PROBLEM: 
    // - Write model = Read model (same structure)
    // - No optimization for different access patterns
    // - Difficult to scale reads and writes independently
    // - Business logic mixed with data access
    // - Hard to cache read operations
}
