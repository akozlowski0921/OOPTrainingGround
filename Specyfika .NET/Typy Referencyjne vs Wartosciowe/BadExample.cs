using System;
using System.Collections.Generic;

namespace SpecyfikaDotNet.TypesComparison
{
    // ❌ BAD: Nieprawidłowe użycie typów wartościowych i referencyjnych

    // BŁĄD 1: Boxing/Unboxing - performance hit
    public class BadPerformanceService
    {
        public void ProcessItems(List<int> items)
        {
            foreach (int item in items)
            {
                // Boxing - konwersja int do object
                object boxed = item; // ❌ Alokacja na heapie
                Console.WriteLine(boxed);
                
                // Unboxing - konwersja object do int
                int unboxed = (int)boxed; // ❌ Performance cost
            }
        }

        // BŁĄD 2: List<object> zamiast generics - boxing
        public double CalculateAverage(List<object> numbers)
        {
            double sum = 0;
            foreach (object num in numbers)
            {
                sum += (double)num; // Unboxing przy każdej iteracji
            }
            return sum / numbers.Count;
        }
    }

    // BŁĄD 3: Mutable struct - nieprzewidywalne zachowanie
    public struct BadMutablePoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public void Move(int deltaX, int deltaY)
        {
            X += deltaX;
            Y += deltaY;
        }
    }

    // BŁĄD 4: Duży struct - kopiowany przy każdym wywołaniu
    public struct BadLargeStruct
    {
        public int Field1 { get; set; }
        public int Field2 { get; set; }
        public int Field3 { get; set; }
        public int Field4 { get; set; }
        public long Field5 { get; set; }
        public long Field6 { get; set; }
        public double Field7 { get; set; }
        public double Field8 { get; set; }
        // 48 bytes - za duży dla struct (powinno być max 16 bytes)
    }

    // BŁĄD 5: Struct z referencjami - mieszany typ
    public struct BadStructWithReferences
    {
        public int Id { get; set; }
        public string Name { get; set; } // Referencja w struct
        public List<int> Items { get; set; } // Referencja w struct
        // Problemy z kopiowaniem - tylko referencje są kopiowane, nie obiekty
    }

    // BŁĄD 6: Brak użycia readonly struct
    public struct BadNonReadonlyStruct
    {
        public int X { get; }
        public int Y { get; }

        public BadNonReadonlyStruct(int x, int y)
        {
            X = x;
            Y = y;
        }

        // Struct wygląda na immutable, ale brak readonly - defensive copy
    }

    // BŁĄD 7: Niepotrzebne kopiowanie dużych struktur
    public class BadStructUsage
    {
        public void ProcessPoint(BadLargeStruct point)
        {
            // point jest kopiowany przy wywołaniu - 48 bytes kopiowane
            Console.WriteLine(point.Field1);
        }

        public BadLargeStruct CreatePoint()
        {
            var point = new BadLargeStruct { Field1 = 1 };
            // point jest kopiowany przy zwracaniu - kolejne 48 bytes
            return point;
        }
    }

    // BŁĄD 8: Brak użycia Span<T> dla slice operations
    public class BadArrayProcessing
    {
        public int[] GetSubArray(int[] source, int start, int length)
        {
            var result = new int[length];
            Array.Copy(source, start, result, 0, length);
            // ❌ Nowa alokacja i kopiowanie danych
            return result;
        }

        public void ProcessSubArray(int[] source, int start, int length)
        {
            for (int i = start; i < start + length; i++)
            {
                // Bezpośredni dostęp do tablicy - bounds checking przy każdej iteracji
                Console.WriteLine(source[i]);
            }
        }
    }
}
