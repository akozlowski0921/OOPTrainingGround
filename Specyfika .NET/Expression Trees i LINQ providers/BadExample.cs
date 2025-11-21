using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecyfikaDotNet.ExpressionTrees
{
    // ❌ BAD: Ręczne budowanie SQL query przez konkatenację stringów
    public class BadQueryBuilder
    {
        public string BuildWhereClause(string fieldName, string operation, object value)
        {
            // Podatne na SQL Injection
            // Brak walidacji parametrów
            // Trudne w utrzymaniu
            return $"{fieldName} {operation} '{value}'";
        }

        public string BuildComplexQuery(Dictionary<string, object> conditions)
        {
            StringBuilder query = new StringBuilder("SELECT * FROM Products WHERE ");
            
            // Ręczne łączenie warunków
            bool first = true;
            foreach (var condition in conditions)
            {
                if (!first)
                    query.Append(" AND ");
                
                query.Append($"{condition.Key} = '{condition.Value}'");
                first = false;
            }

            return query.ToString();
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }

    public class BadExpressionUsage
    {
        public static void Main()
        {
            var builder = new BadQueryBuilder();

            // Niebezpieczne - SQL Injection risk
            string whereClause = builder.BuildWhereClause("Name", "=", "'; DROP TABLE Products; --");
            Console.WriteLine(whereClause);

            // Brak type safety - literówki w nazwach kolumn
            var conditions = new Dictionary<string, object>
            {
                { "Nmae", "Product1" }, // Literówka nie zostanie wykryta
                { "Pric", 100 }         // Literówka nie zostanie wykryta
            };

            string query = builder.BuildComplexQuery(conditions);
            Console.WriteLine(query);

            // Brak możliwości analizy zapytania
            // Brak możliwości optymalizacji
            // Trudne testowanie
        }
    }
}
