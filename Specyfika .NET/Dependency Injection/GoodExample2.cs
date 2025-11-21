// ✅ GOOD: Rozwiązywanie Circular Dependencies

// Rozwiązanie 1: Wprowadzenie trzeciego serwisu (mediator pattern)
public interface IServiceA
{
    void DoWorkA();
}

public interface IServiceB
{
    void DoWorkB();
}

// ✅ Współdzielony interfejs dla wspólnej funkcjonalności
public interface ICoordinator
{
    void CoordinateWork();
}

public class ServiceA : IServiceA
{
    private readonly ICoordinator _coordinator;
    
    // ✅ ServiceA zależy od Coordinator, nie od ServiceB
    public ServiceA(ICoordinator coordinator)
    {
        _coordinator = coordinator;
    }
    
    public void DoWorkA()
    {
        Console.WriteLine("ServiceA doing work");
        _coordinator.CoordinateWork();
    }
}

public class ServiceB : IServiceB
{
    private readonly ICoordinator _coordinator;
    
    // ✅ ServiceB także zależy od Coordinator
    public ServiceB(ICoordinator coordinator)
    {
        _coordinator = coordinator;
    }
    
    public void DoWorkB()
    {
        Console.WriteLine("ServiceB doing work");
        _coordinator.CoordinateWork();
    }
}

// ✅ Coordinator zarządza współpracą między ServiceA i ServiceB
public class WorkCoordinator : ICoordinator
{
    public void CoordinateWork()
    {
        Console.WriteLine("Coordinating work between services");
        // Logika koordynacji bez circular dependency
    }
}

// Rozwiązanie 2: Events/Messaging Pattern
public interface IEventBus
{
    void Publish<T>(T eventData);
    void Subscribe<T>(Action<T> handler);
}

public class OrderCreatedEvent
{
    public int OrderId { get; set; }
    public string CustomerEmail { get; set; }
}

// ✅ OrderService nie zależy bezpośrednio od NotificationService
public class OrderService
{
    private readonly IEventBus _eventBus;
    private readonly IOrderRepository _repository;
    
    public OrderService(IEventBus eventBus, IOrderRepository repository)
    {
        _eventBus = eventBus;
        _repository = repository;
    }
    
    public async Task CreateOrderAsync(Order order)
    {
        await _repository.SaveAsync(order);
        
        // ✅ Publish event - loose coupling
        _eventBus.Publish(new OrderCreatedEvent 
        { 
            OrderId = order.Id, 
            CustomerEmail = order.CustomerEmail 
        });
    }
}

// ✅ NotificationService subskrybuje eventy - nie ma circular dependency
public class NotificationService
{
    private readonly IEmailService _emailService;
    
    public NotificationService(IEventBus eventBus, IEmailService emailService)
    {
        _emailService = emailService;
        
        // ✅ Subscribe do eventów podczas inicjalizacji
        eventBus.Subscribe<OrderCreatedEvent>(OnOrderCreated);
    }
    
    private async void OnOrderCreated(OrderCreatedEvent evt)
    {
        await _emailService.SendConfirmationAsync(evt.CustomerEmail, evt.OrderId);
    }
}

// Rozwiązanie 3: Lazy<T> - opóźnione tworzenie zależności
public class ServiceWithLazy
{
    private readonly Lazy<IServiceB> _lazyServiceB;
    
    // ✅ Lazy<T> opóźnia utworzenie instancji do pierwszego użycia
    public ServiceWithLazy(Lazy<IServiceB> lazyServiceB)
    {
        _lazyServiceB = lazyServiceB;
    }
    
    public void DoWork()
    {
        // ✅ ServiceB jest tworzony dopiero tutaj, gdy jest potrzebny
        _lazyServiceB.Value.DoWorkB();
    }
}

// Rozwiązanie 4: Property Injection (używaj ostrożnie!)
public class ServiceWithPropertyInjection : IServiceA
{
    // ✅ Property injection - unikaj circular dependency w konstruktorze
    public IServiceB ServiceB { get; set; }
    
    public void DoWorkA()
    {
        ServiceB?.DoWorkB();
    }
}

// ✅ Konfiguracja DI
public class Program
{
    public static void Main()
    {
        var services = new ServiceCollection();
        
        // ✅ Rozwiązanie 1: Mediator
        services.AddScoped<IServiceA, ServiceA>();
        services.AddScoped<IServiceB, ServiceB>();
        services.AddScoped<ICoordinator, WorkCoordinator>();
        
        // ✅ Rozwiązanie 2: Event Bus
        services.AddSingleton<IEventBus, InMemoryEventBus>();
        services.AddScoped<OrderService>();
        services.AddScoped<NotificationService>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddSingleton<IEmailService, EmailService>();
        
        // ✅ Rozwiązanie 3: Lazy<T> - automatycznie wspierane przez DI
        services.AddScoped<ServiceWithLazy>();
        
        var provider = services.BuildServiceProvider();
        
        // ✅ Wszystko działa bez circular dependency error
        var serviceA = provider.GetService<IServiceA>();
        serviceA.DoWorkA();
    }
}

// Implementacja prostego Event Bus
public class InMemoryEventBus : IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();
    
    public void Publish<T>(T eventData)
    {
        var eventType = typeof(T);
        if (_handlers.ContainsKey(eventType))
        {
            foreach (var handler in _handlers[eventType])
            {
                ((Action<T>)handler)(eventData);
            }
        }
    }
    
    public void Subscribe<T>(Action<T> handler)
    {
        var eventType = typeof(T);
        if (!_handlers.ContainsKey(eventType))
        {
            _handlers[eventType] = new List<Delegate>();
        }
        _handlers[eventType].Add(handler);
    }
}

public interface IOrderRepository
{
    Task SaveAsync(Order order);
}

public class OrderRepository : IOrderRepository
{
    public Task SaveAsync(Order order)
    {
        // Save logic
        return Task.CompletedTask;
    }
}

public interface IEmailService
{
    Task SendConfirmationAsync(string email, int orderId);
}

public class EmailService : IEmailService
{
    public Task SendConfirmationAsync(string email, int orderId)
    {
        Console.WriteLine($"Sending confirmation to {email} for order {orderId}");
        return Task.CompletedTask;
    }
}

public class Order
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; }
}
