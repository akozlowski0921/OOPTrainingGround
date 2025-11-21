using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DesignPatterns.CQRS.Good2
{
    // ✅ GOOD: CQRS with Event Sourcing - Complete separation

    // ✅ WRITE SIDE - Events and Commands

    // Domain events
    public abstract class DomainEvent
    {
        public Guid AggregateId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Version { get; set; }
    }

    public class OrderCreatedEvent : DomainEvent
    {
        public string CustomerName { get; set; }
        public List<string> Items { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class OrderStatusChangedEvent : DomainEvent
    {
        public string NewStatus { get; set; }
    }

    public class OrderItemAddedEvent : DomainEvent
    {
        public string Item { get; set; }
        public decimal Price { get; set; }
    }

    // Commands
    public class CreateOrderCommand
    {
        public string CustomerName { get; set; }
        public List<string> Items { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class ChangeOrderStatusCommand
    {
        public Guid OrderId { get; set; }
        public string NewStatus { get; set; }
    }

    // Aggregate root - built from events
    public class Order
    {
        public Guid Id { get; private set; }
        public string CustomerName { get; private set; }
        public List<string> Items { get; private set; } = new();
        public decimal TotalAmount { get; private set; }
        public string Status { get; private set; }
        public int Version { get; private set; }
        
        private readonly List<DomainEvent> _uncommittedEvents = new();

        public Order(Guid id)
        {
            Id = id;
        }

        // ✅ Factory method
        public static Order Create(string customerName, List<string> items, decimal totalAmount)
        {
            var order = new Order(Guid.NewGuid());
            order.Apply(new OrderCreatedEvent
            {
                AggregateId = order.Id,
                CustomerName = customerName,
                Items = items,
                TotalAmount = totalAmount,
                Version = 1
            });
            return order;
        }

        // ✅ Command handlers
        public void ChangeStatus(string newStatus)
        {
            if (Status == newStatus) return;

            Apply(new OrderStatusChangedEvent
            {
                AggregateId = Id,
                NewStatus = newStatus,
                Version = Version + 1
            });
        }

        // ✅ Apply event (for new events)
        private void Apply(DomainEvent @event)
        {
            When(@event);
            _uncommittedEvents.Add(@event);
        }

        // ✅ Load from history (for rebuilding state)
        public void LoadFromHistory(IEnumerable<DomainEvent> history)
        {
            foreach (var @event in history)
            {
                When(@event);
            }
        }

        // ✅ Event handlers - rebuild state
        private void When(DomainEvent @event)
        {
            switch (@event)
            {
                case OrderCreatedEvent e:
                    CustomerName = e.CustomerName;
                    Items = e.Items;
                    TotalAmount = e.TotalAmount;
                    Status = "Created";
                    Version = e.Version;
                    break;
                
                case OrderStatusChangedEvent e:
                    Status = e.NewStatus;
                    Version = e.Version;
                    break;
                
                case OrderItemAddedEvent e:
                    Items.Add(e.Item);
                    TotalAmount += e.Price;
                    Version = e.Version;
                    break;
            }
        }

        public IEnumerable<DomainEvent> GetUncommittedEvents() => _uncommittedEvents;
        public void MarkEventsAsCommitted() => _uncommittedEvents.Clear();
    }

    // ✅ Event Store - stores only events
    public interface IEventStore
    {
        Task SaveEventsAsync(Guid aggregateId, IEnumerable<DomainEvent> events, int expectedVersion, CancellationToken ct = default);
        Task<List<DomainEvent>> GetEventsAsync(Guid aggregateId, CancellationToken ct = default);
    }

    // ✅ Command handlers
    public class CreateOrderCommandHandler
    {
        private readonly IEventStore _eventStore;

        public CreateOrderCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<Guid> HandleAsync(CreateOrderCommand command, CancellationToken ct = default)
        {
            // ✅ Create aggregate
            var order = Order.Create(command.CustomerName, command.Items, command.TotalAmount);
            
            // ✅ Save events
            await _eventStore.SaveEventsAsync(
                order.Id, 
                order.GetUncommittedEvents(), 
                expectedVersion: 0, 
                ct);
            
            order.MarkEventsAsCommitted();
            return order.Id;
        }
    }

    public class ChangeOrderStatusCommandHandler
    {
        private readonly IEventStore _eventStore;

        public ChangeOrderStatusCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task HandleAsync(ChangeOrderStatusCommand command, CancellationToken ct = default)
        {
            // ✅ Load from event store
            var events = await _eventStore.GetEventsAsync(command.OrderId, ct);
            var order = new Order(command.OrderId);
            order.LoadFromHistory(events);

            // ✅ Execute command
            order.ChangeStatus(command.NewStatus);

            // ✅ Save new events
            await _eventStore.SaveEventsAsync(
                order.Id,
                order.GetUncommittedEvents(),
                expectedVersion: order.Version,
                ct);

            order.MarkEventsAsCommitted();
        }
    }

    // ✅ READ SIDE - Projections (Read Models)

    // Read model for queries
    public class OrderReadModel
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class OrderSummaryDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }

    // ✅ Projection - builds read model from events
    public interface IOrderProjection
    {
        Task ProjectAsync(DomainEvent @event, CancellationToken ct = default);
    }

    public class OrderReadModelProjection : IOrderProjection
    {
        private readonly IOrderReadRepository _readRepository;

        public OrderReadModelProjection(IOrderReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task ProjectAsync(DomainEvent @event, CancellationToken ct = default)
        {
            switch (@event)
            {
                case OrderCreatedEvent e:
                    await _readRepository.InsertAsync(new OrderReadModel
                    {
                        Id = e.AggregateId,
                        CustomerName = e.CustomerName,
                        ItemCount = e.Items.Count,
                        TotalAmount = e.TotalAmount,
                        Status = "Created",
                        CreatedAt = e.Timestamp
                    }, ct);
                    break;

                case OrderStatusChangedEvent e:
                    await _readRepository.UpdateStatusAsync(e.AggregateId, e.NewStatus, ct);
                    break;

                case OrderItemAddedEvent e:
                    await _readRepository.IncrementItemCountAsync(e.AggregateId, ct);
                    await _readRepository.AddToTotalAsync(e.AggregateId, e.Price, ct);
                    break;
            }
        }
    }

    // ✅ Query handlers
    public class GetOrderQuery
    {
        public Guid OrderId { get; set; }
    }

    public class GetOrdersByCustomerQuery
    {
        public string CustomerName { get; set; }
    }

    public interface IOrderReadRepository
    {
        Task InsertAsync(OrderReadModel order, CancellationToken ct = default);
        Task UpdateStatusAsync(Guid orderId, string status, CancellationToken ct = default);
        Task IncrementItemCountAsync(Guid orderId, CancellationToken ct = default);
        Task AddToTotalAsync(Guid orderId, decimal amount, CancellationToken ct = default);
        Task<OrderReadModel> GetByIdAsync(Guid orderId, CancellationToken ct = default);
        Task<List<OrderSummaryDto>> GetByCustomerAsync(string customerName, CancellationToken ct = default);
    }

    public class GetOrderQueryHandler
    {
        private readonly IOrderReadRepository _readRepository;

        public GetOrderQueryHandler(IOrderReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<OrderReadModel> HandleAsync(GetOrderQuery query, CancellationToken ct = default)
        {
            // ✅ Fast read from optimized read model
            return await _readRepository.GetByIdAsync(query.OrderId, ct);
        }
    }

    public class GetOrdersByCustomerQueryHandler
    {
        private readonly IOrderReadRepository _readRepository;

        public GetOrdersByCustomerQueryHandler(IOrderReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<List<OrderSummaryDto>> HandleAsync(GetOrdersByCustomerQuery query, CancellationToken ct = default)
        {
            // ✅ Optimized query with indexing
            return await _readRepository.GetByCustomerAsync(query.CustomerName, ct);
        }
    }

    // ✅ Benefits:
    // - Complete separation of write (events) and read (projections)
    // - Event store is append-only (optimized for writes)
    // - Read models are optimized for specific queries
    // - Can rebuild read models from events
    // - Independent scaling
    // - Temporal queries (rebuild state at any point in time)
}
