using System;

namespace SOLID.DIP.Good3
{
    public class SendGridEmailSender : IEmailSender
    {
        public void SendEmail(string to, string subject, string body)
        {
            Console.WriteLine($"Sending via SendGrid to {to}");
        }
    }
}
