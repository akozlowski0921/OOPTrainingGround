using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Collections.Generic;

namespace SpecyfikaDotNet.Reflection
{
    // ✅ GOOD: Bezpieczne ładowanie assembly z możliwością unload i izolacją
    public class GoodAssemblyLoader : IDisposable
    {
        private readonly Dictionary<string, WeakReference> _loadedAssemblies = new();
        private AssemblyLoadContext _loadContext;

        public GoodAssemblyLoader(string contextName = null)
        {
            _loadContext = new AssemblyLoadContext(
                contextName ?? $"PluginContext_{Guid.NewGuid():N}",
                isCollectible: true); // Umożliwia unload
        }

        public bool TryLoadAssembly(string assemblyPath, out Assembly assembly)
        {
            assembly = null;

            try
            {
                // Walidacja ścieżki
                if (string.IsNullOrWhiteSpace(assemblyPath))
                {
                    Console.WriteLine("Assembly path is null or empty");
                    return false;
                }

                if (!File.Exists(assemblyPath))
                {
                    Console.WriteLine($"Assembly file not found: {assemblyPath}");
                    return false;
                }

                // Sprawdź cache
                if (_loadedAssemblies.TryGetValue(assemblyPath, out WeakReference weakRef) && 
                    weakRef.IsAlive)
                {
                    assembly = weakRef.Target as Assembly;
                    Console.WriteLine($"Assembly loaded from cache: {assemblyPath}");
                    return assembly != null;
                }

                // Ładowanie w izolowanym kontekście
                assembly = _loadContext.LoadFromAssemblyPath(Path.GetFullPath(assemblyPath));
                
                // Zapisz do cache z WeakReference
                _loadedAssemblies[assemblyPath] = new WeakReference(assembly);
                
                Console.WriteLine($"Assembly loaded successfully: {assembly.FullName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading assembly: {ex.Message}");
                return false;
            }
        }

        public bool TryCreateInstance(Assembly assembly, string typeName, out object instance)
        {
            instance = null;

            try
            {
                if (assembly == null || string.IsNullOrWhiteSpace(typeName))
                    return false;

                Type type = assembly.GetType(typeName);
                
                if (type == null)
                {
                    Console.WriteLine($"Type not found: {typeName}");
                    return false;
                }

                // Sprawdź czy typ ma publiczny konstruktor bezparametrowy
                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor == null)
                {
                    Console.WriteLine($"Type {typeName} has no parameterless constructor");
                    return false;
                }

                instance = Activator.CreateInstance(type);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating instance: {ex.Message}");
                return false;
            }
        }

        public bool TryExecuteMethod(object instance, string methodName, object[] parameters, out object result)
        {
            result = null;

            try
            {
                if (instance == null || string.IsNullOrWhiteSpace(methodName))
                    return false;

                Type type = instance.GetType();
                MethodInfo method = type.GetMethod(
                    methodName,
                    BindingFlags.Public | BindingFlags.Instance);

                if (method == null)
                {
                    Console.WriteLine($"Method not found: {methodName}");
                    return false;
                }

                // Walidacja parametrów
                ParameterInfo[] methodParams = method.GetParameters();
                if ((parameters == null && methodParams.Length > 0) ||
                    (parameters != null && parameters.Length != methodParams.Length))
                {
                    Console.WriteLine($"Parameter count mismatch for method {methodName}");
                    return false;
                }

                result = method.Invoke(instance, parameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing method: {ex.Message}");
                return false;
            }
        }

        public void Unload()
        {
            if (_loadContext != null)
            {
                _loadContext.Unload();
                _loadedAssemblies.Clear();
                Console.WriteLine("Assembly load context unloaded successfully");
            }
        }

        public void Dispose()
        {
            Unload();
            _loadContext = null;
        }
    }

    // Przykładowa klasa plugin do testowania
    public class SamplePlugin
    {
        public string Name => "Sample Plugin";

        public void Execute()
        {
            Console.WriteLine("Plugin executed successfully!");
        }

        public int Calculate(int a, int b)
        {
            return a + b;
        }
    }

    public class GoodAssemblyLoaderUsage
    {
        public static void Main()
        {
            // Użycie z using dla automatycznego Dispose
            using (var loader = new GoodAssemblyLoader("PluginContext"))
            {
                // Symulacja - w prawdziwym scenariuszu byłby zewnętrzny plik DLL
                string assemblyPath = Assembly.GetExecutingAssembly().Location;

                if (loader.TryLoadAssembly(assemblyPath, out Assembly assembly))
                {
                    Console.WriteLine($"Loaded: {assembly.GetName().Name}");

                    // Bezpieczne tworzenie instancji
                    if (loader.TryCreateInstance(
                        assembly,
                        "SpecyfikaDotNet.Reflection.SamplePlugin",
                        out object pluginInstance))
                    {
                        Console.WriteLine("Plugin instance created");

                        // Bezpieczne wykonanie metody
                        if (loader.TryExecuteMethod(pluginInstance, "Execute", null, out _))
                        {
                            Console.WriteLine("Method executed successfully");
                        }

                        // Wykonanie metody z parametrami
                        if (loader.TryExecuteMethod(
                            pluginInstance,
                            "Calculate",
                            new object[] { 5, 3 },
                            out object calcResult))
                        {
                            Console.WriteLine($"Calculation result: {calcResult}");
                        }
                    }
                }

                // Próba załadowania nieistniejącego assembly
                if (!loader.TryLoadAssembly("NonExistent.dll", out _))
                {
                    Console.WriteLine("Gracefully handled missing assembly");
                }
            } // AssemblyLoadContext jest unloadowany automatycznie

            // GC może teraz zwolnić pamięć zajmowaną przez załadowane assembly
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Console.WriteLine("Memory cleaned up");
        }
    }
}
