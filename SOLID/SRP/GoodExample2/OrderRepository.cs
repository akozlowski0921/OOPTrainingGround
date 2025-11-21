namespace SOLID.SRP.Good2
{
    // ✅ GOOD: Odpowiedzialność - tylko dostęp do danych
    public class OrderRepository
    {
        private List<Order> _orders = new List<Order>();

        public void Save(Order order)
        {
            _orders.Add(order);
        }

        public List<Order> GetByMonthAndYear(int month, int year)
        {
            return _orders
                .Where(o => o.OrderDate.Month == month && o.OrderDate.Year == year)
                .ToList();
        }
    }
}
