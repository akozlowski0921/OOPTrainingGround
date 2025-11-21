using System;

namespace SOLID.OCP.Good3
{
    public class EmailNotification : INotificationChannel
    {
        public void Send(string message, string recipient)
        {
            Console.WriteLine($"Sending email to {recipient}: {message}");
        }
    }
}
