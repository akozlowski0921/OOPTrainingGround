using System.Collections.Generic;
using System.Linq;

namespace SOLID.OCP.GoodExample
{
    /// <summary>
    /// Nowy formatter dodany BEZ modyfikacji istniejącego kodu!
    /// To jest siła OCP - rozszerzalność bez modyfikacji
    /// </summary>
    public class JsonReportFormatter : IReportFormatter
    {
        public string Format(List<SalesData> data)
        {
            var total = data.Sum(d => d.Amount);
            var result = "{\n";
            result += $"  \"totalSales\": {total},\n";
            result += $"  \"transactionCount\": {data.Count},\n";
            result += "  \"items\": [\n";
            
            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                result += $"    {{\"date\": \"{item.Date:yyyy-MM-dd}\", \"product\": \"{item.Product}\", \"amount\": {item.Amount}}}";
                if (i < data.Count - 1)
                    result += ",";
                result += "\n";
            }
            
            result += "  ]\n}";
            return result;
        }
    }
}
