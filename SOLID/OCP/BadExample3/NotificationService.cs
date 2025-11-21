using System;

namespace SOLID.OCP.Bad3
{
    // ❌ BAD: Adding new notification channel requires modifying this class
    public class NotificationService
    {
        public void SendNotification(string message, string channel, string recipient)
        {
            if (channel == "Email")
            {
                Console.WriteLine($"Sending email to {recipient}: {message}");
                // Email sending logic
            }
            else if (channel == "SMS")
            {
                Console.WriteLine($"Sending SMS to {recipient}: {message}");
                // SMS sending logic
            }
            else if (channel == "Push")
            {
                Console.WriteLine($"Sending push notification to {recipient}: {message}");
                // Push notification logic
            }
            else if (channel == "Slack")
            {
                Console.WriteLine($"Sending Slack message to {recipient}: {message}");
                // Slack API logic
            }
            else if (channel == "WhatsApp")
            {
                Console.WriteLine($"Sending WhatsApp to {recipient}: {message}");
                // WhatsApp API logic
            }
            else
            {
                throw new ArgumentException($"Unknown channel: {channel}");
            }
        }
    }
}
