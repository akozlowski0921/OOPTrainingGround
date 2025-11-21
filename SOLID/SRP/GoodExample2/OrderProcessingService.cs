namespace SOLID.SRP.Good2
{
    // ✅ GOOD: Orkiestracja - używa innych klas do wykonania zadania
    public class OrderProcessingService
    {
        private readonly OrderValidator _validator;
        private readonly PriceCalculator _priceCalculator;
        private readonly OrderRepository _repository;
        private readonly EmailService _emailService;
        private readonly Logger _logger;

        public OrderProcessingService(
            OrderValidator validator,
            PriceCalculator priceCalculator,
            OrderRepository repository,
            EmailService emailService,
            Logger logger)
        {
            _validator = validator;
            _priceCalculator = priceCalculator;
            _repository = repository;
            _emailService = emailService;
            _logger = logger;
        }

        public bool ProcessOrder(Order order)
        {
            var validationResult = _validator.Validate(order);
            if (!validationResult.IsValid)
            {
                _logger.Log($"Invalid order: {string.Join(", ", validationResult.Errors)}");
                return false;
            }

            order.TotalAmount = _priceCalculator.CalculateTotal(order.Items);
            _repository.Save(order);

            try
            {
                _emailService.SendOrderConfirmation(order.CustomerEmail, order.TotalAmount);
            }
            catch (Exception ex)
            {
                _logger.Log($"Email error: {ex.Message}");
            }

            _logger.Log($"Order processed: {order.Id}, Total: {order.TotalAmount:C}");
            return true;
        }
    }
}
