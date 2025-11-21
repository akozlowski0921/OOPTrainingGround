// ❌ BAD: Circular Dependency - ServiceA → ServiceB → ServiceA

public interface IServiceA
{
    void DoWorkA();
}

public interface IServiceB
{
    void DoWorkB();
}

// ❌ Problem: Circular dependency - A needs B, B needs A
public class ServiceA : IServiceA
{
    private readonly IServiceB _serviceB;
    
    // ❌ ServiceA wymaga ServiceB
    public ServiceA(IServiceB serviceB)
    {
        _serviceB = serviceB;
    }
    
    public void DoWorkA()
    {
        Console.WriteLine("ServiceA doing work");
        _serviceB.DoWorkB(); // Wywołuje ServiceB
    }
}

public class ServiceB : IServiceB
{
    private readonly IServiceA _serviceA;
    
    // ❌ ServiceB wymaga ServiceA - powstaje circular dependency!
    public ServiceB(IServiceA serviceA)
    {
        _serviceA = serviceA;
    }
    
    public void DoWorkB()
    {
        Console.WriteLine("ServiceB doing work");
        _serviceA.DoWorkA(); // Wywołuje ServiceA - nieskończona rekurencja!
    }
}

// ❌ To spowoduje błąd w runtime:
// "A circular dependency was detected for the service of type 'IServiceA'"
public class Program
{
    public static void Main()
    {
        var services = new ServiceCollection();
        
        services.AddScoped<IServiceA, ServiceA>();
        services.AddScoped<IServiceB, ServiceB>();
        
        var provider = services.BuildServiceProvider();
        
        // ❌ Runtime error - DI container nie może rozwiązać circular dependency
        var serviceA = provider.GetService<IServiceA>();
    }
}

// ❌ BAD: Tight coupling między serwisami
public class OrderService
{
    private readonly NotificationService _notificationService;
    
    public OrderService(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    
    public void CreateOrder(Order order)
    {
        // Logika tworzenia zamówienia
        _notificationService.NotifyOrderCreated(order);
    }
}

public class NotificationService
{
    private readonly OrderService _orderService;
    
    // ❌ NotificationService potrzebuje OrderService - circular!
    public NotificationService(OrderService orderService)
    {
        _orderService = orderService;
    }
    
    public void NotifyOrderCreated(Order order)
    {
        // Wysyłanie notyfikacji
        // Czasem potrzebuje sprawdzić status zamówienia
        // _orderService.GetOrderStatus(order.Id);
    }
}

public class Order
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; }
}
