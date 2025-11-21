using System.Collections.Generic;
using System.Linq;

namespace SOLID.SRP.Good3
{
    // ✅ Responsibility: Business calculations
    public class SalesCalculator
    {
        public SalesStatistics Calculate(List<SalesData> data)
        {
            return new SalesStatistics
            {
                TotalSales = data.Sum(s => s.Amount),
                AverageSale = data.Any() ? data.Average(s => s.Amount) : 0,
                MaximumSale = data.Any() ? data.Max(s => s.Amount) : 0,
                Count = data.Count
            };
        }
    }

    public class SalesStatistics
    {
        public decimal TotalSales { get; set; }
        public decimal AverageSale { get; set; }
        public decimal MaximumSale { get; set; }
        public int Count { get; set; }
    }
}
