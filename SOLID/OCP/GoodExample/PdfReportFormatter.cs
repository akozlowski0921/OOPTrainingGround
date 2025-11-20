using System.Collections.Generic;
using System.Linq;

namespace SOLID.OCP.GoodExample
{
    public class PdfReportFormatter : IReportFormatter
    {
        public string Format(List<SalesData> data)
        {
            var total = data.Sum(d => d.Amount);
            return $"[PDF] Raport Sprzedaży\n" +
                   $"==================\n" +
                   $"Całkowita sprzedaż: {total:C}\n" +
                   $"Liczba transakcji: {data.Count}\n" +
                   $"[Koniec PDF]";
        }
    }
}
