using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignPatterns.Facade.Good2
{
    // ✅ GOOD: Facade for legacy system - wraps old API with modern interface

    // ✅ Legacy systems (same as BadExample2)
    public class LegacyOrderDatabase
    {
        public void OpenConnection(string connectionString, int timeout, bool pooling) 
        {
            Console.WriteLine("Opening legacy DB connection...");
        }
        
        public object ExecuteQuery(string sql, Dictionary<string, object> parameters) 
        {
            Console.WriteLine($"Executing: {sql}");
            return new { OrderId = 123, Status = "PENDING" };
        }
        
        public void CloseConnection() => Console.WriteLine("Closing connection");
        public void BeginTransaction() => Console.WriteLine("BEGIN TRANSACTION");
        public void CommitTransaction() => Console.WriteLine("COMMIT");
        public void RollbackTransaction() => Console.WriteLine("ROLLBACK");
    }

    public class LegacyOrderValidator
    {
        public void InitializeValidator(string configPath) 
        {
            Console.WriteLine("Loading validation rules...");
        }
        
        public List<string> Validate(Dictionary<string, object> orderData) 
        {
            return new List<string>();
        }
    }

    public class LegacyPriceCalculator
    {
        private decimal _taxRate;
        private decimal _shippingRate;

        public void SetTaxRate(decimal rate) => _taxRate = rate;
        public void SetShippingRate(decimal rate) => _shippingRate = rate;
        public void SetDiscountRules(string[] rules) { }
        
        public decimal CalculateSubtotal(List<object> items) => 100m;
        public decimal CalculateTax(decimal subtotal) => subtotal * _taxRate;
        public decimal CalculateShipping(decimal weight) => weight * _shippingRate;
        public decimal ApplyDiscounts(decimal total, string[] codes) => total * 0.9m;
    }

    public class LegacyInventorySystem
    {
        public void Connect(string host, int port, string username, string password) 
        {
            Console.WriteLine($"Connecting to inventory...");
        }
        
        public bool CheckAvailability(int productId, int quantity) => true;
        public void ReserveStock(int productId, int quantity, string orderId) { }
        public void Disconnect() => Console.WriteLine("Disconnecting inventory");
    }

    // ✅ Modern DTOs (ukrywają legacy struktury)
    public class OrderRequest
    {
        public int CustomerId { get; set; }
        public List<OrderItem> Items { get; set; }
        public string[] DiscountCodes { get; set; }
    }

    public class OrderItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Weight { get; set; }
    }

    public class OrderResult
    {
        public bool Success { get; set; }
        public string OrderId { get; set; }
        public decimal Total { get; set; }
        public string ErrorMessage { get; set; }
    }

    // ✅ Facade - modern interface over legacy systems
    public class LegacyOrderSystemFacade
    {
        private readonly LegacyOrderDatabase _database;
        private readonly LegacyOrderValidator _validator;
        private readonly LegacyPriceCalculator _calculator;
        private readonly LegacyInventorySystem _inventory;

        public LegacyOrderSystemFacade()
        {
            _database = new LegacyOrderDatabase();
            _validator = new LegacyOrderValidator();
            _calculator = new LegacyPriceCalculator();
            _inventory = new LegacyInventorySystem();

            // ✅ Inicjalizacja ukryta w konstruktorze
            InitializeLegacySystems();
        }

        private void InitializeLegacySystems()
        {
            // ✅ Complexity ukryta - client nie musi tego znać
            _validator.InitializeValidator("/config/validation-rules.xml");
            _calculator.SetTaxRate(0.08m);
            _calculator.SetShippingRate(2.5m);
            _calculator.SetDiscountRules(new[] { "RULE1", "RULE2" });
            _inventory.Connect("legacy-inventory.local", 5000, "admin", "password123");
        }

        // ✅ Prosty, nowoczesny interface
        public OrderResult CreateOrder(OrderRequest request)
        {
            try
            {
                Console.WriteLine("[Facade] Creating order...");

                // ✅ Validation (complexity ukryta)
                var validationErrors = ValidateOrder(request);
                if (validationErrors.Any())
                {
                    return new OrderResult
                    {
                        Success = false,
                        ErrorMessage = string.Join(", ", validationErrors)
                    };
                }

                // ✅ Check inventory (complexity ukryta)
                if (!CheckInventoryAvailability(request.Items))
                {
                    return new OrderResult
                    {
                        Success = false,
                        ErrorMessage = "Some items are out of stock"
                    };
                }

                // ✅ Calculate price (complexity ukryta)
                var total = CalculateOrderTotal(request);

                // ✅ Save to database (complexity ukryta, transaction managed)
                var orderId = SaveOrderToDatabase(request, total);

                // ✅ Reserve inventory (complexity ukryta)
                ReserveInventory(request.Items, orderId);

                Console.WriteLine($"[Facade] Order {orderId} created successfully. Total: ${total}");

                return new OrderResult
                {
                    Success = true,
                    OrderId = orderId,
                    Total = total
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Facade] Error: {ex.Message}");
                return new OrderResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        // ✅ Private methods - enkapsulacja legacy complexity
        private List<string> ValidateOrder(OrderRequest request)
        {
            var orderData = new Dictionary<string, object>
            {
                { "customerId", request.CustomerId },
                { "items", request.Items }
            };
            return _validator.Validate(orderData);
        }

        private bool CheckInventoryAvailability(List<OrderItem> items)
        {
            foreach (var item in items)
            {
                if (!_inventory.CheckAvailability(item.ProductId, item.Quantity))
                {
                    return false;
                }
            }
            return true;
        }

        private decimal CalculateOrderTotal(OrderRequest request)
        {
            var items = request.Items.Cast<object>().ToList();
            var subtotal = _calculator.CalculateSubtotal(items);
            var tax = _calculator.CalculateTax(subtotal);
            var totalWeight = request.Items.Sum(i => i.Weight * i.Quantity);
            var shipping = _calculator.CalculateShipping(totalWeight);
            var total = subtotal + tax + shipping;
            
            if (request.DiscountCodes != null && request.DiscountCodes.Any())
            {
                total = _calculator.ApplyDiscounts(total, request.DiscountCodes);
            }
            
            return total;
        }

        private string SaveOrderToDatabase(OrderRequest request, decimal total)
        {
            _database.OpenConnection("Server=legacy;Database=Orders;", 30, true);
            _database.BeginTransaction();

            try
            {
                var sql = "INSERT INTO Orders (CustomerId, Total, Status) VALUES (@CustomerId, @Total, 'PENDING')";
                var parameters = new Dictionary<string, object>
                {
                    { "@CustomerId", request.CustomerId },
                    { "@Total", total }
                };
                var result = _database.ExecuteQuery(sql, parameters);
                
                _database.CommitTransaction();
                return "ORDER123"; // Simplified
            }
            catch
            {
                _database.RollbackTransaction();
                throw;
            }
            finally
            {
                _database.CloseConnection();
            }
        }

        private void ReserveInventory(List<OrderItem> items, string orderId)
        {
            foreach (var item in items)
            {
                _inventory.ReserveStock(item.ProductId, item.Quantity, orderId);
            }
        }

        // ✅ Cleanup
        public void Dispose()
        {
            _inventory.Disconnect();
        }
    }

    // ✅ Client - prosty, nowoczesny kod
    public class OrderService
    {
        private readonly LegacyOrderSystemFacade _orderFacade;

        public OrderService()
        {
            // ✅ Jeden facade zamiast wielu legacy systemów
            _orderFacade = new LegacyOrderSystemFacade();
        }

        public void CreateOrder(int customerId, List<OrderItem> items, string[] discountCodes)
        {
            // ✅ Prosty, modern interface - facade ukrywa legacy complexity!
            var request = new OrderRequest
            {
                CustomerId = customerId,
                Items = items,
                DiscountCodes = discountCodes
            };

            var result = _orderFacade.CreateOrder(request);

            if (result.Success)
            {
                Console.WriteLine($"Order created! ID: {result.OrderId}, Total: ${result.Total}");
            }
            else
            {
                Console.WriteLine($"Order failed: {result.ErrorMessage}");
            }

            // ✅ Brak orchestration
            // ✅ Brak inicjalizacji
            // ✅ Brak transaction management
            // ✅ Brak konwersji między legacy i modern API
            // ✅ Wszystko ukryte w facade!
        }
    }

    // ✅ Benefits:
    // - Modern interface over legacy API
    // - Gradual migration (wrap old, add new features)
    // - Testable (mock facade instead of all legacy systems)
    // - Centralized place for legacy complexity
    // - Easy to add caching, logging, error handling
    // - Client code is clean and maintainable
}
