using System;

namespace SpecyfikaDotNet.Attributes
{
    // ✅ GOOD: Prawidłowa implementacja deep copy z dedykowanym interfejsem
    public interface IDeepCloneable<T>
    {
        // Generyczny interfejs z wyraźnym kontraktem
        T DeepClone();
    }

    public class GoodPerson : IDeepCloneable<GoodPerson>
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public GoodAddress Address { get; set; }

        // Deep clone - wszystkie referencje są kopiowane rekurencyjnie
        public GoodPerson DeepClone()
        {
            return new GoodPerson
            {
                Name = this.Name,
                Age = this.Age,
                // Deep clone zagnieżdżonego obiektu
                Address = this.Address?.DeepClone()
            };
        }
    }

    public class GoodAddress : IDeepCloneable<GoodAddress>
    {
        public string Street { get; set; }
        public string City { get; set; }

        public GoodAddress DeepClone()
        {
            return new GoodAddress
            {
                Street = this.Street,
                City = this.City
            };
        }
    }

    // ✅ GOOD: Użyteczny interfejs markerowy z kontraktem
    public interface IEntity<TKey>
    {
        // Kontrakt - każdy entity musi mieć ID
        TKey Id { get; set; }
        
        // Metoda do sprawdzenia czy to nowy entity
        bool IsTransient();
    }

    public class GoodProduct : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public bool IsTransient()
        {
            return Id == 0;
        }
    }

    // Extension methods dla IEntity
    public static class EntityExtensions
    {
        public static bool IsPersisted<TKey>(this IEntity<TKey> entity)
        {
            return !entity.IsTransient();
        }

        public static void ValidateId<TKey>(this IEntity<TKey> entity)
        {
            if (entity.IsTransient())
                throw new InvalidOperationException("Entity must have a valid ID");
        }
    }

    public class GoodCloneableUsage
    {
        public static void Main()
        {
            var person1 = new GoodPerson
            {
                Name = "John",
                Age = 30,
                Address = new GoodAddress
                {
                    Street = "Main St",
                    City = "New York"
                }
            };

            // Deep clone
            var person2 = person1.DeepClone();
            
            // Zmiana adresu w person2
            person2.Address.City = "Los Angeles";

            // ✅ person1.Address.City pozostaje niezmieniony
            Console.WriteLine($"Person1 city: {person1.Address.City}"); // New York - poprawnie!
            Console.WriteLine($"Person2 city: {person2.Address.City}"); // Los Angeles

            // Prawdziwa niezależna kopia
            person2.Name = "Jane";
            Console.WriteLine($"Person1 name: {person1.Name}"); // John
            Console.WriteLine($"Person2 name: {person2.Name}"); // Jane

            Console.WriteLine("\n=== IEntity with contract ===");

            // IEntity z kontraktem daje realne korzyści
            var product = new GoodProduct { Id = 0, Name = "Laptop", Price = 1000 };
            
            Console.WriteLine($"Is transient: {product.IsTransient()}"); // true
            Console.WriteLine($"Is persisted: {product.IsPersisted()}"); // false

            // Symulacja zapisu do bazy
            product.Id = 1;
            Console.WriteLine($"After save - Is transient: {product.IsTransient()}"); // false
            Console.WriteLine($"After save - Is persisted: {product.IsPersisted()}"); // true

            // Walidacja
            var newProduct = new GoodProduct { Name = "Mouse" };
            try
            {
                newProduct.ValidateId(); // Rzuci wyjątek
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Validation error: {ex.Message}");
            }

            // Generic repository pattern może wykorzystać IEntity
            Console.WriteLine("\n=== Generic operations ===");
            ProcessEntity(product);
        }

        // Metoda generyczna pracująca z IEntity
        private static void ProcessEntity<TEntity, TKey>(TEntity entity) 
            where TEntity : IEntity<TKey>
        {
            Console.WriteLine($"Processing entity with ID: {entity.Id}");
            Console.WriteLine($"Entity status: {(entity.IsTransient() ? "New" : "Existing")}");
        }
    }
}
