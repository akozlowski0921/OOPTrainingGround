using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SpecyfikaDotNet.Reflection
{
    // ❌ BAD: Nieprawidłowe dynamiczne tworzenie typów bez walidacji
    public class BadDynamicTypeCreator
    {
        public Type CreateDynamicType(string typeName)
        {
            // Brak walidacji nazwy typu
            AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
            
            // Używamy starszego API bez możliwości unload
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName, 
                AssemblyBuilderAccess.Run);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
            
            // Brak obsługi konfliktów nazw
            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Public | TypeAttributes.Class);

            // Tworzenie typu bez żadnej funkcjonalności
            return typeBuilder.CreateType();
        }

        public object CreateProxy(Type interfaceType)
        {
            // Brak sprawdzenia czy to interface
            AssemblyName assemblyName = new AssemblyName("ProxyAssembly");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Run);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("ProxyModule");

            // Próba utworzenia proxy bez implementacji metod
            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                "ProxyType",
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(object),
                new[] { interfaceType });

            Type proxyType = typeBuilder.CreateType();
            
            // To nie zadziała - brak implementacji metod interfejsu
            return Activator.CreateInstance(proxyType);
        }
    }

    public interface IService
    {
        string GetData();
        void ProcessData(string data);
    }

    public class BadDynamicTypeUsage
    {
        public static void Main()
        {
            var creator = new BadDynamicTypeCreator();

            // Tworzenie wielu typów prowadzi do memory leak (nie można unload)
            for (int i = 0; i < 100; i++)
            {
                Type dynamicType = creator.CreateDynamicType($"DynamicType{i}");
                object instance = Activator.CreateInstance(dynamicType);
            }

            // To spowoduje runtime error - brak implementacji interfejsu
            try
            {
                object proxy = creator.CreateProxy(typeof(IService));
                var service = (IService)proxy; // InvalidCastException lub runtime error
                service.GetData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
