using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignPatterns.CQRS.Bad2
{
    // ❌ BAD: No separation between write and read operations with Event Sourcing

    public class OrderEvent
    {
        public string EventType { get; set; }
        public DateTime Timestamp { get; set; }
        public object Data { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public List<string> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<OrderEvent> Events { get; set; } = new();
    }

    // ❌ Single model for both event storage and querying
    public class OrderService
    {
        private readonly List<Order> _orders = new();

        // ❌ Write operation stores events but also updates the model directly
        public void CreateOrder(string customerName, List<string> items, decimal amount)
        {
            var order = new Order
            {
                Id = _orders.Count + 1,
                CustomerName = customerName,
                Items = items,
                TotalAmount = amount,
                Status = "Created"
            };

            // ❌ Storing event but also modifying state directly
            var evt = new OrderEvent
            {
                EventType = "OrderCreated",
                Timestamp = DateTime.UtcNow,
                Data = new { customerName, items, amount }
            };
            order.Events.Add(evt);

            _orders.Add(order);
            // ❌ No clear separation between event log and current state
        }

        // ❌ Query needs to load all events even if we just need current state
        public Order GetOrder(int id)
        {
            return _orders.FirstOrDefault(o => o.Id == id);
            // ❌ Inefficient - loads entire order with all events
            // ❌ No read model optimization
        }

        // ❌ Query that needs to scan all events
        public List<Order> GetOrdersByCustomer(string customerName)
        {
            return _orders.Where(o => o.CustomerName == customerName).ToList();
            // ❌ No indexing or optimization for queries
            // ❌ Returns full orders with all events
        }

        // ❌ Update operation
        public void UpdateOrderStatus(int orderId, string newStatus)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = newStatus;
                order.Events.Add(new OrderEvent
                {
                    EventType = "StatusChanged",
                    Timestamp = DateTime.UtcNow,
                    Data = new { newStatus }
                });
            }
            // ❌ Mixing event sourcing with direct state updates
        }
    }

    // ❌ PROBLEMS:
    // - Events stored alongside current state (redundancy)
    // - No separation of event store from read model
    // - Queries are not optimized
    // - Difficult to rebuild state from events
    // - No independent scaling of reads/writes
}
