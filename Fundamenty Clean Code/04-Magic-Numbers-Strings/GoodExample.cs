using System;
using System.Collections.Generic;

namespace CleanCodeFundamentals.MagicNumbers.Good
{
    // Rozwiązanie: Enum dla statusów zamówienia
    public enum OrderStatus
    {
        Pending = 1,
        Processing = 2,
        Completed = 3,
        Cancelled = 4
    }

    // Rozwiązanie: Enum dla typów użytkowników
    public enum UserType
    {
        Regular,
        Premium,
        Vip,
        Manager,
        Admin
    }

    // Rozwiązanie: Stałe dla wartości biznesowych
    public static class BusinessConstants
    {
        public const decimal HighValueOrderThreshold = 1000m;
        public const decimal StandardDiscountThreshold = 500m;
        
        public const decimal VipDiscountRate = 0.2m;      // 20%
        public const decimal PremiumDiscountRate = 0.15m; // 15%
        public const decimal StandardDiscountRate = 0.1m; // 10%
    }

    public class Order
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Amount { get; set; }
    }

    public class User
    {
        public UserType Type { get; set; }
        public string Email { get; set; }
    }

    public class OrderProcessor
    {
        public bool CanProcessOrder(Order order, User user)
        {
            if (order.Status == OrderStatus.Cancelled)
            {
                return false;
            }

            if (order.Status == OrderStatus.Processing)
            {
                if (user.Type == UserType.Admin || user.Type == UserType.Manager)
                {
                    return true;
                }
                return false;
            }

            if (order.Amount > BusinessConstants.HighValueOrderThreshold)
            {
                if (user.Type == UserType.Vip || user.Type == UserType.Admin)
                {
                    return true;
                }
                return false;
            }

            return true;
        }

        public string GetOrderStatusDescription(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Pending",
                OrderStatus.Processing => "Processing",
                OrderStatus.Completed => "Completed",
                OrderStatus.Cancelled => "Cancelled",
                _ => "Unknown"
            };
        }

        public decimal CalculateDiscount(Order order, User user)
        {
            if (user.Type == UserType.Vip)
            {
                return order.Amount * BusinessConstants.VipDiscountRate;
            }
            
            if (user.Type == UserType.Premium)
            {
                return order.Amount * BusinessConstants.PremiumDiscountRate;
            }

            if (order.Amount > BusinessConstants.StandardDiscountThreshold)
            {
                return order.Amount * BusinessConstants.StandardDiscountRate;
            }

            return 0;
        }
    }
}
