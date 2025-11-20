using System;

namespace SOLID.SRP.GoodExample
{
    /// <summary>
    /// Odpowiedzialność: Logowanie zdarzeń
    /// </summary>
    public class Logger
    {
        public void LogInfo(string message)
        {
            Console.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public void LogError(string message)
        {
            Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }
    }
}
