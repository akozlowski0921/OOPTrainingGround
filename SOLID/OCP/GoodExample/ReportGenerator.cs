using System;
using System.Collections.Generic;

namespace SOLID.OCP.GoodExample
{
    /// <summary>
    /// Generator raportów zgodny z OCP
    /// Otwarty na rozszerzenia (nowe formattery), zamknięty na modyfikacje
    /// </summary>
    public class ReportGenerator
    {
        private readonly IReportFormatter _formatter;

        public ReportGenerator(IReportFormatter formatter)
        {
            _formatter = formatter;
        }

        public string GenerateReport(List<SalesData> data)
        {
            return _formatter.Format(data);
        }
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

            // PDF Report
            Console.WriteLine("=== PDF Report ===");
            var pdfGenerator = new ReportGenerator(new PdfReportFormatter());
            Console.WriteLine(pdfGenerator.GenerateReport(data));

            // HTML Report
            Console.WriteLine("\n=== HTML Report ===");
            var htmlGenerator = new ReportGenerator(new HtmlReportFormatter());
            Console.WriteLine(htmlGenerator.GenerateReport(data));

            // CSV Report
            Console.WriteLine("\n=== CSV Report ===");
            var csvGenerator = new ReportGenerator(new CsvReportFormatter());
            Console.WriteLine(csvGenerator.GenerateReport(data));

            // XML Report
            Console.WriteLine("\n=== XML Report ===");
            var xmlGenerator = new ReportGenerator(new XmlReportFormatter());
            Console.WriteLine(xmlGenerator.GenerateReport(data));

            // JSON Report - nowy format dodany BEZ modyfikacji istniejącego kodu!
            Console.WriteLine("\n=== JSON Report (nowy format!) ===");
            var jsonGenerator = new ReportGenerator(new JsonReportFormatter());
            Console.WriteLine(jsonGenerator.GenerateReport(data));
        }
    }
}
