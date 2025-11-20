using System;
using System.Collections.Generic;

namespace CleanCodeFundamentals.DRY.Bad
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

    // Problem: Ta sama logika biznesowa obliczająca status zamówienia jest powielona w 3 miejscach
    public class OrderReportService
    {
        public string GenerateOrderReport(Order order)
        {
            string status;

            // Kopia #1 logiki obliczającej status
            if (!order.IsPaid)
            {
                status = "Oczekuje na płatność";
            }
            else if (order.IsPaid && !order.IsShipped)
            {
                status = "Opłacone, oczekuje na wysyłkę";
            }
            else if (order.IsShipped && order.DaysSinceOrder <= 30)
            {
                status = "Wysłane";
            }
            else
            {
                status = "Zrealizowane";
            }

            return $"Raport zamówienia #{order.Id}: {status}";
        }
    }

    public class OrderEmailService
    {
        public void SendStatusEmail(Order order, string customerEmail)
        {
            string status;

            // Kopia #2 tej samej logiki - ryzyko desynchronizacji!
            if (!order.IsPaid)
            {
                status = "Oczekuje na płatność";
            }
            else if (order.IsPaid && !order.IsShipped)
            {
                status = "Opłacone, oczekuje na wysyłkę";
            }
            else if (order.IsShipped && order.DaysSinceOrder <= 30)
            {
                status = "Wysłane";
            }
            else
            {
                status = "Zrealizowane";
            }

            Console.WriteLine($"Wysyłanie emaila do {customerEmail}: Twoje zamówienie ma status: {status}");
        }
    }

    public class OrderDashboardController
    {
        public string GetOrderStatusForDisplay(Order order)
        {
            string status;

            // Kopia #3 tej samej logiki - jeszcze większe ryzyko błędów!
            if (!order.IsPaid)
            {
                status = "Oczekuje na płatność";
            }
            else if (order.IsPaid && !order.IsShipped)
            {
                status = "Opłacone, oczekuje na wysyłkę";
            }
            else if (order.IsShipped && order.DaysSinceOrder <= 30)
            {
                status = "Wysłane";
            }
            else
            {
                status = "Zrealizowane";
            }

            return $"<div class='order-status'>{status}</div>";
        }
    }
}
