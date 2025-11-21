using System;

namespace SOLID.OCP.Good3
{
    public class SmsNotification : INotificationChannel
    {
        public void Send(string message, string recipient)
        {
            Console.WriteLine($"Sending SMS to {recipient}: {message}");
        }
    }
}
