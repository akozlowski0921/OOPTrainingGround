using System;

namespace SOLID.LSP.GoodExample
{
    /// <summary>
    /// Klasa bazowa dla wszystkich ptaków - tylko wspólne zachowania
    /// </summary>
    public abstract class Bird
    {
        public string Name { get; set; }

        public void Eat()
        {
            Console.WriteLine($"{Name} je pokarm.");
        }

        public void Sleep()
        {
            Console.WriteLine($"{Name} śpi.");
        }
    }

    /// <summary>
    /// Interfejs dla ptaków, które potrafią latać
    /// </summary>
    public interface IFlyable
    {
        void Fly();
    }

    /// <summary>
    /// Interfejs dla ptaków, które potrafią pływać
    /// </summary>
    public interface ISwimmable
    {
        void Swim();
    }

    /// <summary>
    /// Wróbel - może latać
    /// </summary>
    public class Sparrow : Bird, IFlyable
    {
        public void Fly()
        {
            Console.WriteLine($"{Name} leci w powietrzu!");
        }
    }

    /// <summary>
    /// Orzeł - może latać
    /// </summary>
    public class Eagle : Bird, IFlyable
    {
        public void Fly()
        {
            Console.WriteLine($"{Name} szybuje majestycznie!");
        }
    }

    /// <summary>
    /// Pingwin - nie może latać, ale może pływać
    /// Żadnych wyjątków, żadnych pustych metod!
    /// </summary>
    public class Penguin : Bird, ISwimmable
    {
        public void Swim()
        {
            Console.WriteLine($"{Name} pływa zwinnie pod wodą!");
        }
    }

    /// <summary>
    /// Kaczka - może zarówno latać jak i pływać
    /// </summary>
    public class Duck : Bird, IFlyable, ISwimmable
    {
        public void Fly()
        {
            Console.WriteLine($"{Name} leci nisko nad wodą!");
        }

        public void Swim()
        {
            Console.WriteLine($"{Name} pływa po powierzchni wody!");
        }
    }

    // Program demonstracyjny
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("=== Test ptaków (zgodny z LSP) ===\n");

            var allBirds = new Bird[]
            {
                new Sparrow { Name = "Wróbel" },
                new Eagle { Name = "Orzeł" },
                new Penguin { Name = "Pingwin" },
                new Duck { Name = "Kaczka" }
            };

            // Bezpiecznie - wszystkie ptaki mogą jeść
            Console.WriteLine("--- Wszystkie ptaki jedzą ---");
            foreach (var bird in allBirds)
            {
                bird.Eat();
            }

            Console.WriteLine("\n--- Ptaki latające ---");
            var flyingBirds = new IFlyable[]
            {
                new Sparrow { Name = "Wróbel" },
                new Eagle { Name = "Orzeł" },
                new Duck { Name = "Kaczka" }
            };

            foreach (var flyingBird in flyingBirds)
            {
                flyingBird.Fly();
            }

            Console.WriteLine("\n--- Ptaki pływające ---");
            var swimmingBirds = new ISwimmable[]
            {
                new Penguin { Name = "Pingwin" },
                new Duck { Name = "Kaczka" }
            };

            foreach (var swimmingBird in swimmingBirds)
            {
                swimmingBird.Swim();
            }

            // Demonstracja bezpiecznego polimorfizmu
            Console.WriteLine("\n--- Polimorfizm bez niespodzianek ---");
            MakeFlyableFly(new Eagle { Name = "Orzeł Bielik" });
            MakeSwimmableSwim(new Penguin { Name = "Pingwin Cesarki" });
        }

        // Funkcja przyjmuje TYLKO ptaki, które mogą latać
        public static void MakeFlyableFly(IFlyable flyable)
        {
            flyable.Fly();
        }

        // Funkcja przyjmuje TYLKO ptaki, które mogą pływać
        public static void MakeSwimmableSwim(ISwimmable swimmable)
        {
            swimmable.Swim();
        }
    }
}
