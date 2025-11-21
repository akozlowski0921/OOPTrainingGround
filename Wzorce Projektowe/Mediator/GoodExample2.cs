using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DesignPatterns.Mediator.Good2
{
    // ✅ GOOD: Mediator reduces dependencies between microservices

    // ✅ Domain Events
    public interface IEvent { }

    public class OrderPlacedEvent : IEvent
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentProcessedEvent : IEvent
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }

    public class InventoryReservedEvent : IEvent
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    // ✅ Mediator interface (like MediatR)
    public interface IMediator
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : IEvent;
    }

    // ✅ Event Handler interface
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken ct = default);
    }

    // ✅ Concrete Mediator
    public class EventMediator : IMediator
    {
        private readonly Dictionary<Type, List<object>> _handlers = new();

        public void RegisterHandler<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            if (!_handlers.ContainsKey(eventType))
            {
                _handlers[eventType] = new List<object>();
            }
            _handlers[eventType].Add(handler);
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            if (_handlers.ContainsKey(eventType))
            {
                var tasks = new List<Task>();
                foreach (var handler in _handlers[eventType])
                {
                    var typedHandler = (IEventHandler<TEvent>)handler;
                    tasks.Add(typedHandler.HandleAsync(@event, ct));
                }
                await Task.WhenAll(tasks);
            }
        }
    }

    // ✅ Services - only depend on Mediator

    public class OrderService
    {
        private readonly IMediator _mediator;

        public OrderService(IMediator mediator)
        {
            _mediator = mediator;
            // ✅ Only depends on mediator, not other services
        }

        public async Task PlaceOrderAsync(int orderId, int productId, int quantity, decimal amount)
        {
            Console.WriteLine($"[OrderService] Placing order {orderId}");
            
            // ✅ Publish event through mediator
            await _mediator.PublishAsync(new OrderPlacedEvent
            {
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity,
                Amount = amount
            });

            Console.WriteLine($"[OrderService] Order {orderId} placed");
        }
    }

    public class InventoryService : IEventHandler<OrderPlacedEvent>
    {
        private readonly IMediator _mediator;

        public InventoryService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task HandleAsync(OrderPlacedEvent @event, CancellationToken ct = default)
        {
            Console.WriteLine($"[InventoryService] Checking stock for product {@event.ProductId}");
            
            // Check stock (simplified)
            var hasStock = true;
            if (!hasStock)
            {
                throw new Exception("Out of stock");
            }

            Console.WriteLine($"[InventoryService] Reserving {@event.Quantity} units of product {@event.ProductId}");
            
            // ✅ Publish event for next step
            await _mediator.PublishAsync(new InventoryReservedEvent
            {
                OrderId = @event.OrderId,
                ProductId = @event.ProductId,
                Quantity = @event.Quantity
            });
        }
    }

    public class PaymentService : IEventHandler<InventoryReservedEvent>
    {
        private readonly IMediator _mediator;

        public PaymentService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task HandleAsync(InventoryReservedEvent @event, CancellationToken ct = default)
        {
            Console.WriteLine($"[PaymentService] Processing payment for order {@event.OrderId}");
            await Task.Delay(100); // Simulate payment processing

            // ✅ Publish event
            await _mediator.PublishAsync(new PaymentProcessedEvent
            {
                OrderId = @event.OrderId,
                Amount = 100m
            });

            Console.WriteLine($"[PaymentService] Payment processed for order {@event.OrderId}");
        }
    }

    public class NotificationService : 
        IEventHandler<OrderPlacedEvent>,
        IEventHandler<PaymentProcessedEvent>
    {
        public Task HandleAsync(OrderPlacedEvent @event, CancellationToken ct = default)
        {
            Console.WriteLine($"[NotificationService] Sending order confirmation for {@event.OrderId}");
            return Task.CompletedTask;
        }

        public Task HandleAsync(PaymentProcessedEvent @event, CancellationToken ct = default)
        {
            Console.WriteLine($"[NotificationService] Sending payment confirmation for {@event.OrderId}");
            return Task.CompletedTask;
        }
    }

    public class ShippingService : IEventHandler<PaymentProcessedEvent>
    {
        public Task HandleAsync(PaymentProcessedEvent @event, CancellationToken ct = default)
        {
            Console.WriteLine($"[ShippingService] Creating shipment for order {@event.OrderId}");
            return Task.CompletedTask;
        }
    }

    // ✅ Usage example
    public class MediatorServiceExample
    {
        public static async Task RunAsync()
        {
            var mediator = new EventMediator();

            // ✅ Register handlers
            var inventoryService = new InventoryService(mediator);
            var paymentService = new PaymentService(mediator);
            var notificationService = new NotificationService();
            var shippingService = new ShippingService();

            mediator.RegisterHandler<OrderPlacedEvent>(inventoryService);
            mediator.RegisterHandler<OrderPlacedEvent>(notificationService);
            mediator.RegisterHandler<InventoryReservedEvent>(paymentService);
            mediator.RegisterHandler<PaymentProcessedEvent>(notificationService);
            mediator.RegisterHandler<PaymentProcessedEvent>(shippingService);

            // ✅ Place order
            var orderService = new OrderService(mediator);
            await orderService.PlaceOrderAsync(
                orderId: 123,
                productId: 456,
                quantity: 2,
                amount: 100m);

            await Task.Delay(1000); // Wait for async handlers
        }
    }

    // ✅ Benefits:
    // - Services only depend on mediator (loose coupling)
    // - Easy to add new services (just register handlers)
    // - Event-driven architecture
    // - Services can evolve independently
    // - Easy testing (mock mediator)
    // - No circular dependencies
}
