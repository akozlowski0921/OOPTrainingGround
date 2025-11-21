namespace SOLID.SRP.Good2
{
    // ✅ GOOD: Odpowiedzialność - tylko walidacja zamówień
    public class OrderValidator
    {
        public ValidationResult Validate(Order order)
        {
            var errors = new List<string>();

            if (order.Items == null || order.Items.Count == 0)
            {
                errors.Add("Order must contain at least one item");
            }

            if (string.IsNullOrEmpty(order.CustomerEmail))
            {
                errors.Add("Customer email is required");
            }

            return new ValidationResult 
            { 
                IsValid = errors.Count == 0, 
                Errors = errors 
            };
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; }
    }
}
