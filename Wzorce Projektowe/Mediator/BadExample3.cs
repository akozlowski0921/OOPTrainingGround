using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignPatterns.Mediator.Bad3
{
    // ❌ BAD: No mediator for CQRS/Event Sourcing - direct coupling

    public class OrderAggregate
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public List<string> Events { get; set; } = new();

        // ❌ Direct dependencies on multiple components
        private readonly EventStore _eventStore;
        private readonly OrderReadRepository _readRepository;
        private readonly NotificationService _notificationService;
        private readonly AnalyticsService _analyticsService;

        public OrderAggregate(
            EventStore eventStore,
            OrderReadRepository readRepo,
            NotificationService notificationService,
            AnalyticsService analyticsService)
        {
            _eventStore = eventStore;
            _readRepository = readRepo;
            _notificationService = notificationService;
            _analyticsService = analyticsService;
            // ❌ Aggregate zna wszystkie systemy
        }

        public void PlaceOrder()
        {
            Status = "Placed";
            Events.Add("OrderPlaced");

            // ❌ Aggregate odpowiada za zapis eventów
            _eventStore.Save(Events);

            // ❌ Aggregate aktualizuje read model
            _readRepository.UpdateOrderStatus(OrderId, "Placed");

            // ❌ Aggregate wysyła notyfikacje
            _notificationService.SendOrderConfirmation(OrderId);

            // ❌ Aggregate aktualizuje analytics
            _analyticsService.RecordOrderPlaced(OrderId);

            // ❌ PROBLEMY:
            // - Aggregate ma zbyt wiele odpowiedzialności
            // - Tight coupling z infrastrukturą
            // - Trudne testowanie
            // - Naruszenie SRP
        }
    }

    public class EventStore
    {
        private readonly List<string> _events = new();

        public void Save(List<string> events)
        {
            _events.AddRange(events);
            // ❌ EventStore bezpośrednio aktualizuje projections
            UpdateAllProjections(events);
        }

        private void UpdateAllProjections(List<string> events)
        {
            // ❌ Hard-coded projection updates
            Console.WriteLine("Updating projections...");
        }
    }

    public class OrderReadRepository
    {
        public void UpdateOrderStatus(int orderId, string status)
        {
            Console.WriteLine($"Updated order {orderId} to {status}");
        }
    }

    public class NotificationService
    {
        public void SendOrderConfirmation(int orderId)
        {
            Console.WriteLine($"Sent confirmation for order {orderId}");
        }
    }

    public class AnalyticsService
    {
        public void RecordOrderPlaced(int orderId)
        {
            Console.WriteLine($"Recorded analytics for order {orderId}");
        }
    }

    // ❌ PROBLEMY:
    // - Brak centralizacji przepływu wiadomości
    // - Tight coupling między write side i read side
    // - Aggregate wie o read models
    // - Brak separation of concerns
    // - Trudne dodawanie nowych projections
    // - Trudne skalowanie
}
