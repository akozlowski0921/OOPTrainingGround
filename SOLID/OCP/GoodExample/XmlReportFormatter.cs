using System.Collections.Generic;
using System.Linq;

namespace SOLID.OCP.GoodExample
{
    public class XmlReportFormatter : IReportFormatter
    {
        public string Format(List<SalesData> data)
        {
            var total = data.Sum(d => d.Amount);
            var result = "<?xml version=\"1.0\"?>\n<SalesReport>\n";
            result += $"  <Total>{total}</Total>\n";
            result += $"  <TransactionCount>{data.Count}</TransactionCount>\n";
            result += "  <Items>\n";
            foreach (var item in data)
            {
                result += $"    <Item date=\"{item.Date:yyyy-MM-dd}\" product=\"{item.Product}\" amount=\"{item.Amount}\" />\n";
            }
            result += "  </Items>\n</SalesReport>";
            return result;
        }
    }
}
