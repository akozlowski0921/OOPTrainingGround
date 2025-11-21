namespace SOLID.OCP.Good2
{
    public class RegularCustomerDiscount : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal price, int yearsAsMember)
        {
            return price * 0.05m;
        }
    }
}
