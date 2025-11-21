namespace SOLID.OCP.Good3
{
    // ✅ GOOD: Service is closed for modification, open for extension
    public class NotificationService
    {
        private readonly INotificationChannel _channel;

        public NotificationService(INotificationChannel channel)
        {
            _channel = channel;
        }

        public void SendNotification(string message, string recipient)
        {
            _channel.Send(message, recipient);
        }
    }
}
