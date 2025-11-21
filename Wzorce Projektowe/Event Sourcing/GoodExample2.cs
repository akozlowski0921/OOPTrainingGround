using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DesignPatterns.EventSourcing.Good2
{
    // ✅ GOOD: Event Sourcing - full state reconstruction with projections

    // ✅ Domain Events
    public abstract class CartEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public Guid CartId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Version { get; set; }
    }

    public class CartCreatedEvent : CartEvent
    {
        public string CustomerId { get; set; }
    }

    public class ItemAddedEvent : CartEvent
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ItemRemovedEvent : CartEvent
    {
        public int ProductId { get; set; }
        public string Reason { get; set; }
    }

    public class QuantityChangedEvent : CartEvent
    {
        public int ProductId { get; set; }
        public int OldQuantity { get; set; }
        public int NewQuantity { get; set; }
    }

    public class CartCheckedOutEvent : CartEvent
    {
        public decimal TotalAmount { get; set; }
    }

    // ✅ Aggregate - Shopping Cart
    public class ShoppingCart
    {
        public Guid CartId { get; private set; }
        public string CustomerId { get; private set; }
        public Dictionary<int, CartItem> Items { get; private set; } = new();
        public int Version { get; private set; }
        public bool IsCheckedOut { get; private set; }

        private readonly List<CartEvent> _uncommittedEvents = new();

        public class CartItem
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }

        private ShoppingCart() { }

        // ✅ Factory
        public static ShoppingCart Create(string customerId)
        {
            var cart = new ShoppingCart();
            cart.Apply(new CartCreatedEvent
            {
                CartId = Guid.NewGuid(),
                CustomerId = customerId,
                Version = 1
            });
            return cart;
        }

        // ✅ Commands
        public void AddItem(int productId, string productName, int quantity, decimal price)
        {
            if (IsCheckedOut)
                throw new InvalidOperationException("Cannot modify checked out cart");

            Apply(new ItemAddedEvent
            {
                CartId = CartId,
                ProductId = productId,
                ProductName = productName,
                Quantity = quantity,
                Price = price,
                Version = Version + 1
            });
        }

        public void RemoveItem(int productId, string reason)
        {
            if (!Items.ContainsKey(productId))
                throw new InvalidOperationException("Item not in cart");

            if (IsCheckedOut)
                throw new InvalidOperationException("Cannot modify checked out cart");

            Apply(new ItemRemovedEvent
            {
                CartId = CartId,
                ProductId = productId,
                Reason = reason,
                Version = Version + 1
            });
        }

        public void ChangeQuantity(int productId, int newQuantity)
        {
            if (!Items.ContainsKey(productId))
                throw new InvalidOperationException("Item not in cart");

            if (IsCheckedOut)
                throw new InvalidOperationException("Cannot modify checked out cart");

            var oldQuantity = Items[productId].Quantity;

            Apply(new QuantityChangedEvent
            {
                CartId = CartId,
                ProductId = productId,
                OldQuantity = oldQuantity,
                NewQuantity = newQuantity,
                Version = Version + 1
            });
        }

        public void Checkout()
        {
            if (IsCheckedOut)
                throw new InvalidOperationException("Cart already checked out");

            if (!Items.Any())
                throw new InvalidOperationException("Cannot checkout empty cart");

            var total = Items.Values.Sum(i => i.Price * i.Quantity);

            Apply(new CartCheckedOutEvent
            {
                CartId = CartId,
                TotalAmount = total,
                Version = Version + 1
            });
        }

        // ✅ Apply new event
        private void Apply(CartEvent @event)
        {
            When(@event);
            _uncommittedEvents.Add(@event);
        }

        // ✅ Load from history
        public static ShoppingCart LoadFromHistory(IEnumerable<CartEvent> history)
        {
            var cart = new ShoppingCart();
            foreach (var @event in history)
            {
                cart.When(@event);
            }
            return cart;
        }

        // ✅ Rebuild state at specific point in time
        public static ShoppingCart LoadFromHistoryUntil(IEnumerable<CartEvent> history, DateTime pointInTime)
        {
            var cart = new ShoppingCart();
            foreach (var @event in history.Where(e => e.Timestamp <= pointInTime))
            {
                cart.When(@event);
            }
            return cart;
        }

        // ✅ Event handlers - rebuild state
        private void When(CartEvent @event)
        {
            switch (@event)
            {
                case CartCreatedEvent e:
                    CartId = e.CartId;
                    CustomerId = e.CustomerId;
                    Version = e.Version;
                    break;

                case ItemAddedEvent e:
                    if (Items.ContainsKey(e.ProductId))
                    {
                        Items[e.ProductId].Quantity += e.Quantity;
                    }
                    else
                    {
                        Items[e.ProductId] = new CartItem
                        {
                            ProductId = e.ProductId,
                            ProductName = e.ProductName,
                            Quantity = e.Quantity,
                            Price = e.Price
                        };
                    }
                    Version = e.Version;
                    break;

                case ItemRemovedEvent e:
                    Items.Remove(e.ProductId);
                    Version = e.Version;
                    break;

                case QuantityChangedEvent e:
                    if (Items.ContainsKey(e.ProductId))
                    {
                        Items[e.ProductId].Quantity = e.NewQuantity;
                    }
                    Version = e.Version;
                    break;

                case CartCheckedOutEvent e:
                    IsCheckedOut = true;
                    Version = e.Version;
                    break;
            }
        }

        public IEnumerable<CartEvent> GetUncommittedEvents() => _uncommittedEvents;
        public void MarkEventsAsCommitted() => _uncommittedEvents.Clear();

        public decimal GetTotal() => Items.Values.Sum(i => i.Price * i.Quantity);
    }

    // ✅ Event Store interface
    public interface ICartEventStore
    {
        Task SaveEventsAsync(Guid cartId, IEnumerable<CartEvent> events, int expectedVersion, CancellationToken ct = default);
        Task<List<CartEvent>> GetEventsAsync(Guid cartId, CancellationToken ct = default);
        Task<List<CartEvent>> GetEventsUntilAsync(Guid cartId, DateTime pointInTime, CancellationToken ct = default);
    }

    // ✅ Projections - different views of the same events

    // Projection 1: Current cart state (for display)
    public class CartReadModel
    {
        public Guid CartId { get; set; }
        public string CustomerId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal Total { get; set; }
        public bool IsCheckedOut { get; set; }
        public DateTime LastModified { get; set; }
    }

    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    // Projection 2: Cart analytics (for business insights)
    public class CartAnalytics
    {
        public Guid CartId { get; set; }
        public int TotalItemsAdded { get; set; }
        public int TotalItemsRemoved { get; set; }
        public int QuantityChanges { get; set; }
        public List<int> AbandonedProductIds { get; set; } = new(); // Added but removed
        public TimeSpan TimeToCheckout { get; set; }
    }

    // Projection 3: Audit trail (for compliance)
    public class CartAuditEntry
    {
        public Guid EventId { get; set; }
        public Guid CartId { get; set; }
        public string EventType { get; set; }
        public DateTime Timestamp { get; set; }
        public string Details { get; set; }
    }

    // ✅ Projection builder - builds read models from events
    public class CartProjectionBuilder
    {
        // ✅ Build current state projection
        public static CartReadModel BuildCurrentState(IEnumerable<CartEvent> events)
        {
            var cart = ShoppingCart.LoadFromHistory(events);
            
            return new CartReadModel
            {
                CartId = cart.CartId,
                CustomerId = cart.CustomerId,
                Items = cart.Items.Values.Select(i => new CartItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList(),
                Total = cart.GetTotal(),
                IsCheckedOut = cart.IsCheckedOut,
                LastModified = events.Max(e => e.Timestamp)
            };
        }

        // ✅ Build analytics projection
        public static CartAnalytics BuildAnalytics(IEnumerable<CartEvent> events)
        {
            var eventList = events.ToList();
            var analytics = new CartAnalytics
            {
                CartId = eventList.First().CartId
            };

            var addedProducts = new HashSet<int>();
            var finalProducts = new HashSet<int>();

            foreach (var @event in eventList)
            {
                switch (@event)
                {
                    case ItemAddedEvent e:
                        analytics.TotalItemsAdded++;
                        addedProducts.Add(e.ProductId);
                        finalProducts.Add(e.ProductId);
                        break;

                    case ItemRemovedEvent e:
                        analytics.TotalItemsRemoved++;
                        finalProducts.Remove(e.ProductId);
                        break;

                    case QuantityChangedEvent e:
                        analytics.QuantityChanges++;
                        break;

                    case CartCheckedOutEvent e:
                        var createdEvent = eventList.OfType<CartCreatedEvent>().First();
                        analytics.TimeToCheckout = e.Timestamp - createdEvent.Timestamp;
                        break;
                }
            }

            // Products added but not in final cart = abandoned
            analytics.AbandonedProductIds = addedProducts.Except(finalProducts).ToList();

            return analytics;
        }

        // ✅ Build audit trail projection
        public static List<CartAuditEntry> BuildAuditTrail(IEnumerable<CartEvent> events)
        {
            return events.Select(@event => new CartAuditEntry
            {
                EventId = @event.EventId,
                CartId = @event.CartId,
                EventType = @event.GetType().Name,
                Timestamp = @event.Timestamp,
                Details = GetEventDetails(@event)
            }).ToList();
        }

        private static string GetEventDetails(CartEvent @event)
        {
            return @event switch
            {
                ItemAddedEvent e => $"Added {e.Quantity}x {e.ProductName} (${e.Price})",
                ItemRemovedEvent e => $"Removed product {e.ProductId}. Reason: {e.Reason}",
                QuantityChangedEvent e => $"Changed quantity for product {e.ProductId} from {e.OldQuantity} to {e.NewQuantity}",
                CartCheckedOutEvent e => $"Checked out. Total: ${e.TotalAmount}",
                _ => @event.GetType().Name
            };
        }
    }

    // ✅ Repository
    public class ShoppingCartRepository
    {
        private readonly ICartEventStore _eventStore;

        public ShoppingCartRepository(ICartEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<ShoppingCart> GetByIdAsync(Guid cartId, CancellationToken ct = default)
        {
            var events = await _eventStore.GetEventsAsync(cartId, ct);
            if (!events.Any())
                return null;

            return ShoppingCart.LoadFromHistory(events);
        }

        // ✅ Temporal query - get state at specific time
        public async Task<ShoppingCart> GetByIdAtTimeAsync(Guid cartId, DateTime pointInTime, CancellationToken ct = default)
        {
            var events = await _eventStore.GetEventsUntilAsync(cartId, pointInTime, ct);
            if (!events.Any())
                return null;

            return ShoppingCart.LoadFromHistoryUntil(events, pointInTime);
        }

        public async Task SaveAsync(ShoppingCart cart, CancellationToken ct = default)
        {
            var newEvents = cart.GetUncommittedEvents();
            if (!newEvents.Any())
                return;

            await _eventStore.SaveEventsAsync(
                cart.CartId,
                newEvents,
                cart.Version - newEvents.Count(),
                ct);

            cart.MarkEventsAsCommitted();
        }
    }

    // ✅ Benefits:
    // - Complete reconstruction of state from events
    // - Temporal queries (state at any point in time)
    // - Multiple projections from same events
    // - Business analytics from event stream
    // - Full audit trail for compliance
    // - Debugging by replaying events
}
