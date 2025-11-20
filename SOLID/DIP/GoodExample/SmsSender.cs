using System;

namespace SOLID.DIP.GoodExample
{
    /// <summary>
    /// Konkretna implementacja - wysyłanie przez SMS
    /// Możemy ją dodać bez modyfikacji NotificationService!
    /// </summary>
    public class SmsSender : IMessageSender
    {
        public void SendMessage(string to, string subject, string body)
        {
            Console.WriteLine($"[SMS] Wysyłanie SMS do: {to}");
            Console.WriteLine($"[SMS] {subject}: {body}");
            Console.WriteLine("[SMS] SMS wysłany pomyślnie!\n");
        }
    }
}
