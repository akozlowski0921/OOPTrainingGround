using System;

namespace SpecyfikaDotNet.RecordsVsClasses.Good3
{
    // ✅ GOOD: Record automatycznie generuje Equals, GetHashCode, ToString
    public record GoodPoint(int X, int Y);

    public record GoodConfiguration(
        string Server,
        int Port,
        string Database,
        string Username
    );

    public class UsageExample
    {
        public void TestPoint()
        {
            var point1 = new GoodPoint(10, 20);
            var point2 = new GoodPoint(10, 20);
            var point3 = new GoodPoint(10, 30);

            // ✅ Równość działa poprawnie
            Console.WriteLine(point1 == point2); // True
            Console.WriteLine(point1 == point3); // False
            Console.WriteLine(point1.Equals(point2)); // True

            // ✅ GetHashCode działa poprawnie
            Console.WriteLine(point1.GetHashCode() == point2.GetHashCode()); // True

            // ✅ ToString wygenerowany automatycznie
            Console.WriteLine(point1); // GoodPoint { X = 10, Y = 20 }
        }

        public void TestConfiguration()
        {
            var config = new GoodConfiguration("localhost", 5432, "mydb", "admin");

            // ✅ With expression dla "klonowania" z modyfikacją
            var devConfig = config with { Database = "mydb_dev" };
            var testConfig = config with { Server = "test-server", Port = 5433 };

            Console.WriteLine(config);
            Console.WriteLine(devConfig);
            Console.WriteLine(testConfig);

            // ✅ Immutable - oryginał nie zmieniony
            Console.WriteLine($"Original database: {config.Database}"); // "mydb"
        }

        public void TestPattern matching()
        {
            var point = new GoodPoint(5, 10);

            // ✅ Records wspierają pattern matching
            var description = point switch
            {
                { X: 0, Y: 0 } => "Origin",
                { X: > 0, Y: > 0 } => "First quadrant",
                { X: < 0, Y: > 0 } => "Second quadrant",
                { X: < 0, Y: < 0 } => "Third quadrant",
                { X: > 0, Y: < 0 } => "Fourth quadrant",
                _ => "On axis"
            };

            Console.WriteLine(description); // "First quadrant"
        }
    }

    // ✅ Records z dodatkowymi członkami
    public record Shape(string Name)
    {
        public virtual double Area => 0;
    }

    public record Circle(string Name, double Radius) : Shape(Name)
    {
        public override double Area => Math.PI * Radius * Radius;
    }

    public record Rectangle(string Name, double Width, double Height) : Shape(Name)
    {
        public override double Area => Width * Height;
    }
}
