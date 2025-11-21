using System;

namespace DesignPatterns.Mediator.Bad2
{
    // ❌ BAD: Direct dependencies between microservices

    public class OrderService
    {
        private readonly PaymentService _paymentService;
        private readonly InventoryService _inventoryService;
        private readonly NotificationService _notificationService;
        private readonly ShippingService _shippingService;

        public OrderService(
            PaymentService paymentService,
            InventoryService inventoryService,
            NotificationService notificationService,
            ShippingService shippingService)
        {
            _paymentService = paymentService;
            _inventoryService = inventoryService;
            _notificationService = notificationService;
            _shippingService = shippingService;
            // ❌ OrderService zna wszystkie inne serwisy
            // ❌ Tight coupling
        }

        public void PlaceOrder(int orderId, int productId, int quantity)
        {
            // ❌ Direct calls to all services
            var hasStock = _inventoryService.CheckStock(productId, quantity);
            if (!hasStock)
                throw new Exception("Out of stock");

            _inventoryService.ReserveStock(productId, quantity);
            _paymentService.ProcessPayment(orderId, 100m);
            _notificationService.SendOrderConfirmation(orderId);
            _shippingService.CreateShipment(orderId);
            
            // ❌ OrderService orchestruje wszystko
            // ❌ Trudne testowanie (mockowanie 4 serwisów)
            // ❌ Zmiana w jednym serwisie wymaga zmian w OrderService
        }
    }

    public class PaymentService
    {
        private readonly NotificationService _notificationService;

        public PaymentService(NotificationService notificationService)
        {
            _notificationService = notificationService;
            // ❌ PaymentService zna NotificationService
        }

        public void ProcessPayment(int orderId, decimal amount)
        {
            Console.WriteLine($"Processing payment for order {orderId}");
            _notificationService.SendPaymentConfirmation(orderId);
            // ❌ Cross-service communication
        }
    }

    public class InventoryService
    {
        public bool CheckStock(int productId, int quantity)
        {
            return true;
        }

        public void ReserveStock(int productId, int quantity)
        {
            Console.WriteLine($"Reserved {quantity} of product {productId}");
        }
    }

    public class NotificationService
    {
        public void SendOrderConfirmation(int orderId)
        {
            Console.WriteLine($"Sending order confirmation for {orderId}");
        }

        public void SendPaymentConfirmation(int orderId)
        {
            Console.WriteLine($"Sending payment confirmation for {orderId}");
        }
    }

    public class ShippingService
    {
        public void CreateShipment(int orderId)
        {
            Console.WriteLine($"Creating shipment for order {orderId}");
        }
    }

    // ❌ PROBLEMY:
    // - Skomplikowana sieć zależności między serwisami
    // - Trudne dodanie nowego serwisu
    // - Trudne testowanie (mocking multiple dependencies)
    // - Naruszenie Single Responsibility
    // - Tight coupling - zmiana w jednym serwisie propaguje się
}
