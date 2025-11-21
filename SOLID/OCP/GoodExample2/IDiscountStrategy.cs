namespace SOLID.OCP.Good2
{
    // ✅ GOOD: Interface for discount strategies
    public interface IDiscountStrategy
    {
        decimal CalculateDiscount(decimal price, int yearsAsMember);
    }
}
