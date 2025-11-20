using System;
using System.Collections.Generic;
using System.Linq;

namespace SOLID.OCP.BadExample
{
    /// <summary>
    /// Naruszenie OCP: Dodanie nowego formatu raportu wymaga modyfikacji istniejącego kodu
    /// </summary>
    public class ReportGenerator
    {
        public string GenerateReport(List<SalesData> data, string format)
        {
            // Potężny switch - każdy nowy format wymaga dodania case'a
            switch (format.ToUpper())
            {
                case "PDF":
                    return GeneratePdfReport(data);
                
                case "HTML":
                    return GenerateHtmlReport(data);
                
                case "CSV":
                    return GenerateCsvReport(data);
                
                case "XML":
                    return GenerateXmlReport(data);
                
                default:
                    throw new ArgumentException($"Nieobsługiwany format: {format}");
            }
        }

        private string GeneratePdfReport(List<SalesData> data)
        {
            var total = data.Sum(d => d.Amount);
            return $"[PDF] Raport Sprzedaży\n" +
                   $"==================\n" +
                   $"Całkowita sprzedaż: {total:C}\n" +
                   $"Liczba transakcji: {data.Count}\n" +
                   $"[Koniec PDF]";
        }

        private string GenerateHtmlReport(List<SalesData> data)
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

        private string GenerateCsvReport(List<SalesData> data)
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

        private string GenerateXmlReport(List<SalesData> data)
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

    public class SalesData
    {
        public DateTime Date { get; set; }
        public string Product { get; set; }
        public decimal Amount { get; set; }
    }

    // Program demonstracyjny
    public class Program
    {
        public static void Main()
        {
            var data = new List<SalesData>
            {
                new SalesData { Date = DateTime.Now.AddDays(-2), Product = "Laptop", Amount = 1500.00m },
                new SalesData { Date = DateTime.Now.AddDays(-1), Product = "Mouse", Amount = 25.00m },
                new SalesData { Date = DateTime.Now, Product = "Keyboard", Amount = 75.00m }
            };

            var generator = new ReportGenerator();

            Console.WriteLine("=== PDF Report ===");
            Console.WriteLine(generator.GenerateReport(data, "PDF"));

            Console.WriteLine("\n=== HTML Report ===");
            Console.WriteLine(generator.GenerateReport(data, "HTML"));

            Console.WriteLine("\n=== CSV Report ===");
            Console.WriteLine(generator.GenerateReport(data, "CSV"));

            // Próba użycia nieobsługiwanego formatu
            try
            {
                Console.WriteLine("\n=== JSON Report (nieobsługiwany) ===");
                Console.WriteLine(generator.GenerateReport(data, "JSON"));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
            }
        }
    }
}
