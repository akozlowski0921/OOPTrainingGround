using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SpecyfikaDotNet.ExpressionTrees
{
    // ✅ GOOD: Dynamiczne budowanie warunków LINQ w runtime
    public class GoodDynamicQuery<T>
    {
        private readonly IQueryable<T> _query;

        public GoodDynamicQuery(IQueryable<T> query)
        {
            _query = query;
        }

        public IQueryable<T> ApplyFilters(List<Filter<T>> filters)
        {
            var query = _query;

            foreach (var filter in filters)
            {
                query = query.Where(filter.Expression);
            }

            return query;
        }

        // Dynamiczne budowanie wyrażenia dla property
        public Expression<Func<T, bool>> BuildPropertyFilter(string propertyName, object value, FilterOperator op)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var constant = Expression.Constant(value);

            Expression comparison = op switch
            {
                FilterOperator.Equal => Expression.Equal(property, constant),
                FilterOperator.NotEqual => Expression.NotEqual(property, constant),
                FilterOperator.GreaterThan => Expression.GreaterThan(property, constant),
                FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, constant),
                FilterOperator.LessThan => Expression.LessThan(property, constant),
                FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(property, constant),
                _ => throw new ArgumentException($"Unsupported operator: {op}")
            };

            return Expression.Lambda<Func<T, bool>>(comparison, parameter);
        }

        // Łączenie wielu wyrażeń operatorem AND
        public Expression<Func<T, bool>> CombineWithAnd(
            Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var body1 = new ParameterReplacer(parameter).Visit(expr1.Body);
            var body2 = new ParameterReplacer(parameter).Visit(expr2.Body);

            var combined = Expression.AndAlso(body1, body2);

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        // Łączenie wielu wyrażeń operatorem OR
        public Expression<Func<T, bool>> CombineWithOr(
            Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var body1 = new ParameterReplacer(parameter).Visit(expr1.Body);
            var body2 = new ParameterReplacer(parameter).Visit(expr2.Body);

            var combined = Expression.OrElse(body1, body2);

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }
    }

    // Helper do zamiany parametrów w wyrażeniach
    public class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        public ParameterReplacer(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _parameter;
        }
    }

    public class Filter<T>
    {
        public string Name { get; set; }
        public Expression<Func<T, bool>> Expression { get; set; }
    }

    public enum FilterOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }

    public class GoodDynamicQueryUsage
    {
        public static void Main()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Price = 1000, Category = "Electronics" },
                new Product { Id = 2, Name = "Mouse", Price = 20, Category = "Electronics" },
                new Product { Id = 3, Name = "Desk", Price = 300, Category = "Furniture" },
                new Product { Id = 4, Name = "Chair", Price = 150, Category = "Furniture" }
            }.AsQueryable();

            var dynamicQuery = new GoodDynamicQuery<Product>(products);

            // Dynamiczne budowanie filtrów
            var priceFilter = dynamicQuery.BuildPropertyFilter("Price", 100m, FilterOperator.GreaterThanOrEqual);
            var categoryFilter = dynamicQuery.BuildPropertyFilter("Category", "Electronics", FilterOperator.Equal);

            // Łączenie filtrów
            var combinedFilter = dynamicQuery.CombineWithAnd(priceFilter, categoryFilter);

            // Aplikowanie złożonego filtra
            var result = products.Where(combinedFilter).ToList();

            Console.WriteLine("Products matching Price >= 100 AND Category = Electronics:");
            foreach (var product in result)
            {
                Console.WriteLine($"- {product.Name}: ${product.Price}");
            }

            // Użycie z listą filtrów
            var filters = new List<Filter<Product>>
            {
                new Filter<Product>
                {
                    Name = "MinPrice",
                    Expression = p => p.Price >= 100
                },
                new Filter<Product>
                {
                    Name = "Category",
                    Expression = p => p.Category == "Furniture"
                }
            };

            var filteredQuery = dynamicQuery.ApplyFilters(filters);
            var result2 = filteredQuery.ToList();

            Console.WriteLine("\nProducts with filters (Price >= 100 AND Category = Furniture):");
            foreach (var product in result2)
            {
                Console.WriteLine($"- {product.Name}: ${product.Price}");
            }

            // Możliwość dodawania/usuwania filtrów w runtime
            // Type safe - błędy wykrywane w compile-time
            // Łatwe w utrzymaniu i rozszerzaniu
        }
    }
}
