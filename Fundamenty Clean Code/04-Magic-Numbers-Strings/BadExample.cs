using System;
using System.Collections.Generic;

namespace CleanCodeFundamentals.MagicNumbers.Bad
{
    public class Order
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public decimal Amount { get; set; }
    }

    public class User
    {
        public string Type { get; set; }
        public string Email { get; set; }
    }

    // Problem: Magic numbers i magic strings - co oznacza 4? co to jest "admin"?
    public class OrderProcessor
    {
        public bool CanProcessOrder(Order order, User user)
        {
            // Co oznacza status 4? Trzeba zaglądać do dokumentacji lub bazy danych
            if (order.Status == 4)
            {
                return false; // Status 4 = anulowane?
            }

            // Co oznacza status 2?
            if (order.Status == 2)
            {
                // Co to jest "admin"? Co jeśli ktoś napisze "Admin" z dużej litery?
                if (user.Type == "admin" || user.Type == "manager")
                {
                    return true;
                }
                return false;
            }

            // Co oznacza 1000? Minimalny próg? Maksymalna kwota?
            if (order.Amount > 1000)
            {
                // Co oznacza "vip"?
                if (user.Type == "vip" || user.Type == "admin")
                {
                    return true;
                }
                return false;
            }

            return true;
        }

        public string GetOrderStatusDescription(int status)
        {
            // Kolejne magic numbers
            if (status == 1) return "Pending";
            if (status == 2) return "Processing";
            if (status == 3) return "Completed";
            if (status == 4) return "Cancelled";
            return "Unknown";
        }

        public decimal CalculateDiscount(Order order, User user)
        {
            // Co oznacza 0.1, 0.15, 0.2?
            if (user.Type == "vip")
            {
                return order.Amount * 0.2;
            }
            
            if (user.Type == "premium")
            {
                return order.Amount * 0.15;
            }

            if (order.Amount > 500)
            {
                return order.Amount * 0.1;
            }

            return 0;
        }
    }
}
