using System;

namespace SpecyfikaDotNet.RecordsVsClasses
{
    // ❌ BAD: Mutable DTO - przypadkowa modyfikacja danych
    public class BadUserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class BadOrderDto
    {
        public int OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
    }

    public class BadExampleUsage
    {
        public void ProcessUser(BadUserDto user)
        {
            // PROBLEM 1: Przypadkowa modyfikacja obiektu
            var displayName = user.FirstName;
            user.FirstName = user.FirstName.ToUpper(); // Oryginał został zmieniony!
            
            Console.WriteLine($"User: {user.FirstName} {user.LastName}");
            // Wywołujący metodę nie spodziewa się, że user został zmodyfikowany
        }

        public void CalculateDiscount(BadOrderDto order)
        {
            if (order.TotalPrice > 1000)
            {
                // PROBLEM 2: Modyfikacja współdzielonego obiektu
                order.TotalPrice *= 0.9m; // 10% rabatu
            }
            
            // Inne części kodu widzą zmienioną cenę - możliwe błędy biznesowe
        }

        public bool AreUsersEqual(BadUserDto user1, BadUserDto user2)
        {
            // PROBLEM 3: Porównywanie referencji zamiast wartości
            return user1 == user2; // false, nawet jeśli dane są identyczne
            
            // Trzeba ręcznie implementować Equals i GetHashCode
        }

        public BadUserDto CreateUserCopy(BadUserDto original)
        {
            // PROBLEM 4: Ręczne kopiowanie wszystkich pól
            return new BadUserDto
            {
                Id = original.Id,
                FirstName = original.FirstName,
                LastName = original.LastName,
                Email = original.Email
                // Łatwo zapomnieć o nowym polu przy rozbudowie klasy
            };
        }
    }
}
