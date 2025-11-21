namespace SOLID.OCP.Good2
{
    public class VipCustomerDiscount : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal price, int yearsAsMember)
        {
            var baseDiscount = price * 0.25m;
            var loyaltyBonus = price * (yearsAsMember * 0.01m);
            return baseDiscount + loyaltyBonus;
        }
    }
}
