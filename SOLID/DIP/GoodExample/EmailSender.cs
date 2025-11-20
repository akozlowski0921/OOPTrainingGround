using System;

namespace SOLID.DIP.GoodExample
{
    /// <summary>
    /// Konkretna implementacja - wysyłanie przez email
    /// Implementuje abstrakcję IMessageSender
    /// </summary>
    public class EmailSender : IMessageSender
    {
        public void SendMessage(string to, string subject, string body)
        {
            Console.WriteLine($"[EMAIL] Wysyłanie do: {to}");
            Console.WriteLine($"[EMAIL] Temat: {subject}");
            Console.WriteLine($"[EMAIL] Treść: {body}");
            Console.WriteLine("[EMAIL] Email wysłany pomyślnie!\n");
        }
    }
}
