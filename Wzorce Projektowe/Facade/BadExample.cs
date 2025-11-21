using System;
using System.Collections.Generic;

namespace DesignPatterns.Facade
{
    // ❌ BAD: Client musi znać złożone API dostawcy płatności

    // ❌ Complex payment provider API (like Stripe, PayPal)
    public class PaymentGatewayApi
    {
        public string CreateSession(Dictionary<string, object> config) 
        {
            Console.WriteLine("Creating payment session with config...");
            return "session_123";
        }
        
        public void SetWebhook(string url, string[] events) 
        {
            Console.WriteLine($"Setting webhook: {url}");
        }
        
        public void ConfigureSecurity(string apiKey, string secretKey) 
        {
            Console.WriteLine("Configuring security...");
        }
    }

    public class PaymentProcessor
    {
        public string ProcessPayment(string sessionId, string customerId, decimal amount) 
        {
            Console.WriteLine($"Processing payment: ${amount}");
            return "charge_456";
        }
        
        public void ValidateCard(string cardNumber, string cvv, string expiry) 
        {
            Console.WriteLine("Validating card...");
        }
        
        public void Calculate3DSecure(string cardNumber) 
        {
            Console.WriteLine("Calculating 3D Secure...");
        }
    }

    public class RefundService
    {
        public void InitiateRefund(string chargeId, decimal amount, string reason) 
        {
            Console.WriteLine($"Initiating refund for ${amount}");
        }
        
        public void ValidateRefundEligibility(string chargeId) 
        {
            Console.WriteLine("Validating refund eligibility...");
        }
    }

    public class FraudDetectionService
    {
        public void AnalyzeTransaction(string customerId, decimal amount, string ipAddress) 
        {
            Console.WriteLine("Analyzing transaction for fraud...");
        }
        
        public bool CheckRiskScore(string customerId) 
        {
            return true;
        }
    }

    public class NotificationService
    {
        public void SendPaymentConfirmation(string customerId, string chargeId) 
        {
            Console.WriteLine($"Sending confirmation to {customerId}");
        }
        
        public void SendRefundNotification(string customerId) 
        {
            Console.WriteLine($"Sending refund notification to {customerId}");
        }
    }

    // ❌ Client musi zarządzać wszystkimi serwisami i znać szczegóły API
    public class CheckoutController
    {
        public void ProcessCheckout(string customerId, decimal amount, string cardNumber, string cvv, string expiry)
        {
            // ❌ Złożona orkiestracja wielu serwisów
            var gateway = new PaymentGatewayApi();
            var processor = new PaymentProcessor();
            var fraud = new FraudDetectionService();
            var notifications = new NotificationService();

            // ❌ Client musi znać szczegóły konfiguracji
            gateway.ConfigureSecurity("pk_test_123", "sk_test_456");
            gateway.SetWebhook("https://mysite.com/webhook", new[] { "payment.success", "payment.failed" });

            var config = new Dictionary<string, object>
            {
                { "mode", "payment" },
                { "currency", "usd" },
                { "success_url", "https://mysite.com/success" },
                { "cancel_url", "https://mysite.com/cancel" }
            };
            var sessionId = gateway.CreateSession(config);

            // ❌ Złożona walidacja
            processor.ValidateCard(cardNumber, cvv, expiry);
            processor.Calculate3DSecure(cardNumber);

            // ❌ Fraud check
            fraud.AnalyzeTransaction(customerId, amount, "192.168.1.1");
            if (!fraud.CheckRiskScore(customerId))
            {
                throw new Exception("High risk transaction");
            }

            // ❌ Process payment
            var chargeId = processor.ProcessPayment(sessionId, customerId, amount);

            // ❌ Send notification
            notifications.SendPaymentConfirmation(customerId, chargeId);

            // ❌ PROBLEMY:
            // - Client musi znać API wszystkich serwisów
            // - Złożona inicjalizacja i konfiguracja
            // - Duplikacja kodu w każdym miejscu użycia
            // - Trudne testowanie (wiele zależności)
            // - Zmiany w API propagują się do wszystkich klientów
        }

        public void ProcessRefund(string chargeId, decimal amount, string customerId)
        {
            var refundService = new RefundService();
            var notifications = new NotificationService();

            // ❌ Duplikacja kodu walidacji
            refundService.ValidateRefundEligibility(chargeId);
            refundService.InitiateRefund(chargeId, amount, "Customer request");
            notifications.SendRefundNotification(customerId);
        }
    }
}
