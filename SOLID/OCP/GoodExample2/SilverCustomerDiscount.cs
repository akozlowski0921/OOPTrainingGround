namespace SOLID.OCP.Good2
{
    public class SilverCustomerDiscount : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal price, int yearsAsMember)
        {
            return price * 0.10m;
        }
    }
}
