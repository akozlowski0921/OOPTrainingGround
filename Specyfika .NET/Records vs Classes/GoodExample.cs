using System;

namespace SpecyfikaDotNet.RecordsVsClasses
{
    // ✅ GOOD: Record zapewnia immutability i value-based equality
    public record GoodUserDto(int Id, string FirstName, string LastName, string Email);

    // Alternatywnie: record z właściwościami (C# 9+)
    public record GoodOrderDto
    {
        public int OrderId { get; init; } // init - można ustawić tylko w konstruktorze/inicjalizatorze
        public decimal TotalPrice { get; init; }
        public DateTime OrderDate { get; init; }
        public string Status { get; init; }
    }

    public class GoodExampleUsage
    {
        public void ProcessUser(GoodUserDto user)
        {
            // Nie można zmodyfikować - bezpieczne
            var displayName = user.FirstName.ToUpper();
            
            // user.FirstName = "New"; // ❌ Błąd kompilacji!
            
            // Jeśli potrzeba zmian, tworzymy nową instancję z 'with'
            var modifiedUser = user with { FirstName = user.FirstName.ToUpper() };
            
            Console.WriteLine($"Original: {user.FirstName}, Modified: {modifiedUser.FirstName}");
            // Oryginał pozostaje niezmieniony
        }

        public GoodOrderDto CalculateDiscount(GoodOrderDto order)
        {
            if (order.TotalPrice > 1000)
            {
                // Zwracamy nową instancję z rabatem
                return order with { TotalPrice = order.TotalPrice * 0.9m };
            }
            
            return order; // Oryginał nigdy nie jest modyfikowany
        }

        public bool AreUsersEqual(GoodUserDto user1, GoodUserDto user2)
        {
            // Record automatycznie implementuje value-based equality
            return user1 == user2; // true, jeśli wszystkie pola są równe
            
            // Automatycznie zaimplementowane:
            // - Equals()
            // - GetHashCode()
            // - operator ==
            // - operator !=
        }

        public GoodUserDto CreateUserCopy(GoodUserDto original)
        {
            // with expression - kopiuje wszystkie pola automatycznie
            var copy = original with { }; // Płytka kopia
            
            // Lub z modyfikacją wybranych pól
            var modified = original with { Email = "newemail@example.com" };
            
            return copy;
        }

        public void DemonstrateBehavior()
        {
            var user1 = new GoodUserDto(1, "John", "Doe", "john@example.com");
            var user2 = new GoodUserDto(1, "John", "Doe", "john@example.com");
            
            Console.WriteLine(user1 == user2); // true - value-based equality
            Console.WriteLine(user1.GetHashCode() == user2.GetHashCode()); // true
            
            // ToString() automatycznie generowany
            Console.WriteLine(user1); // GoodUserDto { Id = 1, FirstName = John, ... }
            
            // Deconstruction
            var (id, firstName, lastName, email) = user1;
            Console.WriteLine($"ID: {id}, Name: {firstName} {lastName}");
        }
    }

    // Record może dziedziczyć z innego record
    public record PersonDto(string FirstName, string LastName);
    public record EmployeeDto(string FirstName, string LastName, string EmployeeId) 
        : PersonDto(FirstName, LastName);

    // Record może mieć dodatkowe metody
    public record ProductDto(int Id, string Name, decimal Price)
    {
        public decimal PriceWithTax => Price * 1.23m;
        
        public bool IsExpensive() => Price > 1000;
    }
}
