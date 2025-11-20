using System;

namespace SOLID.DIP.GoodExample
{
    /// <summary>
    /// NotificationService zgodny z DIP
    /// Zależy od abstrakcji (IMessageSender), nie od konkretnej implementacji
    /// </summary>
    public class NotificationService
    {
        private readonly IMessageSender _messageSender;

        // Dependency Injection przez konstruktor
        public NotificationService(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public void SendWelcomeNotification(string recipient, string userName)
        {
            var subject = "Witaj w naszym systemie!";
            var body = $"Cześć {userName},\n\nDziękujemy za rejestrację!";
            
            _messageSender.SendMessage(recipient, subject, body);
        }

        public void SendPasswordResetNotification(string recipient)
        {
            var subject = "Reset hasła";
            var body = "Kliknij link, aby zresetować hasło: https://example.com/reset";
            
            _messageSender.SendMessage(recipient, subject, body);
        }

        public void SendOrderConfirmation(string recipient, string orderNumber)
        {
            var subject = "Potwierdzenie zamówienia";
            var body = $"Twoje zamówienie #{orderNumber} zostało przyjęte do realizacji.";
            
            _messageSender.SendMessage(recipient, subject, body);
        }
    }

    // Program demonstracyjny
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("=== Test NotificationService z różnymi implementacjami ===\n");

            // Email
            Console.WriteLine("--- Wysyłanie przez Email ---");
            var emailService = new NotificationService(new EmailSender());
            emailService.SendWelcomeNotification("jan.kowalski@example.com", "Jan Kowalski");

            // SMS
            Console.WriteLine("--- Wysyłanie przez SMS ---");
            var smsService = new NotificationService(new SmsSender());
            smsService.SendPasswordResetNotification("+48 123 456 789");

            // Push Notification
            Console.WriteLine("--- Wysyłanie przez Push ---");
            var pushService = new NotificationService(new PushNotificationSender());
            pushService.SendOrderConfirmation("user123", "ORD-2024-001");

            // Mock (dla testów)
            Console.WriteLine("--- Testowanie z Mock ---");
            var mockSender = new MockMessageSender();
            var mockService = new NotificationService(mockSender);
            mockService.SendWelcomeNotification("test@example.com", "Test User");
            
            Console.WriteLine($"\n[TEST] Liczba wysłanych wiadomości: {mockSender.MessagesSentCount}");
            Console.WriteLine($"[TEST] Ostatni odbiorca: {mockSender.LastRecipient}");
            Console.WriteLine($"[TEST] Ostatni temat: {mockSender.LastSubject}");

            Console.WriteLine("\n=== Korzyści z DIP ===");
            Console.WriteLine("✅ Możemy łatwo zmienić sposób wysyłania (email → SMS → push)");
            Console.WriteLine("✅ Możemy testować bez wysyłania prawdziwych wiadomości (Mock)");
            Console.WriteLine("✅ Luźne powiązanie (loose coupling)");
            Console.WriteLine("✅ Zgodność z Open/Closed Principle");
            Console.WriteLine("✅ Łatwość w utrzymaniu i rozbudowie");
        }
    }
}
