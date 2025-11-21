using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace SOLID.SRP.Bad2
{
    // ❌ BAD: Klasa robi wszystko - obsługa zamówień, walidacja, email, logowanie, raportowanie
    public class OrderService
    {
        private List<Order> _orders = new List<Order>();
        private string _logFilePath = "orders.log";

        public bool ProcessOrder(Order order)
        {
            // Walidacja
            if (order.Items == null || order.Items.Count == 0)
            {
                Log("Invalid order: no items");
                return false;
            }

            if (string.IsNullOrEmpty(order.CustomerEmail))
            {
                Log("Invalid order: no email");
                return false;
            }

            // Obliczanie ceny
            decimal total = 0;
            foreach (var item in order.Items)
            {
                if (item.Quantity > 100)
                {
                    total += item.Price * item.Quantity * 0.9m; // Rabat hurtowy
                }
                else
                {
                    total += item.Price * item.Quantity;
                }
            }
            order.TotalAmount = total;

            // Zapis do bazy
            _orders.Add(order);

            // Wysyłanie emaila
            try
            {
                var smtp = new SmtpClient("smtp.example.com");
                var mail = new MailMessage();
                mail.To.Add(order.CustomerEmail);
                mail.Subject = "Order Confirmation";
                mail.Body = $"Your order total is: {total:C}";
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Log($"Email error: {ex.Message}");
            }

            // Logowanie
            Log($"Order processed: {order.Id}, Total: {total:C}");

            return true;
        }

        public string GenerateMonthlyReport(int month, int year)
        {
            // Generowanie raportu
            var monthOrders = _orders.FindAll(o => 
                o.OrderDate.Month == month && o.OrderDate.Year == year);

            decimal totalRevenue = 0;
            foreach (var order in monthOrders)
            {
                totalRevenue += order.TotalAmount;
            }

            string report = $"Monthly Report {month}/{year}\n";
            report += $"Total Orders: {monthOrders.Count}\n";
            report += $"Total Revenue: {totalRevenue:C}\n";

            // Zapis raportu do pliku
            System.IO.File.WriteAllText($"report_{month}_{year}.txt", report);

            // Logowanie
            Log($"Report generated for {month}/{year}");

            return report;
        }

        private void Log(string message)
        {
            var logEntry = $"{DateTime.Now}: {message}\n";
            System.IO.File.AppendAllText(_logFilePath, logEntry);
        }
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
