using System;
using System.Collections.Generic;
using System.Text;

namespace SOLID.SRP.Good3
{
    // ✅ Responsibility: HTML generation
    public class HtmlReportGenerator
    {
        public string Generate(DateTime startDate, DateTime endDate, 
            SalesStatistics stats, List<SalesData> data)
        {
            var html = new StringBuilder();
            html.Append("<html><body>");
            html.Append($"<h1>Sales Report</h1>");
            html.Append($"<p>Period: {startDate:d} - {endDate:d}</p>");
            html.Append($"<p>Total Sales: ${stats.TotalSales:N2}</p>");
            html.Append($"<p>Average Sale: ${stats.AverageSale:N2}</p>");
            html.Append($"<p>Maximum Sale: ${stats.MaximumSale:N2}</p>");
            html.Append("<table><tr><th>Date</th><th>Customer</th><th>Amount</th></tr>");
            
            foreach (var sale in data)
            {
                html.Append($"<tr><td>{sale.Date:d}</td><td>{sale.CustomerName}</td><td>${sale.Amount:N2}</td></tr>");
            }
            
            html.Append("</table></body></html>");
            return html.ToString();
        }
    }
}
