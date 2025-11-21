using System.IO;

namespace SOLID.SRP.Good2
{
    // ✅ GOOD: Odpowiedzialność - tylko generowanie raportów
    public class ReportGenerator
    {
        private readonly OrderRepository _repository;
        private readonly Logger _logger;

        public ReportGenerator(OrderRepository repository, Logger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public string GenerateMonthlyReport(int month, int year)
        {
            var orders = _repository.GetByMonthAndYear(month, year);
            var totalRevenue = orders.Sum(o => o.TotalAmount);

            var report = $"Monthly Report {month}/{year}\n";
            report += $"Total Orders: {orders.Count}\n";
            report += $"Total Revenue: {totalRevenue:C}\n";

            File.WriteAllText($"report_{month}_{year}.txt", report);
            _logger.Log($"Report generated for {month}/{year}");

            return report;
        }
    }
}
