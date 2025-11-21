using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DesignPatterns.EventSourcing.Good3
{
    // ✅ GOOD: Event Sourcing + CQRS - perfect synergy

    // ✅ WRITE SIDE - Event Sourcing

    // Domain Events
    public abstract class OrderEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Version { get; set; }
    }

    public class OrderPlacedEvent : OrderEvent
    {
        public string CustomerId { get; set; }
        public List<OrderLineDto> Lines { get; set; }
    }

    public class OrderLineDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderStatusChangedEvent : OrderEvent
    {
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public string ChangedBy { get; set; }
    }

    public class OrderShippedEvent : OrderEvent
    {
        public string TrackingNumber { get; set; }
        public string Carrier { get; set; }
    }

    public class OrderCancelledEvent : OrderEvent
    {
        public string Reason { get; set; }
        public string CancelledBy { get; set; }
    }

    // ✅ Commands
    public class PlaceOrderCommand
    {
        public string CustomerId { get; set; }
        public List<OrderLineDto> Lines { get; set; }
    }

    public class ChangeOrderStatusCommand
    {
        public Guid OrderId { get; set; }
        public string NewStatus { get; set; }
        public string ChangedBy { get; set; }
    }

    public class ShipOrderCommand
    {
        public Guid OrderId { get; set; }
        public string TrackingNumber { get; set; }
        public string Carrier { get; set; }
    }

    // ✅ Aggregate - Order
    public class Order
    {
        public Guid OrderId { get; private set; }
        public string CustomerId { get; private set; }
        public List<OrderLineDto> Lines { get; private set; } = new();
        public string Status { get; private set; }
        public int Version { get; private set; }
        public bool IsShipped { get; private set; }
        public bool IsCancelled { get; private set; }

        private readonly List<OrderEvent> _uncommittedEvents = new();

        private Order() { }

        // ✅ Factory
        public static Order Place(string customerId, List<OrderLineDto> lines)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentException("Customer ID required");
            
            if (!lines.Any())
                throw new ArgumentException("Order must have at least one line");

            var order = new Order();
            order.Apply(new OrderPlacedEvent
            {
                OrderId = Guid.NewGuid(),
                CustomerId = customerId,
                Lines = lines,
                Version = 1
            });
            return order;
        }

        // ✅ Commands
        public void ChangeStatus(string newStatus, string changedBy)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Cannot change status of cancelled order");

            if (Status == newStatus)
                return;

            Apply(new OrderStatusChangedEvent
            {
                OrderId = OrderId,
                OldStatus = Status,
                NewStatus = newStatus,
                ChangedBy = changedBy,
                Version = Version + 1
            });
        }

        public void Ship(string trackingNumber, string carrier)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Cannot ship cancelled order");

            if (IsShipped)
                throw new InvalidOperationException("Order already shipped");

            Apply(new OrderShippedEvent
            {
                OrderId = OrderId,
                TrackingNumber = trackingNumber,
                Carrier = carrier,
                Version = Version + 1
            });
        }

        public void Cancel(string reason, string cancelledBy)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Order already cancelled");

            if (IsShipped)
                throw new InvalidOperationException("Cannot cancel shipped order");

            Apply(new OrderCancelledEvent
            {
                OrderId = OrderId,
                Reason = reason,
                CancelledBy = cancelledBy,
                Version = Version + 1
            });
        }

        // ✅ Apply event
        private void Apply(OrderEvent @event)
        {
            When(@event);
            _uncommittedEvents.Add(@event);
        }

        // ✅ Load from history
        public static Order LoadFromHistory(IEnumerable<OrderEvent> history)
        {
            var order = new Order();
            foreach (var @event in history)
            {
                order.When(@event);
            }
            return order;
        }

        // ✅ Event handlers
        private void When(OrderEvent @event)
        {
            switch (@event)
            {
                case OrderPlacedEvent e:
                    OrderId = e.OrderId;
                    CustomerId = e.CustomerId;
                    Lines = e.Lines;
                    Status = "Pending";
                    Version = e.Version;
                    break;

                case OrderStatusChangedEvent e:
                    Status = e.NewStatus;
                    Version = e.Version;
                    break;

                case OrderShippedEvent e:
                    IsShipped = true;
                    Status = "Shipped";
                    Version = e.Version;
                    break;

                case OrderCancelledEvent e:
                    IsCancelled = true;
                    Status = "Cancelled";
                    Version = e.Version;
                    break;
            }
        }

        public IEnumerable<OrderEvent> GetUncommittedEvents() => _uncommittedEvents;
        public void MarkEventsAsCommitted() => _uncommittedEvents.Clear();

        public decimal GetTotal() => Lines.Sum(l => l.Price * l.Quantity);
    }

    // ✅ Event Store
    public interface IOrderEventStore
    {
        Task SaveEventsAsync(Guid orderId, IEnumerable<OrderEvent> events, int expectedVersion, CancellationToken ct = default);
        Task<List<OrderEvent>> GetEventsAsync(Guid orderId, CancellationToken ct = default);
        Task<List<OrderEvent>> GetAllEventsAsync(CancellationToken ct = default);
    }

    // ✅ Command handlers
    public class PlaceOrderCommandHandler
    {
        private readonly IOrderEventStore _eventStore;
        private readonly IEventBus _eventBus;

        public PlaceOrderCommandHandler(IOrderEventStore eventStore, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
        }

        public async Task<Guid> HandleAsync(PlaceOrderCommand command, CancellationToken ct = default)
        {
            // ✅ Create aggregate
            var order = Order.Place(command.CustomerId, command.Lines);

            // ✅ Save events
            await _eventStore.SaveEventsAsync(order.OrderId, order.GetUncommittedEvents(), 0, ct);

            // ✅ Publish events to projections
            foreach (var @event in order.GetUncommittedEvents())
            {
                await _eventBus.PublishAsync(@event, ct);
            }

            order.MarkEventsAsCommitted();
            return order.OrderId;
        }
    }

    public class ChangeOrderStatusCommandHandler
    {
        private readonly IOrderEventStore _eventStore;
        private readonly IEventBus _eventBus;

        public ChangeOrderStatusCommandHandler(IOrderEventStore eventStore, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
        }

        public async Task HandleAsync(ChangeOrderStatusCommand command, CancellationToken ct = default)
        {
            // ✅ Load from event store
            var events = await _eventStore.GetEventsAsync(command.OrderId, ct);
            var order = Order.LoadFromHistory(events);

            // ✅ Execute command
            order.ChangeStatus(command.NewStatus, command.ChangedBy);

            // ✅ Save and publish
            await _eventStore.SaveEventsAsync(order.OrderId, order.GetUncommittedEvents(), order.Version - 1, ct);
            
            foreach (var @event in order.GetUncommittedEvents())
            {
                await _eventBus.PublishAsync(@event, ct);
            }

            order.MarkEventsAsCommitted();
        }
    }

    // ✅ READ SIDE - CQRS Projections

    // Read Model 1: Order summary (for list views)
    public class OrderSummaryReadModel
    {
        public Guid OrderId { get; set; }
        public string CustomerId { get; set; }
        public int ItemCount { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public DateTime PlacedAt { get; set; }
    }

    // Read Model 2: Order details (for detail view)
    public class OrderDetailReadModel
    {
        public Guid OrderId { get; set; }
        public string CustomerId { get; set; }
        public List<OrderLineDto> Lines { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public DateTime PlacedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public string TrackingNumber { get; set; }
        public string Carrier { get; set; }
    }

    // Read Model 3: Customer orders (denormalized)
    public class CustomerOrdersReadModel
    {
        public string CustomerId { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime LastOrderDate { get; set; }
    }

    // Read Model 4: Order statistics (for dashboard)
    public class OrderStatistics
    {
        public Dictionary<string, int> OrdersByStatus { get; set; } = new();
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    // ✅ Projections - build read models from events
    public class OrderProjections
    {
        private readonly IOrderReadRepository _readRepository;

        public OrderProjections(IOrderReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        // ✅ Handle events and update read models
        public async Task HandleAsync(OrderEvent @event, CancellationToken ct = default)
        {
            switch (@event)
            {
                case OrderPlacedEvent e:
                    await OnOrderPlaced(e, ct);
                    break;

                case OrderStatusChangedEvent e:
                    await OnOrderStatusChanged(e, ct);
                    break;

                case OrderShippedEvent e:
                    await OnOrderShipped(e, ct);
                    break;

                case OrderCancelledEvent e:
                    await OnOrderCancelled(e, ct);
                    break;
            }
        }

        private async Task OnOrderPlaced(OrderPlacedEvent e, CancellationToken ct)
        {
            var total = e.Lines.Sum(l => l.Price * l.Quantity);

            // ✅ Update order summary
            await _readRepository.InsertOrderSummaryAsync(new OrderSummaryReadModel
            {
                OrderId = e.OrderId,
                CustomerId = e.CustomerId,
                ItemCount = e.Lines.Count,
                Total = total,
                Status = "Pending",
                PlacedAt = e.Timestamp
            }, ct);

            // ✅ Update order details
            await _readRepository.InsertOrderDetailAsync(new OrderDetailReadModel
            {
                OrderId = e.OrderId,
                CustomerId = e.CustomerId,
                Lines = e.Lines,
                Total = total,
                Status = "Pending",
                PlacedAt = e.Timestamp
            }, ct);

            // ✅ Update customer orders (denormalized)
            await _readRepository.IncrementCustomerOrdersAsync(e.CustomerId, total, e.Timestamp, ct);

            // ✅ Update statistics
            await _readRepository.IncrementStatisticsAsync("Pending", total, ct);
        }

        private async Task OnOrderStatusChanged(OrderStatusChangedEvent e, CancellationToken ct)
        {
            await _readRepository.UpdateOrderStatusAsync(e.OrderId, e.NewStatus, ct);
            await _readRepository.UpdateStatisticsOnStatusChangeAsync(e.OldStatus, e.NewStatus, ct);
        }

        private async Task OnOrderShipped(OrderShippedEvent e, CancellationToken ct)
        {
            await _readRepository.MarkOrderAsShippedAsync(
                e.OrderId,
                e.TrackingNumber,
                e.Carrier,
                e.Timestamp,
                ct);
        }

        private async Task OnOrderCancelled(OrderCancelledEvent e, CancellationToken ct)
        {
            await _readRepository.UpdateOrderStatusAsync(e.OrderId, "Cancelled", ct);
        }
    }

    // ✅ Queries
    public class GetOrderQuery
    {
        public Guid OrderId { get; set; }
    }

    public class GetOrdersByCustomerQuery
    {
        public string CustomerId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetOrderStatisticsQuery { }

    // ✅ Read repository (cached, optimized for reads)
    public interface IOrderReadRepository
    {
        Task InsertOrderSummaryAsync(OrderSummaryReadModel order, CancellationToken ct = default);
        Task InsertOrderDetailAsync(OrderDetailReadModel order, CancellationToken ct = default);
        Task<OrderDetailReadModel> GetOrderDetailAsync(Guid orderId, CancellationToken ct = default);
        Task<List<OrderSummaryReadModel>> GetOrdersByCustomerAsync(string customerId, int page, int pageSize, CancellationToken ct = default);
        Task UpdateOrderStatusAsync(Guid orderId, string status, CancellationToken ct = default);
        Task MarkOrderAsShippedAsync(Guid orderId, string trackingNumber, string carrier, DateTime shippedAt, CancellationToken ct = default);
        Task IncrementCustomerOrdersAsync(string customerId, decimal amount, DateTime orderDate, CancellationToken ct = default);
        Task IncrementStatisticsAsync(string status, decimal amount, CancellationToken ct = default);
        Task UpdateStatisticsOnStatusChangeAsync(string oldStatus, string newStatus, CancellationToken ct = default);
        Task<OrderStatistics> GetStatisticsAsync(CancellationToken ct = default);
    }

    // ✅ Query handlers
    public class GetOrderQueryHandler
    {
        private readonly IOrderReadRepository _readRepository;

        public GetOrderQueryHandler(IOrderReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<OrderDetailReadModel> HandleAsync(GetOrderQuery query, CancellationToken ct = default)
        {
            // ✅ Fast read from optimized, cached read model
            return await _readRepository.GetOrderDetailAsync(query.OrderId, ct);
        }
    }

    public class GetOrdersByCustomerQueryHandler
    {
        private readonly IOrderReadRepository _readRepository;

        public GetOrdersByCustomerQueryHandler(IOrderReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<List<OrderSummaryReadModel>> HandleAsync(GetOrdersByCustomerQuery query, CancellationToken ct = default)
        {
            // ✅ Optimized, paginated query
            return await _readRepository.GetOrdersByCustomerAsync(
                query.CustomerId,
                query.PageNumber,
                query.PageSize,
                ct);
        }
    }

    public class GetOrderStatisticsQueryHandler
    {
        private readonly IOrderReadRepository _readRepository;

        public GetOrderStatisticsQueryHandler(IOrderReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<OrderStatistics> HandleAsync(GetOrderStatisticsQuery query, CancellationToken ct = default)
        {
            // ✅ Pre-aggregated statistics (no expensive queries)
            return await _readRepository.GetStatisticsAsync();
        }
    }

    // ✅ Event Bus
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : OrderEvent;
    }

    // ✅ Benefits of Event Sourcing + CQRS:
    // - Write side: Full event history, audit trail, temporal queries
    // - Read side: Multiple optimized projections, caching, denormalization
    // - Events drive projections (eventual consistency)
    // - Independent scaling of writes and reads
    // - Different storage for events (append-only) vs read models (optimized queries)
    // - Business analytics from event stream
    // - Easy to add new projections without touching write side
    // - Replay events to rebuild or add new read models
}
