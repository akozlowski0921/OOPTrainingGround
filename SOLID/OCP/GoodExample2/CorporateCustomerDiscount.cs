namespace SOLID.OCP.Good2
{
    public class CorporateCustomerDiscount : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal price, int yearsAsMember)
        {
            if (price > 10000)
            {
                return price * 0.30m;
            }
            return price * 0.18m;
        }
    }
}
