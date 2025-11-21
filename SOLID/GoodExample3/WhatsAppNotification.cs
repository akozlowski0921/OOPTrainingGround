using System;

namespace SOLID.OCP.Good3
{
    public class WhatsAppNotification : INotificationChannel
    {
        public void Send(string message, string recipient)
        {
            Console.WriteLine($"Sending WhatsApp to {recipient}: {message}");
        }
    }
}
