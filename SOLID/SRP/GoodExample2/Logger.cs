using System.IO;

namespace SOLID.SRP.Good2
{
    // ✅ GOOD: Odpowiedzialność - tylko logowanie
    public class Logger
    {
        private readonly string _logFilePath;

        public Logger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public void Log(string message)
        {
            var logEntry = $"{DateTime.Now}: {message}\n";
            File.AppendAllText(_logFilePath, logEntry);
        }
    }
}
