namespace SOLID.OCP.Good2
{
    public class GoldCustomerDiscount : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal price, int yearsAsMember)
        {
            return price * 0.15m;
        }
    }
}
