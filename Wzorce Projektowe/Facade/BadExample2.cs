using System;
using System.Collections.Generic;

namespace DesignPatterns.Facade.Bad2
{
    // ❌ BAD: No facade for legacy system - client must know old API

    // ❌ Legacy Order System (stare, złożone API z lat 90)
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
        
        public void CloseConnection() 
        {
            Console.WriteLine("Closing legacy DB connection");
        }
        
        public void BeginTransaction() 
        {
            Console.WriteLine("BEGIN TRANSACTION");
        }
        
        public void CommitTransaction() 
        {
            Console.WriteLine("COMMIT");
        }
        
        public void RollbackTransaction() 
        {
            Console.WriteLine("ROLLBACK");
        }
    }

    public class LegacyOrderValidator
    {
        public void InitializeValidator(string configPath) 
        {
            Console.WriteLine("Loading validation rules from config...");
        }
        
        public List<string> Validate(Dictionary<string, object> orderData) 
        {
            Console.WriteLine("Validating order data...");
            return new List<string>(); // No errors
        }
    }

    public class LegacyPriceCalculator
    {
        private decimal _taxRate;
        private decimal _shippingRate;

        public void SetTaxRate(decimal rate) => _taxRate = rate;
        public void SetShippingRate(decimal rate) => _shippingRate = rate;
        public void SetDiscountRules(string[] rules) { }
        
        public decimal CalculateSubtotal(List<object> items) 
        {
            Console.WriteLine("Calculating subtotal...");
            return 100m;
        }
        
        public decimal CalculateTax(decimal subtotal) 
        {
            Console.WriteLine("Calculating tax...");
            return subtotal * _taxRate;
        }
        
        public decimal CalculateShipping(decimal weight) 
        {
            Console.WriteLine("Calculating shipping...");
            return weight * _shippingRate;
        }
        
        public decimal ApplyDiscounts(decimal total, string[] discountCodes) 
        {
            Console.WriteLine("Applying discounts...");
            return total * 0.9m;
        }
    }

    public class LegacyInventorySystem
    {
        public void Connect(string host, int port, string username, string password) 
        {
            Console.WriteLine($"Connecting to inventory system at {host}:{port}");
        }
        
        public bool CheckAvailability(int productId, int quantity) 
        {
            Console.WriteLine($"Checking stock for product {productId}");
            return true;
        }
        
        public void ReserveStock(int productId, int quantity, string orderId) 
        {
            Console.WriteLine($"Reserving {quantity} units of product {productId}");
        }
        
        public void Disconnect() 
        {
            Console.WriteLine("Disconnecting from inventory system");
        }
    }

    // ❌ Client musi znać wszystkie szczegóły legacy API
    public class OrderService
    {
        public void CreateOrder(int customerId, List<object> items, string[] discountCodes)
        {
            // ❌ Złożona inicjalizacja wielu systemów
            var database = new LegacyOrderDatabase();
            var validator = new LegacyOrderValidator();
            var calculator = new LegacyPriceCalculator();
            var inventory = new LegacyInventorySystem();

            try
            {
                // ❌ Szczegółowa konfiguracja każdego systemu
                database.OpenConnection("Server=legacy;Database=Orders;", 30, true);
                database.BeginTransaction();

                validator.InitializeValidator("/config/validation-rules.xml");

                calculator.SetTaxRate(0.08m);
                calculator.SetShippingRate(2.5m);
                calculator.SetDiscountRules(new[] { "RULE1", "RULE2" });

                inventory.Connect("legacy-inventory.local", 5000, "admin", "password123");

                // ❌ Walidacja z legacy API
                var orderData = new Dictionary<string, object>
                {
                    { "customerId", customerId },
                    { "items", items }
                };
                var errors = validator.Validate(orderData);
                if (errors.Count > 0)
                {
                    throw new Exception("Validation failed");
                }

                // ❌ Sprawdzenie inventory z legacy API
                foreach (var item in items)
                {
                    if (!inventory.CheckAvailability(1, 1))
                    {
                        throw new Exception("Out of stock");
                    }
                }

                // ❌ Kalkulacja ceny z legacy API
                var subtotal = calculator.CalculateSubtotal(items);
                var tax = calculator.CalculateTax(subtotal);
                var shipping = calculator.CalculateShipping(5.0m);
                var total = subtotal + tax + shipping;
                total = calculator.ApplyDiscounts(total, discountCodes);

                // ❌ Zapis do legacy database
                var sql = "INSERT INTO Orders (CustomerId, Total, Status) VALUES (@CustomerId, @Total, 'PENDING')";
                var parameters = new Dictionary<string, object>
                {
                    { "@CustomerId", customerId },
                    { "@Total", total }
                };
                database.ExecuteQuery(sql, parameters);

                // ❌ Rezerwacja inventory
                foreach (var item in items)
                {
                    inventory.ReserveStock(1, 1, "ORDER123");
                }

                database.CommitTransaction();
                Console.WriteLine($"Order created with total: ${total}");
            }
            catch (Exception ex)
            {
                database.RollbackTransaction();
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                database.CloseConnection();
                inventory.Disconnect();
            }

            // ❌ PROBLEMY:
            // - Client musi znać wszystkie szczegóły legacy API
            // - Złożona inicjalizacja i konfiguracja
            // - Ręczne zarządzanie transakcjami
            // - Trudne testowanie (wiele zależności)
            // - Duplikacja kodu w każdym miejscu użycia
            // - Zmiany w legacy API wymagają zmian w wielu miejscach
        }
    }
}
