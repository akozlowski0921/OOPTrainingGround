namespace SOLID.SRP.Good2
{
    // ✅ GOOD: Odpowiedzialność - tylko obliczanie cen
    public class PriceCalculator
    {
        private const int BulkDiscountThreshold = 100;
        private const decimal BulkDiscountRate = 0.9m;

        public decimal CalculateTotal(List<OrderItem> items)
        {
            decimal total = 0;

            foreach (var item in items)
            {
                var itemTotal = item.Price * item.Quantity;

                if (item.Quantity > BulkDiscountThreshold)
                {
                    itemTotal *= BulkDiscountRate;
                }

                total += itemTotal;
            }

            return total;
        }
    }
}
