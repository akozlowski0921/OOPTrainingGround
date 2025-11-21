using System;

namespace SOLID.DIP.Good3
{
    public class SmtpEmailSender : IEmailSender
    {
        public void SendEmail(string to, string subject, string body)
        {
            Console.WriteLine($"Sending via SMTP to {to}");
        }
    }
}
