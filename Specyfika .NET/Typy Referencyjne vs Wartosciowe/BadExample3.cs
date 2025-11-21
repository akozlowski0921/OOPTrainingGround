using System;
using System.Linq;

namespace SpecyfikaDotNet.TypesComparison.Bad3
{
    // ❌ BAD: Memory i performance anti-patterns

    // BŁĄD 1: Boxing w collections
    public class BadBoxingInCollections
    {
        public void AddItems()
        {
            var list = new System.Collections.ArrayList(); // Non-generic!
            
            for (int i = 0; i < 1000; i++)
            {
                list.Add(i); // ❌ Boxing każdego int
            }
        }
    }

    // BŁĄD 2: String concatenation w pętli
    public class BadStringConcatenation
    {
        public string BuildString(int count)
        {
            string result = "";
            for (int i = 0; i < count; i++)
            {
                result += i.ToString(); // ❌ Nowy string każdorazowo
            }
            return result;
        }
    }

    // BŁĄD 3: Niepotrzebne ToList/ToArray
    public class BadMaterialization
    {
        public int ProcessData(int[] data)
        {
            // ❌ Niepotrzebne ToList
            var list = data.ToList();
            return list.Count;
        }
    }

    // BŁĄD 4: Class zamiast struct dla małych immutable
    public class BadSmallClass
    {
        public int X { get; }
        public int Y { get; }
        
        public BadSmallClass(int x, int y)
        {
            X = x;
            Y = y;
        }
        // Powinno być readonly struct
    }
}
