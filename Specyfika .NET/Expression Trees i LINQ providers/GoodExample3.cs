using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SpecyfikaDotNet.ExpressionTrees
{
    // ✅ GOOD: Prawidłowa implementacja LINQ providera z translacją do SQL-like syntax
    public class GoodCustomQueryProvider : IQueryProvider
    {
        private readonly List<Product> _dataSource;

        public GoodCustomQueryProvider(List<Product> dataSource)
        {
            _dataSource = dataSource ?? new List<Product>();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException("Use generic version");
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new GoodCustomQueryable<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return Execute<object>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            // Analizuj expression tree
            var translator = new QueryTranslator();
            string sqlLike = translator.Translate(expression);
            
            Console.WriteLine($"Translated query: {sqlLike}");

            // Wykonaj zapytanie (w prawdziwym scenariuszu: zapytanie do bazy)
            var compiled = CompileExpression<TResult>(expression);
            return compiled();
        }

        private Func<TResult> CompileExpression<TResult>(Expression expression)
        {
            // Kompiluj expression do wykonywalnej funkcji
            var lambda = Expression.Lambda<Func<TResult>>(expression);
            return lambda.Compile();
        }

        public IEnumerable<T> ExecuteEnumerable<T>(Expression expression)
        {
            // Tłumacz i wykonaj zapytanie
            var translator = new QueryTranslator();
            string sqlLike = translator.Translate(expression);
            
            Console.WriteLine($"Executing query: {sqlLike}");

            // Wykonaj faktyczne zapytanie na źródle danych
            var visitor = new QueryExecutor(_dataSource);
            return visitor.Execute<T>(expression);
        }
    }

    // Translator expression tree do SQL-like syntax
    public class QueryTranslator : ExpressionVisitor
    {
        private readonly StringBuilder _sql = new();
        private string _tableName = "Products";

        public string Translate(Expression expression)
        {
            _sql.Clear();
            _sql.Append($"SELECT * FROM {_tableName}");
            Visit(expression);
            return _sql.ToString();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Where")
            {
                Visit(node.Arguments[0]); // Source
                _sql.Append(" WHERE ");
                
                // Extract lambda from Where
                var lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
                VisitWhere(lambda.Body);
                
                return node;
            }

            if (node.Method.Name == "Select")
            {
                Visit(node.Arguments[0]); // Source
                // Dodaj logikę dla Select jeśli potrzebna
                return node;
            }

            return base.VisitMethodCall(node);
        }

        private void VisitWhere(Expression expression)
        {
            if (expression is BinaryExpression binary)
            {
                VisitBinary(binary);
            }
        }

        private void VisitBinary(BinaryExpression binary)
        {
            if (binary.Left is MemberExpression member)
            {
                _sql.Append(member.Member.Name);
            }

            _sql.Append($" {GetOperator(binary.NodeType)} ");

            if (binary.Right is ConstantExpression constant)
            {
                if (constant.Type == typeof(string))
                    _sql.Append($"'{constant.Value}'");
                else
                    _sql.Append(constant.Value);
            }
        }

        private string GetOperator(ExpressionType nodeType)
        {
            return nodeType switch
            {
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "<>",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.AndAlso => "AND",
                ExpressionType.OrElse => "OR",
                _ => throw new NotSupportedException($"Operator {nodeType} not supported")
            };
        }

        private static Expression StripQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
            {
                expression = ((UnaryExpression)expression).Operand;
            }
            return expression;
        }
    }

    // Executor zapytań
    public class QueryExecutor
    {
        private readonly IEnumerable<Product> _dataSource;

        public QueryExecutor(IEnumerable<Product> dataSource)
        {
            _dataSource = dataSource;
        }

        public IEnumerable<T> Execute<T>(Expression expression)
        {
            // Kompiluj i wykonaj expression
            var lambda = Expression.Lambda<Func<IEnumerable<T>>>(expression);
            var compiled = lambda.Compile();
            return compiled();
        }
    }

    public class GoodCustomQueryable<T> : IQueryable<T>
    {
        public GoodCustomQueryable(GoodCustomQueryProvider provider, Expression expression)
        {
            Provider = provider;
            Expression = expression;
        }

        public Type ElementType => typeof(T);
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }

        public IEnumerator<T> GetEnumerator()
        {
            // Wykonaj zapytanie przez providera
            return ((GoodCustomQueryProvider)Provider)
                .ExecuteEnumerable<T>(Expression)
                .GetEnumerator();
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
        public string Category { get; set; }
    }

    public class GoodCustomProviderUsage
    {
        public static void Main()
        {
            // Źródło danych
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Price = 1000, Category = "Electronics" },
                new Product { Id = 2, Name = "Mouse", Price = 20, Category = "Electronics" },
                new Product { Id = 3, Name = "Desk", Price = 300, Category = "Furniture" }
            };

            // Utworzenie providera z danymi
            var provider = new GoodCustomQueryProvider(products);
            
            // Utworzenie queryable z expression
            var queryable = new GoodCustomQueryable<Product>(
                provider,
                Expression.Constant(products.AsQueryable())
            );

            // LINQ query - zostanie przetłumaczone na SQL-like syntax
            var query = queryable.Where(p => p.Price > 100);

            Console.WriteLine("=== Executing query ===");
            var result = query.ToList();

            Console.WriteLine($"\nResults ({result.Count} items):");
            foreach (var product in result)
            {
                Console.WriteLine($"- {product.Name}: ${product.Price}");
            }

            // Provider analizuje expression tree
            // Tłumaczy na SQL-like syntax
            // Optymalizuje zapytanie
            // Możliwość integracji z bazą danych
        }
    }
}
