namespace SOLID.OCP.Good2
{
    // ✅ GOOD: Calculator uses strategy pattern - closed for modification, open for extension
    public class DiscountCalculator
    {
        private readonly IDiscountStrategy _strategy;

        public DiscountCalculator(IDiscountStrategy strategy)
        {
            _strategy = strategy;
        }

        public decimal CalculateDiscount(decimal price, int yearsAsMember)
        {
            return _strategy.CalculateDiscount(price, yearsAsMember);
        }
    }
}
