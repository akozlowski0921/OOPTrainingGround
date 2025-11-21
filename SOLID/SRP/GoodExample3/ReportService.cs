using System;

namespace SOLID.SRP.Good3
{
    // ✅ Orchestration: Coordinates different services
    public class ReportService
    {
        private readonly SalesDataRepository _repository;
        private readonly SalesCalculator _calculator;
        private readonly HtmlReportGenerator _htmlGenerator;
        private readonly PdfConverter _pdfConverter;
        private readonly ReportStorage _storage;
        private readonly EmailSender _emailSender;
        private readonly ActivityLogger _logger;

        public ReportService(
            SalesDataRepository repository,
            SalesCalculator calculator,
            HtmlReportGenerator htmlGenerator,
            PdfConverter pdfConverter,
            ReportStorage storage,
            EmailSender emailSender,
            ActivityLogger logger)
        {
            _repository = repository;
            _calculator = calculator;
            _htmlGenerator = htmlGenerator;
            _pdfConverter = pdfConverter;
            _storage = storage;
            _emailSender = emailSender;
            _logger = logger;
        }

        public string GenerateAndSendReport(string recipientEmail, DateTime startDate, DateTime endDate)
        {
            var data = _repository.GetByDateRange(startDate, endDate);
            var stats = _calculator.Calculate(data);
            var html = _htmlGenerator.Generate(startDate, endDate, stats, data);
            var pdf = _pdfConverter.ConvertHtmlToPdf(html);
            
            var fileName = $"report_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf";
            _storage.Save(fileName, pdf);
            _emailSender.SendReportEmail(recipientEmail, "Sales Report", html, pdf);
            _logger.Log($"Report generated and sent to {recipientEmail}");

            return fileName;
        }
    }
}
