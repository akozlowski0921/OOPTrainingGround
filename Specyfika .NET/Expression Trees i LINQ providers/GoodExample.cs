using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SpecyfikaDotNet.ExpressionTrees
{
    // ✅ GOOD: Używanie Expression Trees do bezpiecznej analizy wyrażeń lambda
    public class GoodExpressionAnalyzer
    {
        public void AnalyzeExpression<T>(Expression<Func<T, bool>> expression)
        {
            Console.WriteLine($"Expression: {expression}");
            Console.WriteLine($"Expression Type: {expression.NodeType}");
            Console.WriteLine($"Return Type: {expression.ReturnType}");
            
            // Analiza body wyrażenia
            AnalyzeNode(expression.Body, 0);
        }

        private void AnalyzeNode(Expression node, int depth)
        {
            string indent = new string(' ', depth * 2);
            
            Console.WriteLine($"{indent}Node Type: {node.NodeType}");
            Console.WriteLine($"{indent}Type: {node.Type}");

            switch (node)
            {
                case BinaryExpression binary:
                    Console.WriteLine($"{indent}Binary: {binary.NodeType}");
                    Console.WriteLine($"{indent}Left:");
                    AnalyzeNode(binary.Left, depth + 1);
                    Console.WriteLine($"{indent}Right:");
                    AnalyzeNode(binary.Right, depth + 1);
                    break;

                case MemberExpression member:
                    Console.WriteLine($"{indent}Member: {member.Member.Name}");
                    if (member.Expression != null)
                    {
                        Console.WriteLine($"{indent}Expression:");
                        AnalyzeNode(member.Expression, depth + 1);
                    }
                    break;

                case ConstantExpression constant:
                    Console.WriteLine($"{indent}Value: {constant.Value}");
                    break;

                case ParameterExpression parameter:
                    Console.WriteLine($"{indent}Parameter: {parameter.Name}");
                    break;

                case MethodCallExpression methodCall:
                    Console.WriteLine($"{indent}Method: {methodCall.Method.Name}");
                    Console.WriteLine($"{indent}Arguments:");
                    foreach (var arg in methodCall.Arguments)
                    {
                        AnalyzeNode(arg, depth + 1);
                    }
                    break;

                case UnaryExpression unary:
                    Console.WriteLine($"{indent}Unary: {unary.NodeType}");
                    AnalyzeNode(unary.Operand, depth + 1);
                    break;
            }
        }

        public string ExtractPropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            if (expression.Body is MemberExpression member)
            {
                return member.Member.Name;
            }

            if (expression.Body is UnaryExpression unary && unary.Operand is MemberExpression unaryMember)
            {
                return unaryMember.Member.Name;
            }

            throw new ArgumentException("Expression must be a member access", nameof(expression));
        }

        public object GetConstantValue<T>(Expression<Func<T, bool>> expression)
        {
            // Parsowanie wyrażenia typu: x => x.Price > 100
            if (expression.Body is BinaryExpression binary)
            {
                if (binary.Right is ConstantExpression constant)
                {
                    return constant.Value;
                }
                
                if (binary.Right is MemberExpression member)
                {
                    // Kompilacja i wykonanie do pobrania wartości
                    var lambda = Expression.Lambda<Func<object>>(
                        Expression.Convert(member, typeof(object)));
                    return lambda.Compile()();
                }
            }

            throw new ArgumentException("Cannot extract constant value from expression");
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }

    public class GoodExpressionUsage
    {
        public static void Main()
        {
            var analyzer = new GoodExpressionAnalyzer();

            // Analiza prostego wyrażenia
            Expression<Func<Product, bool>> simpleExpr = p => p.Price > 100;
            Console.WriteLine("=== Simple Expression Analysis ===");
            analyzer.AnalyzeExpression(simpleExpr);

            // Analiza złożonego wyrażenia
            Expression<Func<Product, bool>> complexExpr = 
                p => p.Price > 100 && p.Category == "Electronics";
            Console.WriteLine("\n=== Complex Expression Analysis ===");
            analyzer.AnalyzeExpression(complexExpr);

            // Ekstrakcja nazwy property
            Expression<Func<Product, string>> nameExpr = p => p.Name;
            string propertyName = analyzer.ExtractPropertyName(nameExpr);
            Console.WriteLine($"\n=== Extracted Property Name: {propertyName} ===");

            // Ekstrakcja wartości stałej
            try
            {
                object value = analyzer.GetConstantValue(simpleExpr);
                Console.WriteLine($"\n=== Extracted Constant Value: {value} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Type safety - literówki są wykrywane w compile-time
            // Expression<Func<Product, bool>> invalid = p => p.Pric > 100; // Błąd kompilacji
        }
    }
}
