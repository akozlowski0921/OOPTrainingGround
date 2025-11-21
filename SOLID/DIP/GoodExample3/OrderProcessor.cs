namespace SOLID.DIP.Good3
{
    // ✅ GOOD: Depends on abstraction
    public class OrderProcessor
    {
        private readonly IEmailSender _emailSender;

        public OrderProcessor(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public void ProcessOrder(int orderId)
        {
            _emailSender.SendEmail("customer@example.com", "Order Confirmation", $"Order {orderId} confirmed");
        }
    }
}
