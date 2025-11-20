using System;
using System.Collections.Generic;

namespace CleanCodeFundamentals.DRY.Good
{
    public class Order
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public bool IsShipped { get; set; }
        public DateTime OrderDate { get; set; }
        public int DaysSinceOrder => (DateTime.Now - OrderDate).Days;
    }

    // Rozwiązanie: Logika biznesowa wydzielona do wspólnej klasy
    public class OrderStatusCalculator
    {
        public string CalculateStatus(Order order)
        {
            if (!order.IsPaid)
            {
                return "Oczekuje na płatność";
            }
            
            if (order.IsPaid && !order.IsShipped)
            {
                return "Opłacone, oczekuje na wysyłkę";
            }
            
            if (order.IsShipped && order.DaysSinceOrder <= 30)
            {
                return "Wysłane";
            }
            
            return "Zrealizowane";
        }
    }

    public class OrderReportService
    {
        private readonly OrderStatusCalculator _statusCalculator;

        public OrderReportService(OrderStatusCalculator statusCalculator)
        {
            _statusCalculator = statusCalculator;
        }

        public string GenerateOrderReport(Order order)
        {
            var status = _statusCalculator.CalculateStatus(order);
            return $"Raport zamówienia #{order.Id}: {status}";
        }
    }

    public class OrderEmailService
    {
        private readonly OrderStatusCalculator _statusCalculator;

        public OrderEmailService(OrderStatusCalculator statusCalculator)
        {
            _statusCalculator = statusCalculator;
        }

        public void SendStatusEmail(Order order, string customerEmail)
        {
            var status = _statusCalculator.CalculateStatus(order);
            Console.WriteLine($"Wysyłanie emaila do {customerEmail}: Twoje zamówienie ma status: {status}");
        }
    }

    public class OrderDashboardController
    {
        private readonly OrderStatusCalculator _statusCalculator;

        public OrderDashboardController(OrderStatusCalculator statusCalculator)
        {
            _statusCalculator = statusCalculator;
        }

        public string GetOrderStatusForDisplay(Order order)
        {
            var status = _statusCalculator.CalculateStatus(order);
            return $"<div class='order-status'>{status}</div>";
        }
    }
}
