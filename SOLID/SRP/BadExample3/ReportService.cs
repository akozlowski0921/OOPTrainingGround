using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOLID.SRP.Bad3
{
    // ❌ BAD: Report service doing HTML generation, PDF conversion, email sending, and storage
    public class ReportService
    {
        private List<SalesData> _salesData = new List<SalesData>();

        public string GenerateAndSendReport(string recipientEmail, DateTime startDate, DateTime endDate)
        {
            // Responsibility 1: Data filtering
            var filteredData = _salesData
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .ToList();

            // Responsibility 2: Calculations
            var totalSales = filteredData.Sum(s => s.Amount);
            var avgSale = filteredData.Any() ? filteredData.Average(s => s.Amount) : 0;
            var maxSale = filteredData.Any() ? filteredData.Max(s => s.Amount) : 0;

            // Responsibility 3: HTML generation
            var html = new StringBuilder();
            html.Append("<html><body>");
            html.Append($"<h1>Sales Report</h1>");
            html.Append($"<p>Period: {startDate:d} - {endDate:d}</p>");
            html.Append($"<p>Total Sales: ${totalSales:N2}</p>");
            html.Append($"<p>Average Sale: ${avgSale:N2}</p>");
            html.Append($"<p>Maximum Sale: ${maxSale:N2}</p>");
            html.Append("<table><tr><th>Date</th><th>Customer</th><th>Amount</th></tr>");
            
            foreach (var sale in filteredData)
            {
                html.Append($"<tr><td>{sale.Date:d}</td><td>{sale.CustomerName}</td><td>${sale.Amount:N2}</td></tr>");
            }
            
            html.Append("</table></body></html>");
            var htmlContent = html.ToString();

            // Responsibility 4: PDF conversion (simulated)
            var pdfBytes = ConvertHtmlToPdf(htmlContent);

            // Responsibility 5: File storage
            var fileName = $"report_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf";
            System.IO.File.WriteAllBytes($"C:\\Reports\\{fileName}", pdfBytes);

            // Responsibility 6: Email sending
            SendEmail(recipientEmail, "Sales Report", htmlContent, pdfBytes);

            // Responsibility 7: Logging
            LogActivity($"Report generated and sent to {recipientEmail}");

            return fileName;
        }

        private byte[] ConvertHtmlToPdf(string html)
        {
            // Simulated PDF conversion
            return System.Text.Encoding.UTF8.GetBytes(html);
        }

        private void SendEmail(string to, string subject, string body, byte[] attachment)
        {
            // Simulated email sending
            Console.WriteLine($"Sending email to {to}");
        }

        private void LogActivity(string message)
        {
            System.IO.File.AppendAllText("activity.log", $"{DateTime.Now}: {message}\n");
        }
    }

    public class SalesData
    {
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
    }
}
