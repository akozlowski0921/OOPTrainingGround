using System;

namespace SOLID.OCP.Good3
{
    public class PushNotification : INotificationChannel
    {
        public void Send(string message, string recipient)
        {
            Console.WriteLine($"Sending push notification to {recipient}: {message}");
        }
    }
}
