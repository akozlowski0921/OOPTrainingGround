using System;
using Microsoft.Extensions.DependencyInjection;

namespace SpecyfikaDotNet.DependencyInjection
{
    // ✅ GOOD: Użycie IServiceScopeFactory do tworzenia scope'ów
    public class GoodOrderProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        // Wstrzykujemy factory zamiast bezpośrednio DbContext
        public GoodOrderProcessor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void ProcessOrder(int orderId)
        {
            // Tworzymy nowy scope dla każdej operacji
            using (var scope = _scopeFactory.CreateScope())
            {
                // Pobieramy świeżą instancję DbContext z tego scope'a
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                var order = context.Orders.Find(orderId);
                // Przetwarzanie zamówienia...
                
                // DbContext zostanie automatycznie zwolniony po wyjściu z using
            }
        }

        public void ProcessMultipleOrders(int[] orderIds)
        {
            foreach (var orderId in orderIds)
            {
                // Każda iteracja tworzy nowy scope z nowym DbContext
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var order = context.Orders.Find(orderId);
                    // Przetwarzanie...
                }
            }
        }
    }

    public class GoodStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // DbContext pozostaje Scoped - to jest poprawne
            services.AddScoped<AppDbContext>();
            
            // OrderProcessor jako Singleton - OK, bo używa IServiceScopeFactory
            services.AddSingleton<GoodOrderProcessor>();
            
            // IServiceScopeFactory jest automatycznie dostępne w DI
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
