using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesignPatterns.Facade.Good
{
    // ✅ GOOD: Facade Pattern - upraszcza złożone API płatności

    // ✅ Subsystems (same complex APIs as BadExample)
    public class PaymentGatewayApi
    {
        public string CreateSession(Dictionary<string, object> config) 
        {
            Console.WriteLine("Creating payment session...");
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

    // ✅ Payment Result DTO
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string ChargeId { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class RefundResult
    {
        public bool Success { get; set; }
        public string RefundId { get; set; }
        public string ErrorMessage { get; set; }
    }

    // ✅ Facade - prosty interface nad złożonym API
    public class PaymentFacade
    {
        private readonly PaymentGatewayApi _gateway;
        private readonly PaymentProcessor _processor;
        private readonly RefundService _refundService;
        private readonly FraudDetectionService _fraudDetection;
        private readonly NotificationService _notifications;

        public PaymentFacade()
        {
            _gateway = new PaymentGatewayApi();
            _processor = new PaymentProcessor();
            _refundService = new RefundService();
            _fraudDetection = new FraudDetectionService();
            _notifications = new NotificationService();

            // ✅ Inicjalizacja w konstruktorze - ukryta przed klientem
            InitializeGateway();
        }

        private void InitializeGateway()
        {
            // ✅ Complexity ukryta w facade
            _gateway.ConfigureSecurity("pk_test_123", "sk_test_456");
            _gateway.SetWebhook(
                "https://mysite.com/webhook", 
                new[] { "payment.success", "payment.failed", "refund.created" });
        }

        // ✅ Prosty interface dla płatności
        public PaymentResult ProcessPayment(
            string customerId, 
            decimal amount, 
            string cardNumber, 
            string cvv, 
            string expiry,
            string ipAddress = "127.0.0.1")
        {
            try
            {
                Console.WriteLine($"[PaymentFacade] Processing payment for {customerId}");

                // ✅ Walidacja karty (complexity ukryta)
                _processor.ValidateCard(cardNumber, cvv, expiry);
                _processor.Calculate3DSecure(cardNumber);

                // ✅ Fraud detection (complexity ukryta)
                _fraudDetection.AnalyzeTransaction(customerId, amount, ipAddress);
                if (!_fraudDetection.CheckRiskScore(customerId))
                {
                    return new PaymentResult
                    {
                        Success = false,
                        ErrorMessage = "Transaction flagged as high risk"
                    };
                }

                // ✅ Create session and process (complexity ukryta)
                var config = CreatePaymentConfig(amount);
                var sessionId = _gateway.CreateSession(config);
                var chargeId = _processor.ProcessPayment(sessionId, customerId, amount);

                // ✅ Notifications (complexity ukryta)
                _notifications.SendPaymentConfirmation(customerId, chargeId);

                return new PaymentResult
                {
                    Success = true,
                    ChargeId = chargeId
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PaymentFacade] Error: {ex.Message}");
                return new PaymentResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        // ✅ Prosty interface dla refund
        public RefundResult ProcessRefund(string chargeId, decimal amount, string customerId, string reason = "Customer request")
        {
            try
            {
                Console.WriteLine($"[PaymentFacade] Processing refund for charge {chargeId}");

                // ✅ Walidacja (complexity ukryta)
                _refundService.ValidateRefundEligibility(chargeId);

                // ✅ Initiate refund
                _refundService.InitiateRefund(chargeId, amount, reason);

                // ✅ Notify customer
                _notifications.SendRefundNotification(customerId);

                return new RefundResult
                {
                    Success = true,
                    RefundId = $"refund_{Guid.NewGuid()}"
                };
            }
            catch (Exception ex)
            {
                return new RefundResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        // ✅ Helper method - complexity ukryta
        private Dictionary<string, object> CreatePaymentConfig(decimal amount)
        {
            return new Dictionary<string, object>
            {
                { "mode", "payment" },
                { "currency", "usd" },
                { "amount", amount },
                { "success_url", "https://mysite.com/success" },
                { "cancel_url", "https://mysite.com/cancel" }
            };
        }
    }

    // ✅ Client - prosty kod
    public class CheckoutController
    {
        private readonly PaymentFacade _paymentFacade;

        public CheckoutController()
        {
            // ✅ Jeden obiekt facade zamiast wielu serwisów
            _paymentFacade = new PaymentFacade();
        }

        public void ProcessCheckout(string customerId, decimal amount, string cardNumber, string cvv, string expiry)
        {
            // ✅ Prosty interface - facade ukrywa wszystkie szczegóły!
            var result = _paymentFacade.ProcessPayment(
                customerId, 
                amount, 
                cardNumber, 
                cvv, 
                expiry);

            if (result.Success)
            {
                Console.WriteLine($"Payment successful! Charge ID: {result.ChargeId}");
            }
            else
            {
                Console.WriteLine($"Payment failed: {result.ErrorMessage}");
            }

            // ✅ Brak orkiestracji wielu serwisów
            // ✅ Brak konfiguracji
            // ✅ Brak walidacji
            // ✅ Wszystko ukryte w facade!
        }

        public void ProcessRefund(string chargeId, decimal amount, string customerId)
        {
            // ✅ Równie prosty interface dla refund
            var result = _paymentFacade.ProcessRefund(chargeId, amount, customerId);

            if (result.Success)
            {
                Console.WriteLine($"Refund successful! Refund ID: {result.RefundId}");
            }
            else
            {
                Console.WriteLine($"Refund failed: {result.ErrorMessage}");
            }
        }
    }

    // ✅ Benefits:
    // - Prosty interface dla skomplikowanego API
    // - Inicjalizacja i konfiguracja ukryta
    // - Orchestration ukryta w facade
    // - Łatwe testowanie (mock facade)
    // - Zmiany w API nie propagują się do klientów
    // - Single Responsibility (facade orchestrates, client uses)
    // - Error handling centralized
}
