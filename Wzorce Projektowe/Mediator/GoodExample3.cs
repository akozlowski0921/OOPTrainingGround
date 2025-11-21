using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DesignPatterns.Mediator.Good3
{
    // ✅ GOOD: Mediator for CQRS/Event Sourcing - MediatR style

    // ✅ Domain Events
    public abstract class DomainEvent
    {
        public Guid AggregateId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class OrderPlacedEvent : DomainEvent
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }

    public class OrderShippedEvent : DomainEvent
    {
        public int OrderId { get; set; }
        public string TrackingNumber { get; set; }
    }

    // ✅ Commands
    public interface ICommand { }

    public class PlaceOrderCommand : ICommand
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }

    // ✅ Queries
    public interface IQuery<TResult> { }

    public class GetOrderQuery : IQuery<OrderDto>
    {
        public int OrderId { get; set; }
    }

    public class OrderDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
    }

    // ✅ Mediator interface (like MediatR)
    public interface IMediator
    {
        Task<TResult> SendAsync<TResult>(ICommand command, CancellationToken ct = default);
        Task<TResult> SendAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default);
        Task PublishAsync(DomainEvent domainEvent, CancellationToken ct = default);
    }

    // ✅ Handler interfaces
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command, CancellationToken ct = default);
    }

    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query, CancellationToken ct = default);
    }

    public interface IEventHandler<TEvent> where TEvent : DomainEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken ct = default);
    }

    // ✅ Mediator implementation
    public class CqrsMediator : IMediator
    {
        private readonly Dictionary<Type, object> _commandHandlers = new();
        private readonly Dictionary<Type, object> _queryHandlers = new();
        private readonly Dictionary<Type, List<object>> _eventHandlers = new();

        public void RegisterCommandHandler<TCommand>(ICommandHandler<TCommand> handler) 
            where TCommand : ICommand
        {
            _commandHandlers[typeof(TCommand)] = handler;
        }

        public void RegisterQueryHandler<TQuery, TResult>(IQueryHandler<TQuery, TResult> handler)
            where TQuery : IQuery<TResult>
        {
            _queryHandlers[typeof(TQuery)] = handler;
        }

        public void RegisterEventHandler<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : DomainEvent
        {
            var eventType = typeof(TEvent);
            if (!_eventHandlers.ContainsKey(eventType))
            {
                _eventHandlers[eventType] = new List<object>();
            }
            _eventHandlers[eventType].Add(handler);
        }

        // ✅ Send command (write operation)
        public async Task<TResult> SendAsync<TResult>(ICommand command, CancellationToken ct = default)
        {
            var commandType = command.GetType();
            if (_commandHandlers.TryGetValue(commandType, out var handler))
            {
                var method = handler.GetType().GetMethod("HandleAsync");
                var task = (Task)method.Invoke(handler, new object[] { command, ct });
                await task;
                return default(TResult);
            }
            throw new InvalidOperationException($"No handler for command {commandType.Name}");
        }

        // ✅ Send query (read operation)
        public async Task<TResult> SendAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default)
        {
            var queryType = query.GetType();
            if (_queryHandlers.TryGetValue(queryType, out var handler))
            {
                var typedHandler = (IQueryHandler<IQuery<TResult>, TResult>)handler;
                return await typedHandler.HandleAsync(query, ct);
            }
            throw new InvalidOperationException($"No handler for query {queryType.Name}");
        }

        // ✅ Publish event (to multiple handlers)
        public async Task PublishAsync(DomainEvent domainEvent, CancellationToken ct = default)
        {
            var eventType = domainEvent.GetType();
            if (_eventHandlers.TryGetValue(eventType, out var handlers))
            {
                var tasks = new List<Task>();
                foreach (var handler in handlers)
                {
                    var method = handler.GetType().GetMethod("HandleAsync");
                    var task = (Task)method.Invoke(handler, new object[] { domainEvent, ct });
                    tasks.Add(task);
                }
                await Task.WhenAll(tasks);
            }
        }
    }

    // ✅ WRITE SIDE - Command Handler
    public class PlaceOrderCommandHandler : ICommandHandler<PlaceOrderCommand>
    {
        private readonly IMediator _mediator;

        public PlaceOrderCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task HandleAsync(PlaceOrderCommand command, CancellationToken ct = default)
        {
            Console.WriteLine($"[Command] Placing order {command.OrderId}");
            
            // ✅ Business logic here
            // Save to event store...
            
            // ✅ Publish domain event through mediator
            await _mediator.PublishAsync(new OrderPlacedEvent
            {
                OrderId = command.OrderId,
                Amount = command.Amount
            }, ct);
        }
    }

    // ✅ READ SIDE - Query Handler
    public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, OrderDto>
    {
        public Task<OrderDto> HandleAsync(GetOrderQuery query, CancellationToken ct = default)
        {
            Console.WriteLine($"[Query] Getting order {query.OrderId}");
            
            // ✅ Read from read model (fast, cached, denormalized)
            return Task.FromResult(new OrderDto
            {
                OrderId = query.OrderId,
                Status = "Placed",
                Amount = 100m
            });
        }
    }

    // ✅ PROJECTIONS - Event Handlers (update read models)
    public class OrderReadModelProjection : IEventHandler<OrderPlacedEvent>
    {
        public Task HandleAsync(OrderPlacedEvent @event, CancellationToken ct = default)
        {
            Console.WriteLine($"[Projection] Updating read model for order {@event.OrderId}");
            // ✅ Update read model database
            return Task.CompletedTask;
        }
    }

    public class OrderAnalyticsProjection : IEventHandler<OrderPlacedEvent>
    {
        public Task HandleAsync(OrderPlacedEvent @event, CancellationToken ct = default)
        {
            Console.WriteLine($"[Analytics] Recording order {@event.OrderId}");
            // ✅ Update analytics database
            return Task.CompletedTask;
        }
    }

    public class NotificationHandler : IEventHandler<OrderPlacedEvent>, IEventHandler<OrderShippedEvent>
    {
        public Task HandleAsync(OrderPlacedEvent @event, CancellationToken ct = default)
        {
            Console.WriteLine($"[Notification] Sending order confirmation for {@event.OrderId}");
            return Task.CompletedTask;
        }

        public Task HandleAsync(OrderShippedEvent @event, CancellationToken ct = default)
        {
            Console.WriteLine($"[Notification] Sending shipping notification for {@event.OrderId}");
            return Task.CompletedTask;
        }
    }

    // ✅ Usage example
    public class CqrsMediatorExample
    {
        public static async Task RunAsync()
        {
            var mediator = new CqrsMediator();

            // ✅ Register handlers
            mediator.RegisterCommandHandler(new PlaceOrderCommandHandler(mediator));
            mediator.RegisterQueryHandler<GetOrderQuery, OrderDto>(new GetOrderQueryHandler());
            mediator.RegisterEventHandler(new OrderReadModelProjection());
            mediator.RegisterEventHandler(new OrderAnalyticsProjection());
            mediator.RegisterEventHandler(new NotificationHandler());

            // ✅ Execute command (write)
            await mediator.SendAsync<object>(new PlaceOrderCommand
            {
                OrderId = 123,
                Amount = 100m
            });

            // ✅ Execute query (read)
            var order = await mediator.SendAsync(new GetOrderQuery { OrderId = 123 });
            Console.WriteLine($"Order status: {order.Status}");

            // ✅ Events automatically propagate to all handlers
            await Task.Delay(100);
        }
    }

    // ✅ Benefits:
    // - Centralized message flow (all through mediator)
    // - Complete separation: Commands, Queries, Events
    // - Loose coupling (handlers don't know about each other)
    // - Easy to add new projections (just register handler)
    // - Event-driven architecture
    // - Testable (mock mediator)
    // - Scales well (add handlers without changing existing code)
}
