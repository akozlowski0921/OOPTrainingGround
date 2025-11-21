using System;
using System.IO;
using System.Text;
using System.Threading;

namespace SpecyfikaDotNet.ResourceManagement.Bad3
{
    // ❌ BAD: Brak zarządzania wieloma zasobami i brak obsługi wyjątków
    public class ReportGenerator
    {
        private FileStream _reportFile;
        private StreamWriter _writer;
        private Timer _autoSaveTimer;
        private MemoryStream _buffer;

        public ReportGenerator(string reportPath)
        {
            // Wszystkie te zasoby wymagają dispose
            _reportFile = new FileStream(reportPath, FileMode.Create);
            _writer = new StreamWriter(_reportFile, Encoding.UTF8);
            _buffer = new MemoryStream();

            // Timer też wymaga dispose
            _autoSaveTimer = new Timer(AutoSave, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        public void AddSection(string title, string content)
        {
            // Jeśli tutaj wystąpi wyjątek, zasoby nie zostaną zwolnione
            _writer.WriteLine($"## {title}");
            _writer.WriteLine(content);
            _writer.WriteLine();

            // Buforujemy w pamięci
            var bytes = Encoding.UTF8.GetBytes(content);
            _buffer.Write(bytes, 0, bytes.Length);
        }

        private void AutoSave(object state)
        {
            try
            {
                _writer.Flush();
            }
            catch
            {
                // Ignorujemy błędy, ale zasoby nadal wiszą
            }
        }

        public void Finalize()
        {
            // Próba manualnego czyszczenia, ale to nie jest Dispose pattern
            _writer.Flush();
            _writer.Close();
            _reportFile.Close();
            _buffer.Close();
            _autoSaveTimer.Dispose();
        }

        // BŁĘDY:
        // 1. Brak implementacji IDisposable
        // 2. Finalize() to złe nawiązanie do finalizera (mylące)
        // 3. Jeśli AddSection wyrzuci wyjątek, zasoby wiszą
        // 4. Nikt nie wymusi wywołania Finalize()
        // 5. Brak flagi _disposed do sprawdzania stanu
        // 6. Nie ma SuppressFinalize dla GC
    }

    public class BadReportUsage
    {
        public void GenerateMonthlyReport()
        {
            var generator = new ReportGenerator("monthly_report.txt");

            // BŁĄD: Brak using lub try-finally
            // Jeśli wystąpi wyjątek, zasoby nigdy nie zostaną zwolnione
            generator.AddSection("Sales", "Total sales: $10000");
            generator.AddSection("Expenses", "Total expenses: $5000");

            // BŁĄD: Programista musi pamiętać o wywołaniu Finalize()
            // Łatwo zapomnieć, szczególnie gdy jest wyjątek
            generator.Finalize();
        }
    }
}
