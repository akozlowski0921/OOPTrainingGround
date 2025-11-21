using System.Net.Mail;

namespace SOLID.SRP.Good2
{
    // ✅ GOOD: Odpowiedzialność - tylko wysyłanie emaili
    public class EmailService
    {
        private readonly string _smtpServer;

        public EmailService(string smtpServer)
        {
            _smtpServer = smtpServer;
        }

        public void SendOrderConfirmation(string customerEmail, decimal totalAmount)
        {
            var smtp = new SmtpClient(_smtpServer);
            var mail = new MailMessage
            {
                To = { customerEmail },
                Subject = "Order Confirmation",
                Body = $"Your order total is: {totalAmount:C}"
            };
            smtp.Send(mail);
        }
    }
}
