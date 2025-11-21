using System;
using System.Collections.Generic;

namespace SpecyfikaDotNet.RecordsVsClasses.Bad2
{
    // ❌ BAD: Mutable class dla value objects - problemy z równością
    public class BadAddress
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }

    public class BadMoney
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }

    public class UsageExample
    {
        public void TestEquality()
        {
            var address1 = new BadAddress 
            { 
                Street = "Main St", 
                City = "Warsaw",
                ZipCode = "00-001",
                Country = "Poland"
            };

            var address2 = new BadAddress 
            { 
                Street = "Main St", 
                City = "Warsaw",
                ZipCode = "00-001",
                Country = "Poland"
            };

            // PROBLEM: Pomimo identycznych wartości, równość zwraca false
            if (address1 == address2) // False!
            {
                Console.WriteLine("Addresses are equal");
            }
            else
            {
                Console.WriteLine("Addresses are different"); // Wypisze to
            }

            // Dictionary z value objectami jako klucze
            var addressBook = new Dictionary<BadAddress, string>();
            addressBook[address1] = "John Doe";

            // PROBLEM: Nie znajdzie, bo address2 to inny obiekt referencyjny
            var name = addressBook.ContainsKey(address2) ? addressBook[address2] : "Not found";
            Console.WriteLine(name); // "Not found"
        }

        public void TestMutability()
        {
            var price = new BadMoney { Amount = 100, Currency = "USD" };
            var discountedPrice = price;

            // PROBLEM: Modyfikujemy współdzielony obiekt
            discountedPrice.Amount *= 0.8m;

            Console.WriteLine($"Original: {price.Amount}"); // 80 - zmienione!
            Console.WriteLine($"Discounted: {discountedPrice.Amount}"); // 80
        }
    }
}
