using System;
using Microsoft.Extensions.DependencyInjection;

namespace SpecyfikaDotNet.DependencyInjection
{
    // ❌ BAD: Wstrzyknięcie Scoped Service (DbContext) do Singletona
    public class BadOrderProcessor
    {
        private readonly AppDbContext _context; // BŁĄD: DbContext powinien być Scoped, nie Singleton

        // Constructor Injection w Singletonie
        public BadOrderProcessor(AppDbContext context)
        {
            _context = context; // Ten sam DbContext będzie żył przez cały cykl życia aplikacji
        }

        public void ProcessOrder(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            // Przetwarzanie zamówienia...
            
            // PROBLEM: DbContext jest thread-unsafe i przechowuje cache
            // Współbieżne zapytania mogą prowadzić do wyjątków lub niepoprawnych danych
        }
    }

    public class BadStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // DbContext z lifetime Scoped
            services.AddScoped<AppDbContext>();
            
            // OrderProcessor jako Singleton - BŁĄD!
            // Singleton żyje przez cały czas działania aplikacji
            // i będzie trzymać tę samą instancję DbContext
            services.AddSingleton<BadOrderProcessor>();
        }
    }

    // Uproszczone klasy dla przykładu
    public class AppDbContext
    {
        public IQueryable<Order> Orders { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
    }
}
