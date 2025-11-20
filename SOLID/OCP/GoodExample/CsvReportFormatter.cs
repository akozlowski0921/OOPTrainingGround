using System.Collections.Generic;
using System.Linq;

namespace SOLID.OCP.GoodExample
{
    public class CsvReportFormatter : IReportFormatter
    {
        public string Format(List<SalesData> data)
        {
            var total = data.Sum(d => d.Amount);
            var result = "Data,Product,Amount\n";
            foreach (var item in data)
            {
                result += $"{item.Date:yyyy-MM-dd},{item.Product},{item.Amount}\n";
            }
            result += $"TOTAL,{data.Count} transactions,{total:C}";
            return result;
        }
    }
}
