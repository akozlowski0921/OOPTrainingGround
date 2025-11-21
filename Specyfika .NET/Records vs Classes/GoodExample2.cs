using System;
using System.Collections.Generic;

namespace SpecyfikaDotNet.RecordsVsClasses.Good2
{
    // ✅ GOOD: Record dla value objects - wartościowa równość
    public record GoodAddress(
        string Street,
        string City,
        string ZipCode,
        string Country
    );

    public record GoodMoney(decimal Amount, string Currency);

    public class UsageExample
    {
        public void TestEquality()
        {
            var address1 = new GoodAddress("Main St", "Warsaw", "00-001", "Poland");
            var address2 = new GoodAddress("Main St", "Warsaw", "00-001", "Poland");

            // ✅ Record ma wartościową równość out-of-the-box
            if (address1 == address2) // True!
            {
                Console.WriteLine("Addresses are equal"); // Wypisze to
            }

            // Dictionary z records jako klucze działa poprawnie
            var addressBook = new Dictionary<GoodAddress, string>();
            addressBook[address1] = "John Doe";

            // ✅ Znajdzie, bo address2 ma te same wartości
            var name = addressBook.ContainsKey(address2) ? addressBook[address2] : "Not found";
            Console.WriteLine(name); // "John Doe"

            // ✅ GetHashCode też działa poprawnie
            Console.WriteLine($"Hash1: {address1.GetHashCode()}");
            Console.WriteLine($"Hash2: {address2.GetHashCode()}"); // Ten sam hash
        }

        public void TestImmutability()
        {
            var price = new GoodMoney(100, "USD");

            // ✅ With expression tworzy nową kopię
            var discountedPrice = price with { Amount = price.Amount * 0.8m };

            Console.WriteLine($"Original: {price.Amount}"); // 100 - nie zmienione!
            Console.WriteLine($"Discounted: {discountedPrice.Amount}"); // 80
        }

        public void TestDeconstruction()
        {
            var address = new GoodAddress("Main St", "Warsaw", "00-001", "Poland");

            // ✅ Record wspiera dekonstrukcję
            var (street, city, zip, country) = address;
            Console.WriteLine($"{street}, {city}, {zip}, {country}");
        }

        public void TestInheritance()
        {
            // ✅ Records wspierają dziedziczenie
            var homeAddress = new HomeAddress("Main St", "Warsaw", "00-001", "Poland", "Apt 5");
            Console.WriteLine(homeAddress);
        }
    }

    // Dziedziczenie z record
    public record HomeAddress(
        string Street,
        string City,
        string ZipCode,
        string Country,
        string ApartmentNumber
    ) : GoodAddress(Street, City, ZipCode, Country);
}
