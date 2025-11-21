using System;

namespace SOLID.OCP.Good3
{
    public class SlackNotification : INotificationChannel
    {
        public void Send(string message, string recipient)
        {
            Console.WriteLine($"Sending Slack message to {recipient}: {message}");
        }
    }
}
