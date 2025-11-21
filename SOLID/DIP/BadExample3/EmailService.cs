using System;

namespace SOLID.DIP.Bad3
{
    // Low-level module
    public class SmtpEmailSender
    {
        public void SendEmail(string to, string subject, string body)
        {
            Console.WriteLine($"Sending via SMTP to {to}");
        }
    }

    // ❌ BAD: High-level depends on low-level
    public class OrderProcessor
    {
        private SmtpEmailSender _emailSender = new SmtpEmailSender();

        public void ProcessOrder(int orderId)
        {
            // Process order logic
            _emailSender.SendEmail("customer@example.com", "Order Confirmation", $"Order {orderId} confirmed");
        }
    }

    public class UserRegistration
    {
        private SmtpEmailSender _emailSender = new SmtpEmailSender();

        public void RegisterUser(string email)
        {
            // Registration logic
            _emailSender.SendEmail(email, "Welcome", "Welcome to our service");
        }
    }
}
