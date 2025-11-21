using System;
using System.Collections.Generic;

namespace SpecyfikaDotNet.TypesComparison
{
    // ✅ GOOD: Prawidłowe użycie typów wartościowych i referencyjnych

    // ✅ Unikanie boxingu przez użycie generics
    public class GoodPerformanceService
    {
        public void ProcessItems<T>(List<T> items)
        {
            foreach (T item in items)
            {
                // Brak boxing - T pozostaje jako value type
                Console.WriteLine(item);
            }
        }

        // ✅ Generics zamiast object - brak boxing
        public double CalculateAverage(List<double> numbers)
        {
            double sum = 0;
            foreach (double num in numbers)
            {
                sum += num; // Brak unboxing
            }
            return sum / numbers.Count;
        }
    }

    // ✅ Readonly struct - immutable i thread-safe
    public readonly struct GoodPoint
    {
        public int X { get; }
        public int Y { get; }

        public GoodPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        // Metody zwracają nową instancję zamiast modyfikować
        public GoodPoint Move(int deltaX, int deltaY)
        {
            return new GoodPoint(X + deltaX, Y + deltaY);
        }
    }

    // ✅ Record struct (C# 10+) - immutable by default
    public readonly record struct GoodPointRecord(int X, int Y)
    {
        public GoodPointRecord Move(int deltaX, int deltaY) =>
            new(X + deltaX, Y + deltaY);
    }

    // ✅ Class dla dużych danych - unikanie kopiowania
    public class GoodLargeData
    {
        public int Field1 { get; set; }
        public int Field2 { get; set; }
        public int Field3 { get; set; }
        public int Field4 { get; set; }
        public long Field5 { get; set; }
        public long Field6 { get; set; }
        public double Field7 { get; set; }
        public double Field8 { get; set; }
        // Class - przekazywana przez referencję, brak kopiowania
    }

    // ✅ Readonly struct - małe, immutable
    public readonly struct Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Different currencies");

            return new Money(Amount + other.Amount, Currency);
        }
    }

    // ✅ Ref struct dla stack-only scenarios
    public ref struct GoodStackOnlyBuffer
    {
        private Span<byte> _buffer;

        public GoodStackOnlyBuffer(Span<byte> buffer)
        {
            _buffer = buffer;
        }

        public void Write(byte value, int index)
        {
            _buffer[index] = value;
        }

        // ref struct nie może być boxowany, używany w async, lub przechowywany w klasie
        // Gwarantuje stack allocation - zero heap allocations
    }

    // ✅ Span<T> dla wydajnego przetwarzania bez alokacji
    public class GoodArrayProcessing
    {
        public ReadOnlySpan<int> GetSubArray(int[] source, int start, int length)
        {
            // Span - zero allocation, widok na fragment tablicy
            return source.AsSpan(start, length);
        }

        public void ProcessSubArray(ReadOnlySpan<int> data)
        {
            // Span - zoptymalizowany dostęp, bounds checking raz
            foreach (var item in data)
            {
                Console.WriteLine(item);
            }
        }

        // Span<T> dla string manipulation bez alokacji
        public int CountVowels(ReadOnlySpan<char> text)
        {
            int count = 0;
            foreach (char c in text)
            {
                if ("aeiouAEIOU".Contains(c))
                    count++;
            }
            return count;
        }
    }

    // ✅ In parameter dla readonly pass-by-reference (duże struktury)
    public readonly struct LargeValueStruct
    {
        public int Field1 { get; }
        public int Field2 { get; }
        public long Field3 { get; }
        public long Field4 { get; }

        public LargeValueStruct(int f1, int f2, long f3, long f4)
        {
            Field1 = f1;
            Field2 = f2;
            Field3 = f3;
            Field4 = f4;
        }
    }

    public class GoodStructPassage
    {
        public void ProcessLargeStruct(in LargeValueStruct data)
        {
            // 'in' - readonly reference, brak kopiowania
            Console.WriteLine(data.Field1);
            // data.Field1 = 10; // ❌ Błąd kompilacji - readonly
        }

        // Ref return dla modyfikacji bez kopiowania
        public ref int GetElement(int[] array, int index)
        {
            return ref array[index];
        }
    }

    // ✅ Memory<T> i ReadOnlyMemory<T> dla async scenarios
    public class GoodMemoryUsage
    {
        public async Task<int> ProcessAsync(ReadOnlyMemory<byte> data)
        {
            // Memory<T> może być używany w async (w przeciwieństwie do Span<T>)
            await Task.Delay(100);
            return data.Length;
        }

        public ReadOnlyMemory<char> GetSubstring(string text, int start, int length)
        {
            // Zero allocation slice
            return text.AsMemory(start, length);
        }
    }

    // ✅ Kiedy używać struct vs class
    public static class TypeSelectionGuidelines
    {
        // Używaj STRUCT gdy:
        // - Typ jest mały (≤ 16 bytes)
        // - Immutable (readonly struct lub record struct)
        // - Wartościowa semantyka (kopiowanie ma sens)
        // - Używany w kolekcjach (brak indirection)
        // Przykład: Point, Money, GUID, DateTime

        // Używaj CLASS gdy:
        // - Typ jest duży (> 16 bytes)
        // - Mutable state
        // - Potrzeba dziedziczenia
        // - Referencjna semantyka (współdzielenie stanu)
        // Przykład: Entity, ViewModel, Service
    }

    // ✅ Przykład użycia Span<T> vs tradycyjne podejście
    public class SpanPerformanceComparison
    {
        // Tradycyjne - wiele alokacji
        public string[] SplitTraditional(string text)
        {
            return text.Split(' '); // Alokacja tablicy stringów
        }

        // Span - zero allocations
        public void ProcessWordsWithSpan(ReadOnlySpan<char> text)
        {
            int start = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                {
                    var word = text.Slice(start, i - start);
                    // Process word - zero allocation
                    start = i + 1;
                }
            }
        }
    }
}
