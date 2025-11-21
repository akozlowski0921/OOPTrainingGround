namespace SOLID.OCP.Good3
{
    // ✅ GOOD: Interface for notification channels
    public interface INotificationChannel
    {
        void Send(string message, string recipient);
    }
}
