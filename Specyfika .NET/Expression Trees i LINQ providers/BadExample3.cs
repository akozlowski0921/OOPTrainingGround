using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SpecyfikaDotNet.ExpressionTrees
{
    // ❌ BAD: Nieprawidłowa implementacja LINQ providera - nie tłumaczy na natywne zapytania
    public class BadCustomQueryProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            // Zwraca zawsze to samo bez analizy expression
            return new BadCustomQueryable<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            // Zawsze zwraca null - nie tłumaczy wyrażenia
            return null;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            // Nie analizuje expression tree
            // Nie optymalizuje zapytania
            // Po prostu wykonuje w pamięci
            Console.WriteLine("Executing in memory - not optimized");
            return default;
        }
    }

    public class BadCustomQueryable<T> : IQueryable<T>
    {
        public BadCustomQueryable(IQueryProvider provider, Expression expression)
        {
            Provider = provider;
            Expression = expression;
        }

        public Type ElementType => typeof(T);
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }

        public IEnumerator<T> GetEnumerator()
        {
            // Nie wykonuje zapytania - zwraca pustą kolekcję
            return Enumerable.Empty<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class BadCustomProviderUsage
    {
        public static void Main()
        {
            var provider = new BadCustomQueryProvider();
            var expression = Expression.Constant(new List<Product>());
            var queryable = new BadCustomQueryable<Product>(provider, expression);

            // Provider nie tłumaczy Where na natywne zapytanie
            var filtered = queryable.Where(p => p.Price > 100);

            // GetEnumerator zwraca pustą kolekcję - dane są tracone
            var result = filtered.ToList();
            Console.WriteLine($"Result count: {result.Count}"); // 0 - źle!

            // Brak optymalizacji
            // Brak możliwości analizy zapytania
            // Brak możliwości przełożenia na SQL/inny język zapytań
        }
    }
}
