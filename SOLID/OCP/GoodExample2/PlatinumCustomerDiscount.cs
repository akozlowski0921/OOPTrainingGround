namespace SOLID.OCP.Good2
{
    public class PlatinumCustomerDiscount : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal price, int yearsAsMember)
        {
            return price * 0.20m;
        }
    }
}
