using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace SpecyfikaDotNet.Reflection
{
    // ✅ GOOD: Bezpieczne i wydajne użycie reflection z cache i walidacją
    public class GoodReflectionExample
    {
        // Cache dla MethodInfo - reflection lookup jest kosztowny
        private readonly ConcurrentDictionary<string, MethodInfo> _methodCache = new();
        private readonly ConcurrentDictionary<string, PropertyInfo> _propertyCache = new();

        public bool TryCallMethodByName(object obj, string methodName, object[] parameters, out object result)
        {
            result = null;

            if (obj == null || string.IsNullOrWhiteSpace(methodName))
                return false;

            try
            {
                Type type = obj.GetType();
                string cacheKey = $"{type.FullName}.{methodName}";

                // Pobierz z cache lub dodaj do cache
                MethodInfo method = _methodCache.GetOrAdd(cacheKey, _ =>
                {
                    return type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
                });

                if (method == null)
                    return false;

                // Walidacja parametrów
                ParameterInfo[] methodParams = method.GetParameters();
                if (parameters == null && methodParams.Length > 0)
                    return false;

                if (parameters != null && parameters.Length != methodParams.Length)
                    return false;

                result = method.Invoke(obj, parameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error invoking method {methodName}: {ex.Message}");
                return false;
            }
        }

        public bool TryGetPropertyValue(object obj, string propertyName, out object value)
        {
            value = null;

            if (obj == null || string.IsNullOrWhiteSpace(propertyName))
                return false;

            try
            {
                Type type = obj.GetType();
                string cacheKey = $"{type.FullName}.{propertyName}";

                PropertyInfo prop = _propertyCache.GetOrAdd(cacheKey, _ =>
                {
                    return type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                });

                if (prop == null)
                    return false;

                value = prop.GetValue(obj);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting property {propertyName}: {ex.Message}");
                return false;
            }
        }
    }

    // Przykładowa klasa testowa
    public class Calculator
    {
        public int Add(int a, int b) => a + b;
        public int Multiply(int a, int b) => a * b;
        public string Name { get; set; } = "Calculator";
    }

    public class GoodUsageExample
    {
        public static void Main()
        {
            var good = new GoodReflectionExample();
            var calc = new Calculator();

            // Bezpieczne wywołanie z obsługą błędów
            if (good.TryCallMethodByName(calc, "Add", new object[] { 5, 3 }, out object result))
            {
                Console.WriteLine($"Result: {result}"); // Output: Result: 8
            }

            // Bezpieczna obsługa nieistniejącej metody
            if (!good.TryCallMethodByName(calc, "NonExistentMethod", null, out _))
            {
                Console.WriteLine("Method not found - handled gracefully");
            }

            // Wydajne - cache jest używany przy kolejnych wywołaniach
            for (int i = 0; i < 1000; i++)
            {
                good.TryCallMethodByName(calc, "Add", new object[] { i, i + 1 }, out _);
            }

            // Bezpieczne pobieranie property
            if (good.TryGetPropertyValue(calc, "Name", out object name))
            {
                Console.WriteLine($"Calculator name: {name}");
            }
        }
    }
}
