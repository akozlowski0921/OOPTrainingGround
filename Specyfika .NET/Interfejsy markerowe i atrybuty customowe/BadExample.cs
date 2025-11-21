using System;

namespace SpecyfikaDotNet.Attributes
{
    // ❌ BAD: Nieprawidłowa implementacja ICloneable - shallow copy problem
    public class BadPerson : ICloneable
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public BadAddress Address { get; set; }

        // Problem: shallow copy - referencje są współdzielone
        public object Clone()
        {
            // MemberwiseClone tworzy płytką kopię
            // Address jest współdzielony między oryginałem a klonem!
            return this.MemberwiseClone();
        }
    }

    public class BadAddress
    {
        public string Street { get; set; }
        public string City { get; set; }
    }

    // ❌ BAD: Nieużyteczny interfejs markerowy bez kontraktu
    public interface IEntity
    {
        // Pusty interfejs - brak kontraktu, brak funkcjonalności
        // Nie wiadomo co to znaczy być "Entity"
    }

    public class BadProduct : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // IEntity nic nie wnosi - tylko "znacznik"
    }

    public class BadCloneableUsage
    {
        public static void Main()
        {
            var person1 = new BadPerson
            {
                Name = "John",
                Age = 30,
                Address = new BadAddress
                {
                    Street = "Main St",
                    City = "New York"
                }
            };

            // Klonowanie
            var person2 = (BadPerson)person1.Clone();
            
            // Zmiana adresu w person2
            person2.Address.City = "Los Angeles";

            // PROBLEM: person1.Address.City też się zmienił!
            Console.WriteLine($"Person1 city: {person1.Address.City}"); // Los Angeles - źle!
            Console.WriteLine($"Person2 city: {person2.Address.City}"); // Los Angeles

            // Shallow copy powoduje współdzielenie referencji
            // Zmiana w jednym obiekcie wpływa na drugi
            // To nie jest prawdziwy klon!

            // IEntity nie daje żadnych korzyści
            var product = new BadProduct { Id = 1, Name = "Laptop" };
            bool isEntity = product is IEntity; // true, ale co z tego?
            Console.WriteLine($"Is entity: {isEntity}"); // Bezużyteczna informacja
        }
    }
}
