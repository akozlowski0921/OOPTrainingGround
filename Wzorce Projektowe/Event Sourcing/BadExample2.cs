using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignPatterns.EventSourcing.Bad2
{
    // ❌ BAD: Cannot reconstruct state from history - data loss scenarios

    public class ShoppingCart
    {
        public int CartId { get; set; }
        public List<CartItem> Items { get; set; } = new();
        public decimal Total { get; set; }
        public DateTime LastModified { get; set; }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    // ❌ Traditional repository - only current state
    public class ShoppingCartService
    {
        private readonly List<ShoppingCart> _carts = new();

        public void AddItem(int cartId, int productId, string productName, int quantity, decimal price)
        {
            var cart = _carts.FirstOrDefault(c => c.CartId == cartId);
            if (cart == null)
            {
                cart = new ShoppingCart { CartId = cartId };
                _carts.Add(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                // ❌ Nie wiemy że klient dodał produkt DRUGI RAZ
                // ❌ Utraciliśmy timestamp tej operacji
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = productName,
                    Quantity = quantity,
                    Price = price
                });
            }

            cart.Total = cart.Items.Sum(i => i.Price * i.Quantity);
            cart.LastModified = DateTime.UtcNow;
            // ❌ Straciliśmy sekwencję działań użytkownika
        }

        public void RemoveItem(int cartId, int productId)
        {
            var cart = _carts.FirstOrDefault(c => c.CartId == cartId);
            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    cart.Items.Remove(item);
                    cart.Total = cart.Items.Sum(i => i.Price * i.Quantity);
                    cart.LastModified = DateTime.UtcNow;
                    // ❌ Nie możemy przywrócić usuniętego produktu
                    // ❌ Nie wiemy DLACZEGO został usunięty
                }
            }
        }

        public void UpdateQuantity(int cartId, int productId, int newQuantity)
        {
            var cart = _carts.FirstOrDefault(c => c.CartId == cartId);
            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    item.Quantity = newQuantity;
                    cart.Total = cart.Items.Sum(i => i.Price * i.Quantity);
                    // ❌ Straciliśmy poprzednią ilość
                    // ❌ Nie możemy zobaczyć historii zmian quantity
                }
            }
        }

        public ShoppingCart GetCart(int cartId)
        {
            return _carts.FirstOrDefault(c => c.CartId == cartId);
            // ❌ Tylko current state
        }

        // ❌ Nie możemy odpowiedzieć na pytania:
        // - Jak często użytkownik zmienił quantity?
        // - Które produkty dodał i potem usunął?
        // - Jaki był stan koszyka 5 minut temu?
        // - Czy użytkownik wahał się przy zakupie?
    }

    // ❌ PROBLEMY:
    // - Brak możliwości odtworzenia historii działań użytkownika
    // - Analytics: nie wiemy jak użytkownik interagował z koszykiem
    // - Debugging: trudno zrozumieć jak doszło do błędu
    // - Business insights: które produkty są dodawane ale nie kupowane?
    // - Temporal queries: nie możemy zobaczyć stanu w przeszłości
    // - Compliance: brak audit trail dla regulowanych branż
}
