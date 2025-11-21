using System;
using System.Collections.Generic;

namespace SpecyfikaDotNet.TypesComparison.Bad2
{
    // ❌ BAD: Struct misuse patterns

    // BŁĄD 1: Duży mutable struct
    public struct BadLargeMutableStruct
    {
        public int Field1 { get; set; }
        public int Field2 { get; set; }
        public int Field3 { get; set; }
        public long Field4 { get; set; }
        public long Field5 { get; set; }
        // 32 bytes - za duży, kopiowany przy każdym wywołaniu

        public void Increment()
        {
            Field1++; // Modyfikuje kopię!
        }
    }

    // BŁĄD 2: Struct jako parametr bez in
    public class BadStructParameters
    {
        public void ProcessLarge(BadLargeMutableStruct data)
        {
            // ❌ Cały struct kopiowany (32 bytes)
            Console.WriteLine(data.Field1);
        }
    }

    // BŁĄD 3: List<ValueType> vs List<ReferenceType>
    public class BadListPerformance
    {
        public void ComparePerformance()
        {
            // ValueType - dobra locality
            var structList = new List<SimpleStruct>();
            
            // ❌ Ale boxing przy używaniu jako object
            foreach (SimpleStruct item in structList)
            {
                object boxed = item; // Boxing!
            }
        }
    }

    public struct SimpleStruct
    {
        public int Value { get; set; }
    }

    // BŁĄD 4: Struct z default constructor side effects
    public struct BadStructWithConstructor
    {
        public int Value { get; set; }
        
        // ❌ Constructor może nie być wywołany
        public BadStructWithConstructor()
        {
            Value = 42; // Może nie działać jak oczekujesz
        }
    }

    // BŁĄD 5: Brak użycia stackalloc
    public class BadArrayAllocation
    {
        public void Process()
        {
            // ❌ Heap allocation
            int[] buffer = new int[100];
            
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = i;
            }
        }
    }

    // BŁĄD 6: Niepotrzebne kopiowanie stringów
    public class BadStringHandling
    {
        public string ProcessText(string text)
        {
            // ❌ Wiele substring calls - alokacje
            var result = text.Substring(0, 10);
            result += text.Substring(10, 10);
            return result;
        }
    }
}
