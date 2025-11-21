using System;
using System.IO;
using System.Text;
using System.Threading;

namespace SpecyfikaDotNet.ResourceManagement.Good3
{
    // ✅ GOOD: Prawidłowa implementacja IDisposable z wieloma zasobami
    public class ReportGenerator : IDisposable
    {
        private FileStream _reportFile;
        private StreamWriter _writer;
        private Timer _autoSaveTimer;
        private MemoryStream _buffer;
        private bool _disposed = false;

        public ReportGenerator(string reportPath)
        {
            try
            {
                _reportFile = new FileStream(reportPath, FileMode.Create);
                _writer = new StreamWriter(_reportFile, Encoding.UTF8);
                _buffer = new MemoryStream();
                _autoSaveTimer = new Timer(AutoSave, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
            }
            catch
            {
                // Jeśli coś pójdzie nie tak podczas konstruktora, czyścimy już utworzone zasoby
                Dispose();
                throw;
            }
        }

        public void AddSection(string title, string content)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ReportGenerator));

            _writer.WriteLine($"## {title}");
            _writer.WriteLine(content);
            _writer.WriteLine();

            var bytes = Encoding.UTF8.GetBytes(content);
            _buffer.Write(bytes, 0, bytes.Length);
        }

        private void AutoSave(object state)
        {
            if (!_disposed)
            {
                try
                {
                    _writer?.Flush();
                }
                catch
                {
                    // Log error, but don't crash
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    try
                    {
                        _autoSaveTimer?.Dispose();
                    }
                    catch { /* Ignore */ }

                    try
                    {
                        _writer?.Flush();
                        _writer?.Dispose();
                    }
                    catch { /* Ignore */ }

                    try
                    {
                        _reportFile?.Dispose();
                    }
                    catch { /* Ignore */ }

                    try
                    {
                        _buffer?.Dispose();
                    }
                    catch { /* Ignore */ }
                }

                // Free unmanaged resources here if any

                _disposed = true;
            }
        }

        // Opcjonalnie: finalizer tylko jeśli mamy niezarządzane zasoby
        // ~ReportGenerator()
        // {
        //     Dispose(false);
        // }
    }

    public class GoodReportUsage
    {
        public void GenerateMonthlyReport()
        {
            // Using zapewnia, że Dispose() zostanie wywołane
            // nawet gdy wystąpi wyjątek
            using (var generator = new ReportGenerator("monthly_report.txt"))
            {
                generator.AddSection("Sales", "Total sales: $10000");
                generator.AddSection("Expenses", "Total expenses: $5000");
                // Dispose() wywołane automatycznie na końcu using
            }
        }

        // Alternatywnie: using declaration (C# 8.0+)
        public void GenerateMonthlyReportModern()
        {
            using var generator = new ReportGenerator("monthly_report.txt");
            generator.AddSection("Sales", "Total sales: $10000");
            generator.AddSection("Expenses", "Total expenses: $5000");
            // Dispose() wywołane automatycznie na końcu scope
        }
    }
}
