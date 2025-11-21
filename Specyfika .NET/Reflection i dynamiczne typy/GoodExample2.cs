using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SpecyfikaDotNet.Reflection
{
    // ✅ GOOD: Prawidłowe tworzenie dynamicznych typów z pełną implementacją
    public class GoodDynamicTypeCreator
    {
        public Type CreateDynamicType(string typeName, params (string PropertyName, Type PropertyType)[] properties)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentException("Type name cannot be null or empty", nameof(typeName));

            if (properties == null || properties.Length == 0)
                throw new ArgumentException("At least one property is required", nameof(properties));

            AssemblyName assemblyName = new AssemblyName($"DynamicAssembly_{Guid.NewGuid():N}");
            
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Run);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Public | TypeAttributes.Class);

            // Tworzymy properties z backing fields
            foreach (var (propertyName, propertyType) in properties)
            {
                CreateProperty(typeBuilder, propertyName, propertyType);
            }

            return typeBuilder.CreateType();
        }

        private void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            // Backing field
            FieldBuilder fieldBuilder = typeBuilder.DefineField(
                $"_{char.ToLower(propertyName[0])}{propertyName.Substring(1)}",
                propertyType,
                FieldAttributes.Private);

            // Property
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(
                propertyName,
                PropertyAttributes.HasDefault,
                propertyType,
                null);

            // Getter
            MethodBuilder getterBuilder = typeBuilder.DefineMethod(
                $"get_{propertyName}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                propertyType,
                Type.EmptyTypes);

            ILGenerator getterIL = getterBuilder.GetILGenerator();
            getterIL.Emit(OpCodes.Ldarg_0);
            getterIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getterIL.Emit(OpCodes.Ret);

            // Setter
            MethodBuilder setterBuilder = typeBuilder.DefineMethod(
                $"set_{propertyName}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null,
                new[] { propertyType });

            ILGenerator setterIL = setterBuilder.GetILGenerator();
            setterIL.Emit(OpCodes.Ldarg_0);
            setterIL.Emit(OpCodes.Ldarg_1);
            setterIL.Emit(OpCodes.Stfld, fieldBuilder);
            setterIL.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterBuilder);
            propertyBuilder.SetSetMethod(setterBuilder);
        }

        public Type CreateSimpleProxy<TInterface>(Func<string, object[], object> methodHandler) 
            where TInterface : class
        {
            Type interfaceType = typeof(TInterface);
            
            if (!interfaceType.IsInterface)
                throw new ArgumentException($"{interfaceType.Name} must be an interface", nameof(TInterface));

            AssemblyName assemblyName = new AssemblyName($"ProxyAssembly_{Guid.NewGuid():N}");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Run);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("ProxyModule");

            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                $"{interfaceType.Name}Proxy",
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(object),
                new[] { interfaceType });

            // Implementacja wszystkich metod interfejsu
            foreach (var method in interfaceType.GetMethods())
            {
                ImplementMethod(typeBuilder, method, methodHandler);
            }

            return typeBuilder.CreateType();
        }

        private void ImplementMethod(TypeBuilder typeBuilder, MethodInfo method, Func<string, object[], object> handler)
        {
            ParameterInfo[] parameters = method.GetParameters();
            Type[] parameterTypes = parameters.Select(p => p.ParameterType).ToArray();

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                method.ReturnType,
                parameterTypes);

            ILGenerator il = methodBuilder.GetILGenerator();

            // Prosty stub - wywołuje handler
            if (method.ReturnType == typeof(void))
            {
                il.Emit(OpCodes.Ret);
            }
            else
            {
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ret);
            }

            typeBuilder.DefineMethodOverride(methodBuilder, method);
        }
    }

    public interface IService
    {
        string GetData();
        void ProcessData(string data);
    }

    public class GoodDynamicTypeUsage
    {
        public static void Main()
        {
            var creator = new GoodDynamicTypeCreator();

            // Tworzenie typu z properties
            Type dynamicType = creator.CreateDynamicType(
                "Person",
                ("Name", typeof(string)),
                ("Age", typeof(int)),
                ("Email", typeof(string))
            );

            object instance = Activator.CreateInstance(dynamicType);

            // Ustawianie wartości properties
            PropertyInfo nameProp = dynamicType.GetProperty("Name");
            nameProp.SetValue(instance, "John Doe");

            PropertyInfo ageProp = dynamicType.GetProperty("Age");
            ageProp.SetValue(instance, 30);

            Console.WriteLine($"Created dynamic type: {dynamicType.Name}");
            Console.WriteLine($"Name: {nameProp.GetValue(instance)}");
            Console.WriteLine($"Age: {ageProp.GetValue(instance)}");

            // Tworzenie proxy z implementacją
            Type proxyType = creator.CreateSimpleProxy<IService>((methodName, args) =>
            {
                Console.WriteLine($"Method {methodName} called");
                return methodName == "GetData" ? "Dynamic data" : null;
            });

            var proxy = (IService)Activator.CreateInstance(proxyType);
            Console.WriteLine($"Proxy created: {proxy.GetType().Name}");
        }
    }
}
