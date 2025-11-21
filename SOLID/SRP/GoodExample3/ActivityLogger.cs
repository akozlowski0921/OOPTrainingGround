using System;
using System.IO;

namespace SOLID.SRP.Good3
{
    // ✅ Responsibility: Logging
    public class ActivityLogger
    {
        private readonly string _logFile;

        public ActivityLogger(string logFile)
        {
            _logFile = logFile;
        }

        public void Log(string message)
        {
            File.AppendAllText(_logFile, $"{DateTime.Now}: {message}\n");
        }
    }
}
