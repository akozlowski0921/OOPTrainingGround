namespace SOLID.DIP.Good3
{
    // ✅ GOOD: Depends on abstraction
    public class UserRegistration
    {
        private readonly IEmailSender _emailSender;

        public UserRegistration(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public void RegisterUser(string email)
        {
            _emailSender.SendEmail(email, "Welcome", "Welcome to our service");
        }
    }
}
