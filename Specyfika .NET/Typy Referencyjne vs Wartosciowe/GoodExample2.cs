using System;
using System.Collections.Generic;

namespace SpecyfikaDotNet.TypesComparison.Good2
{
    // ✅ GOOD: Proper struct usage

    // ✅ Small readonly struct (≤16 bytes)
    public readonly struct GoodPoint
    {
        public int X { get; }
        public int Y { get; }

        public GoodPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public GoodPoint Move(int dx, int dy) =>
            new GoodPoint(X + dx, Y + dy);
    }

    // ✅ In parameter dla większych struct
    public class GoodStructParameters
    {
        public readonly struct LargeStruct
        {
            public long F1 { get; }
            public long F2 { get; }
            public long F3 { get; }
            public long F4 { get; }
        }

        public void ProcessLarge(in LargeStruct data)
        {
            // ✅ Readonly reference - zero copy
            Console.WriteLine(data.F1);
        }
    }

    // ✅ Generic collections - no boxing
    public class GoodListPerformance
    {
        public void UseGenerics()
        {
            var list = new List<int>(); // ✅ No boxing
            
            for (int i = 0; i < 1000; i++)
            {
                list.Add(i);
            }
        }
    }

    // ✅ Stackalloc dla hot path
    public class GoodArrayAllocation
    {
        public void Process()
        {
            // ✅ Stack allocation - zero GC
            Span<int> buffer = stackalloc int[100];
            
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = i;
            }
        }
    }

    // ✅ Span<char> dla string operations
    public class GoodStringHandling
    {
        public void ProcessText(ReadOnlySpan<char> text)
        {
            // ✅ Zero allocation slicing
            var part1 = text.Slice(0, 10);
            var part2 = text.Slice(10, 10);
        }
    }
}
