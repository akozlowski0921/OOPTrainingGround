using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignPatterns.EventSourcing.Bad3
{
    // ❌ BAD: No integration with CQRS - missing the synergy

    public class Order
    {
        public int OrderId { get; set; }
        public string CustomerId { get; set; }
        public List<OrderLine> Lines { get; set; } = new();
        public decimal Total { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
    }

    public class OrderLine
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    // ❌ Single service for everything
    public class OrderService
    {
        private readonly List<Order> _orders = new();

        // ❌ Write operation
        public void CreateOrder(string customerId, List<OrderLine> lines)
        {
            var order = new Order
            {
                OrderId = _orders.Count + 1,
                CustomerId = customerId,
                Lines = lines,
                Total = lines.Sum(l => l.Price * l.Quantity),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };
            _orders.Add(order);
            // ❌ Direct state update, no events
            // ❌ Same model for reads and writes
        }

        // ❌ Query - loads entire order
        public Order GetOrder(int orderId)
        {
            return _orders.FirstOrDefault(o => o.OrderId == orderId);
            // ❌ Returns full entity even if we only need summary
            // ❌ No caching
            // ❌ No optimization
        }

        // ❌ Query - expensive aggregation
        public List<Order> GetOrdersByCustomer(string customerId)
        {
            return _orders.Where(o => o.CustomerId == customerId).ToList();
            // ❌ Loads all orders with all lines
            // ❌ No pagination
        }

        // ❌ Write operation
        public void UpdateOrderStatus(int orderId, string newStatus)
        {
            var order = _orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                order.Status = newStatus;
                if (newStatus == "Shipped")
                    order.ShippedAt = DateTime.UtcNow;
            }
            // ❌ Brak eventów - straciliśmy informację o zmianie
            // ❌ Nie możemy odbudować historii statusów
        }

        // ❌ Complex query mixed with business logic
        public Dictionary<string, int> GetOrderCountByStatus()
        {
            return _orders.GroupBy(o => o.Status)
                .ToDictionary(g => g.Key, g => g.Count());
            // ❌ Expensive in-memory aggregation
            // ❌ No caching of statistics
        }

        // ❌ Reporting query
        public decimal GetTotalRevenueByCustomer(string customerId)
        {
            return _orders
                .Where(o => o.CustomerId == customerId && o.Status == "Shipped")
                .Sum(o => o.Total);
            // ❌ Scans all orders
            // ❌ No denormalized view for reports
        }
    }

    // ❌ PROBLEMS:
    // - No event sourcing = no audit trail, no history
    // - No CQRS = reads and writes use same model
    // - Queries load entire aggregates when only summary needed
    // - No caching strategy
    // - No separate optimization for reads vs writes
    // - Reporting queries are expensive
    // - Cannot scale reads independently from writes
    // - No event-driven architecture for notifications
    // - Missing the synergy between Event Sourcing and CQRS
}
