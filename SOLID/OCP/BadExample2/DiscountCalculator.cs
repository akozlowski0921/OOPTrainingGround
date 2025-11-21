using System;

namespace SOLID.OCP.Bad2
{
    // ❌ BAD: Adding new discount type requires modifying this class
    public class DiscountCalculator
    {
        public decimal CalculateDiscount(decimal price, string customerType, int yearsAsMember)
        {
            if (customerType == "Regular")
            {
                return price * 0.05m;
            }
            else if (customerType == "Silver")
            {
                return price * 0.10m;
            }
            else if (customerType == "Gold")
            {
                return price * 0.15m;
            }
            else if (customerType == "Platinum")
            {
                return price * 0.20m;
            }
            else if (customerType == "VIP")
            {
                // VIP gets extra discount based on membership years
                var baseDiscount = price * 0.25m;
                var loyaltyBonus = price * (yearsAsMember * 0.01m);
                return baseDiscount + loyaltyBonus;
            }
            else if (customerType == "Corporate")
            {
                // Different calculation for corporate
                if (price > 10000)
                {
                    return price * 0.30m;
                }
                return price * 0.18m;
            }
            
            return 0;
        }
    }
}
