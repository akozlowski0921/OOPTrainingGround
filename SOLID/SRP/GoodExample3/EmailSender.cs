using System;

namespace SOLID.SRP.Good3
{
    // ✅ Responsibility: Email sending
    public class EmailSender
    {
        public void SendReportEmail(string recipientEmail, string subject, 
            string htmlBody, byte[] pdfAttachment)
        {
            // In real implementation, use email library
            Console.WriteLine($"Sending email to {recipientEmail}");
        }
    }
}
