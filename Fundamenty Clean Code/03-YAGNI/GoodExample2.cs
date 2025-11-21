using System;

namespace CleanCodeFundamentals.YAGNI.Good2
{
    // Rozwiązanie: Implementujemy tylko to, co jest aktualnie potrzebne
    public class ReportGenerator
    {
        private readonly PdfFormatter _pdfFormatter;

        public ReportGenerator()
        {
            _pdfFormatter = new PdfFormatter();
        }

        public byte[] GenerateReport(string data)
        {
            return _pdfFormatter.Format(data);
        }
    }

    public class PdfFormatter
    {
        public byte[] Format(string data)
        {
            // Rzeczywista implementacja formatowania PDF
            return System.Text.Encoding.UTF8.GetBytes($"PDF: {data}");
        }
    }

    // Uwaga: Gdy w przyszłości będziemy potrzebować innych formatów,
    // wtedy dodamy interfejs i inne implementacje.
    // Teraz to jest nadmiarowe.
}
