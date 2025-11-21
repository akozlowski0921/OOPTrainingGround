using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace SpecyfikaDotNet.TypesComparison.Good3
{
    // ✅ GOOD: Memory optimization patterns

    // ✅ StringBuilder zamiast concatenation
    public class GoodStringConcatenation
    {
        public string BuildString(int count)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                sb.Append(i);
            }
            return sb.ToString();
        }
    }

    // ✅ IEnumerable zamiast materializacji
    public class GoodMaterialization
    {
        public int ProcessData(IEnumerable<int> data)
        {
            // ✅ Lazy evaluation - no materialization
            return data.Count();
        }
    }

    // ✅ Readonly struct dla value objects
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
                throw new InvalidOperationException();
            return new Money(Amount + other.Amount, Currency);
        }
    }

    // ✅ ArrayPool dla reusable buffers
    public class GoodArrayPool
    {
        public void ProcessWithPool()
        {
            var pool = System.Buffers.ArrayPool<byte>.Shared;
            var buffer = pool.Rent(1024);
            
            try
            {
                // Use buffer
            }
            finally
            {
                pool.Return(buffer);
            }
        }
    }

    // ✅ Memory<T> dla async scenarios
    public class GoodMemoryUsage
    {
        public async System.Threading.Tasks.Task ProcessAsync(Memory<byte> data)
        {
            await System.Threading.Tasks.Task.Delay(100);
            // Memory<T> works in async
        }
    }
}
