using System;
using System.Collections.Generic;
using System.Threading;

namespace DesignPatterns.Command.Bad2
{
    // ❌ BAD: No proper task scheduling/queuing in microservices

    public class EmailService
    {
        public void SendEmail(string to, string subject, string body)
        {
            Console.WriteLine($"Sending email to {to}");
            Thread.Sleep(100); // Simulate work
        }
    }

    public class SmsService
    {
        public void SendSms(string phone, string message)
        {
            Console.WriteLine($"Sending SMS to {phone}");
            Thread.Sleep(100);
        }
    }

    public class NotificationService
    {
        private readonly EmailService _emailService = new();
        private readonly SmsService _smsService = new();

        // ❌ Synchronous execution - blocks caller
        public void SendWelcomeNotification(string email, string phone)
        {
            _emailService.SendEmail(email, "Welcome", "Welcome to our service");
            _smsService.SendSms(phone, "Welcome!");
            // ❌ No queuing - wykonywane natychmiast
            // ❌ No retry logic
            // ❌ No priority handling
            // ❌ Cannot defer execution
            // ❌ No task tracking
        }

        // ❌ No way to schedule for later
        public void SendReminderNotification(string email, string message)
        {
            _emailService.SendEmail(email, "Reminder", message);
            // ❌ Cannot schedule for specific time
            // ❌ No background processing
        }
    }

    // ❌ PROBLEMY:
    // - Brak kolejkowania tasków
    // - Synchroniczne wykonanie blokuje wątek
    // - Brak retry logic przy błędach
    // - Nie można schedulować zadań na później
    // - Brak priorytetyzacji
    // - Trudne testowanie
}
