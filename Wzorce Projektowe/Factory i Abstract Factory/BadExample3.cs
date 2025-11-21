using System;

namespace DesignPatterns.Factory.Bad3
{
    // ❌ BAD: Configuration sprawl

    public class BadConfigBasedCreation
    {
        public object CreateService(string config)
        {
            // ❌ String-based configuration
            if (config == "email")
                return new EmailService();
            if (config == "sms")
                return new SmsService();
            return null;
        }
    }

    class EmailService { }
    class SmsService { }
}
