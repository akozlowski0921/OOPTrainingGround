namespace SOLID.DIP.Good3
{
    // ✅ Abstraction
    public interface IEmailSender
    {
        void SendEmail(string to, string subject, string body);
    }
}
