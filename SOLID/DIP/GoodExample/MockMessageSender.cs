using System;

namespace SOLID.DIP.GoodExample
{
    /// <summary>
    /// Mock implementation dla testów
    /// Nie wysyła prawdziwych wiadomości - idealne do testowania!
    /// </summary>
    public class MockMessageSender : IMessageSender
    {
        public int MessagesSentCount { get; private set; }
        public string LastRecipient { get; private set; } = string.Empty;
        public string LastSubject { get; private set; } = string.Empty;
        public string LastBody { get; private set; } = string.Empty;

        public void SendMessage(string to, string subject, string body)
        {
            MessagesSentCount++;
            LastRecipient = to;
            LastSubject = subject;
            LastBody = body;
            
            Console.WriteLine($"[MOCK] Symulacja wysłania wiadomości do: {to}");
        }
    }
}
