using System;
using System.IO;
using System.Reflection;

namespace SpecyfikaDotNet.Reflection
{
    // ❌ BAD: Niebezpieczne ładowanie assembly bez izolacji i walidacji
    public class BadAssemblyLoader
    {
        public Assembly LoadExternalAssembly(string assemblyPath)
        {
            // Brak sprawdzenia czy plik istnieje
            // Brak walidacji czy to bezpieczny plik
            // Assembly jest ładowane do domyślnego kontekstu - nie można go unload
            return Assembly.LoadFrom(assemblyPath);
        }

        public object CreateInstanceFromAssembly(string assemblyPath, string typeName)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            // Brak sprawdzenia czy typ istnieje
            // Brak obsługi wyjątków
            Type type = assembly.GetType(typeName);
            return Activator.CreateInstance(type);
        }

        public void ExecuteMethodFromAssembly(string assemblyPath, string typeName, string methodName)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            Type type = assembly.GetType(typeName);
            object instance = Activator.CreateInstance(type);

            MethodInfo method = type.GetMethod(methodName);
            
            // Niebezpieczne wykonanie bez sandboxingu
            // Brak obsługi błędów
            method.Invoke(instance, null);
        }
    }

    public class BadAssemblyLoaderUsage
    {
        public static void Main()
        {
            var loader = new BadAssemblyLoader();

            try
            {
                // Ładowanie bez walidacji - ryzyko bezpieczeństwa
                Assembly assembly = loader.LoadExternalAssembly("ExternalPlugin.dll");

                // Wielokrotne ładowanie tego samego assembly - memory leak
                for (int i = 0; i < 10; i++)
                {
                    loader.LoadExternalAssembly("ExternalPlugin.dll");
                }

                // Brak możliwości unload - assembly zostaje w pamięci na zawsze
                // Problem: jeśli plugin jest aktualizowany, stara wersja pozostaje w pamięci

                // Wykonanie kodu z zewnętrznego assembly bez sandbox
                loader.ExecuteMethodFromAssembly("ExternalPlugin.dll", "Plugin.MainClass", "Execute");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
