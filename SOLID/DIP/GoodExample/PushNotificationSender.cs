using System;

namespace SOLID.DIP.GoodExample
{
    /// <summary>
    /// Konkretna implementacja - wysyłanie powiadomień push
    /// Kolejna implementacja dodana bez modyfikacji istniejącego kodu
    /// </summary>
    public class PushNotificationSender : IMessageSender
    {
        public void SendMessage(string to, string subject, string body)
        {
            Console.WriteLine($"[PUSH] Wysyłanie powiadomienia push do: {to}");
            Console.WriteLine($"[PUSH] Tytuł: {subject}");
            Console.WriteLine($"[PUSH] Treść: {body}");
            Console.WriteLine("[PUSH] Powiadomienie wysłane pomyślnie!\n");
        }
    }
}
