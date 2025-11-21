using System;
using System.Collections.Generic;

namespace CleanCodeFundamentals.YAGNI.Bad2
{
    // Problem: Przygotowanie na przyszłe wymagania, których nigdy nie będzie
    public class ReportGenerator
    {
        // "Na przyszłość" dodano wsparcie dla 15 formatów, choć używany jest tylko PDF
        private Dictionary<string, IReportFormatter> _formatters;
        private Dictionary<string, bool> _formatterEnabled;
        private Dictionary<string, int> _formatterPriority;
        private Dictionary<string, string> _formatterDescription;

        public ReportGenerator()
        {
            _formatters = new Dictionary<string, IReportFormatter>();
            _formatterEnabled = new Dictionary<string, bool>();
            _formatterPriority = new Dictionary<string, int>();
            _formatterDescription = new Dictionary<string, string>();
            
            // Rejestracja wielu formaterów "na przyszłość"
            RegisterFormatter("pdf", new PdfFormatter(), true, 1, "PDF format");
            RegisterFormatter("excel", new ExcelFormatter(), false, 2, "Excel format");
            RegisterFormatter("word", new WordFormatter(), false, 3, "Word format");
            RegisterFormatter("html", new HtmlFormatter(), false, 4, "HTML format");
            RegisterFormatter("xml", new XmlFormatter(), false, 5, "XML format");
            RegisterFormatter("json", new JsonFormatter(), false, 6, "JSON format");
            RegisterFormatter("csv", new CsvFormatter(), false, 7, "CSV format");
            RegisterFormatter("txt", new TxtFormatter(), false, 8, "Text format");
        }

        private void RegisterFormatter(string type, IReportFormatter formatter, 
            bool enabled, int priority, string description)
        {
            _formatters[type] = formatter;
            _formatterEnabled[type] = enabled;
            _formatterPriority[type] = priority;
            _formatterDescription[type] = description;
        }

        public byte[] GenerateReport(string data, string format)
        {
            // Skomplikowana logika dla "przyszłościowego" systemu pluginów
            if (!_formatters.ContainsKey(format))
            {
                throw new ArgumentException($"Unknown format: {format}");
            }

            if (!_formatterEnabled[format])
            {
                throw new InvalidOperationException($"Formatter {format} is disabled");
            }

            // W rzeczywistości używany jest tylko PDF
            return _formatters[format].Format(data);
        }

        // Metody "na przyszłość", które nigdy nie są używane
        public void EnableFormatter(string type)
        {
            if (_formatterEnabled.ContainsKey(type))
                _formatterEnabled[type] = true;
        }

        public void DisableFormatter(string type)
        {
            if (_formatterEnabled.ContainsKey(type))
                _formatterEnabled[type] = false;
        }

        public void SetFormatterPriority(string type, int priority)
        {
            if (_formatterPriority.ContainsKey(type))
                _formatterPriority[type] = priority;
        }

        public List<string> GetAvailableFormatters()
        {
            return new List<string>(_formatters.Keys);
        }

        public Dictionary<string, bool> GetFormatterStatus()
        {
            return new Dictionary<string, bool>(_formatterEnabled);
        }
    }

    public interface IReportFormatter
    {
        byte[] Format(string data);
    }

    public class PdfFormatter : IReportFormatter
    {
        public byte[] Format(string data) => System.Text.Encoding.UTF8.GetBytes($"PDF: {data}");
    }

    // Wszystkie poniższe formatery są zaimplementowane, ale nigdy nie używane
    public class ExcelFormatter : IReportFormatter
    {
        public byte[] Format(string data) => System.Text.Encoding.UTF8.GetBytes($"Excel: {data}");
    }

    public class WordFormatter : IReportFormatter
    {
        public byte[] Format(string data) => System.Text.Encoding.UTF8.GetBytes($"Word: {data}");
    }

    public class HtmlFormatter : IReportFormatter
    {
        public byte[] Format(string data) => System.Text.Encoding.UTF8.GetBytes($"HTML: {data}");
    }

    public class XmlFormatter : IReportFormatter
    {
        public byte[] Format(string data) => System.Text.Encoding.UTF8.GetBytes($"XML: {data}");
    }

    public class JsonFormatter : IReportFormatter
    {
        public byte[] Format(string data) => System.Text.Encoding.UTF8.GetBytes($"JSON: {data}");
    }

    public class CsvFormatter : IReportFormatter
    {
        public byte[] Format(string data) => System.Text.Encoding.UTF8.GetBytes($"CSV: {data}");
    }

    public class TxtFormatter : IReportFormatter
    {
        public byte[] Format(string data) => System.Text.Encoding.UTF8.GetBytes($"TXT: {data}");
    }
}
