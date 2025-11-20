using System;

namespace SOLID.LSP.BadExample
{
    /// <summary>
    /// Klasa bazowa reprezentująca ptaka
    /// </summary>
    public class Bird
    {
        public string Name { get; set; }

        public virtual void Fly()
        {
            Console.WriteLine($"{Name} leci w powietrzu!");
        }

        public void Eat()
        {
            Console.WriteLine($"{Name} je pokarm.");
        }
    }

    /// <summary>
    /// Naruszenie LSP: Pingwin NIE może latać, ale dziedziczy po Bird
    /// </summary>
    public class Penguin : Bird
    {
        public override void Fly()
        {
            // Pingwin nie może latać - musimy rzucić wyjątek lub nic nie robić
            throw new NotImplementedException("Pingwiny nie potrafią latać!");
        }
    }

    public class Sparrow : Bird
    {
        // Wróbel może latać - OK
    }

    public class Eagle : Bird
    {
        // Orzeł może latać - OK
    }

    // Program demonstracyjny
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("=== Test ptaków ===\n");

            var birds = new Bird[]
            {
                new Sparrow { Name = "Wróbel" },
                new Eagle { Name = "Orzeł" },
                new Penguin { Name = "Pingwin" }
            };

            // Problem: Nie możemy bezpiecznie użyć Fly() na wszystkich Birds!
            foreach (var bird in birds)
            {
                bird.Eat();
                
                try
                {
                    bird.Fly(); // To rzuci wyjątek dla Penguin!
                }
                catch (NotImplementedException ex)
                {
                    Console.WriteLine($"BŁĄD: {ex.Message}");
                }
                
                Console.WriteLine();
            }
        }

        // Funkcja przyjmująca Bird - założenie: każdy ptak może latać
        public static void MakeBirdFly(Bird bird)
        {
            Console.WriteLine($"Próba sprawienia by {bird.Name} poleciał...");
            bird.Fly(); // To może rzucić wyjątek!
        }
    }
}
