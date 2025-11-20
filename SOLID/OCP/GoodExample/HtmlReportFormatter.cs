using System.Collections.Generic;
using System.Linq;

namespace SOLID.OCP.GoodExample
{
    public class HtmlReportFormatter : IReportFormatter
    {
        public string Format(List<SalesData> data)
        {
            var total = data.Sum(d => d.Amount);
            return $"<html>\n" +
                   $"<body>\n" +
                   $"  <h1>Raport Sprzedaży</h1>\n" +
                   $"  <p>Całkowita sprzedaż: {total:C}</p>\n" +
                   $"  <p>Liczba transakcji: {data.Count}</p>\n" +
                   $"</body>\n" +
                   $"</html>";
        }
    }
}
