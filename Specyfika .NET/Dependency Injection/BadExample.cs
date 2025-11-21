// ❌ BAD: Ręczne tworzenie zależności - tight coupling, brak testowalności

public class OrderController
{
    private readonly OrderService _orderService;
    
    public OrderController()
    {
        // ❌ Problem 1: Bezpośrednie tworzenie zależności
        // - Tight coupling do konkretnych implementacji
        // - Niemożliwe do mockowania w testach
        // - Trudne do zmiany implementacji
        var dbConnection = new SqlConnection("Server=localhost;Database=Shop");
        var orderRepository = new OrderRepository(dbConnection);
        var emailService = new SmtpEmailService("smtp.gmail.com");
        
        _orderService = new OrderService(orderRepository, emailService);
    }
    
    public IActionResult CreateOrder(Order order)
    {
        try
        {
            _orderService.ProcessOrder(order);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class OrderService
{
    private readonly OrderRepository _repository;
    private readonly SmtpEmailService _emailService;
    
    public OrderService(OrderRepository repository, SmtpEmailService emailService)
    {
        // ❌ Problem 2: Zależność od konkretnej implementacji
        // - Nie używamy interfejsów
        // - Niemożliwe do podstawienia innej implementacji
        _repository = repository;
        _emailService = emailService;
    }
    
    public void ProcessOrder(Order order)
    {
        // ❌ Problem 3: Tworzenie nowych instancji w środku metody
        var validator = new OrderValidator(); // Nowa instancja za każdym razem
        
        if (!validator.Validate(order))
            throw new InvalidOperationException("Invalid order");
            
        _repository.Save(order);
        _emailService.SendConfirmation(order.CustomerEmail);
    }
}

public class OrderRepository
{
    private readonly SqlConnection _connection;
    
    public OrderRepository(SqlConnection connection)
    {
        _connection = connection;
    }
    
    public void Save(Order order)
    {
        // ❌ Problem 4: Brak zarządzania czasem życia połączenia
        // - Connection może być już zamknięte
        // - Brak proper disposal
        _connection.Open();
        // ... save logic
        _connection.Close();
    }
}

public class SmtpEmailService
{
    private readonly string _smtpServer;
    
    public SmtpEmailService(string smtpServer)
    {
        // ❌ Problem 5: Hardcoded configuration
        _smtpServer = smtpServer;
    }
    
    public void SendConfirmation(string email)
    {
        // ... email logic
    }
}

public class OrderValidator
{
    public bool Validate(Order order)
    {
        return order.Quantity > 0 && !string.IsNullOrEmpty(order.CustomerEmail);
    }
}

public class Order
{
    public int Quantity { get; set; }
    public string CustomerEmail { get; set; }
}
