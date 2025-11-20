using System;

namespace SOLID.DIP.BadExample
{
    /// <summary>
    /// Konkretna implementacja wysyłania emaili
    /// </summary>
    public class EmailSender
    {
        public void SendEmail(string to, string subject, string body)
        {
            Console.WriteLine($"[EMAIL] Wysyłanie do: {to}");
            Console.WriteLine($"[EMAIL] Temat: {subject}");
            Console.WriteLine($"[EMAIL] Treść: {body}");
            Console.WriteLine("[EMAIL] Email wysłany pomyślnie!\n");
        }
    }

    /// <summary>
    /// Naruszenie DIP: Bezpośrednia zależność od konkretnej implementacji (EmailSender)
    /// Klasa wysokopoziomowa (NotificationService) zależy od klasy niskopoziomowej (EmailSender)
    /// </summary>
    public class NotificationService
    {
        private readonly EmailSender _emailSender;

        public NotificationService()
        {
            // Tworzenie konkretnej implementacji w konstruktorze - mocne powiązanie!
            _emailSender = new EmailSender();
        }

        public void SendWelcomeNotification(string userEmail, string userName)
        {
            var subject = "Witaj w naszym systemie!";
            var body = $"Cześć {userName},\n\nDziękujemy za rejestrację!";
            
            _emailSender.SendEmail(userEmail, subject, body);
        }

        public void SendPasswordResetNotification(string userEmail)
        {
            var subject = "Reset hasła";
            var body = "Kliknij link, aby zresetować hasło: https://example.com/reset";
            
            _emailSender.SendEmail(userEmail, subject, body);
        }
    }

    // Program demonstracyjny
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("=== Test NotificationService ===\n");

            var notificationService = new NotificationService();
            
            notificationService.SendWelcomeNotification("jan.kowalski@example.com", "Jan Kowalski");
            notificationService.SendPasswordResetNotification("anna.nowak@example.com");

            Console.WriteLine("\n=== Problem: Nie możemy łatwo zmienić sposobu wysyłania ===");
            Console.WriteLine("Chcemy wysyłać SMS zamiast email? Musimy modyfikować NotificationService!");
            Console.WriteLine("Chcemy testować bez wysyłania prawdziwych emaili? Niemożliwe bez modyfikacji!");
        }
    }

    /// <summary>
    /// Problemy z tym podejściem:
    /// 1. Nie możemy łatwo zmienić EmailSender na SmsSender
    /// 2. Nie możemy testować NotificationService bez wysyłania prawdziwych emaili
    /// 3. Silne powiązanie (tight coupling) - zmiana EmailSender wymaga zmiany NotificationService
    /// 4. Naruszenie Open/Closed Principle - nie możemy rozszerzyć bez modyfikacji
    /// 5. Trudność w utrzymaniu - każda zmiana w EmailSender może zepsuć NotificationService
    /// </summary>
}
