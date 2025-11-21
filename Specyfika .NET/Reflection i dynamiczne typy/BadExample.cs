using System;
using System.Reflection;

namespace SpecyfikaDotNet.Reflection
{
    // ❌ BAD: Niebezpieczne i nieefektywne użycie reflection bez walidacji
    public class BadReflectionExample
    {
        public void CallMethodByName(object obj, string methodName, object[] parameters)
        {
            // Brak sprawdzenia czy obiekt nie jest null
            Type type = obj.GetType();
            
            // Brak sprawdzenia czy metoda istnieje
            MethodInfo method = type.GetMethod(methodName);
            
            // NullReferenceException jeśli metoda nie istnieje
            method.Invoke(obj, parameters);
        }

        public object GetPropertyValue(object obj, string propertyName)
        {
            Type type = obj.GetType();
            
            // Brak walidacji - crash jeśli property nie istnieje
            PropertyInfo prop = type.GetProperty(propertyName);
            return prop.GetValue(obj);
        }
    }

    // Przykładowa klasa testowa
    public class Calculator
    {
        public int Add(int a, int b) => a + b;
        public int Multiply(int a, int b) => a * b;
        public string Name { get; set; } = "Calculator";
    }

    public class BadUsageExample
    {
        public static void Main()
        {
            var bad = new BadReflectionExample();
            var calc = new Calculator();

            // Ryzyko: brak obsługi błędów
            bad.CallMethodByName(calc, "Add", new object[] { 5, 3 });
            
            // To spowoduje crash - metoda nie istnieje
            bad.CallMethodByName(calc, "NonExistentMethod", null);
            
            // Nieefektywne - za każdym razem pełne reflection lookup
            for (int i = 0; i < 1000; i++)
            {
                bad.CallMethodByName(calc, "Add", new object[] { i, i + 1 });
            }
        }
    }
}
