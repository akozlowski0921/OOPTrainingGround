using System;
using System.Collections.Generic;

namespace SpecyfikaDotNet.DependencyInjection.Bad2
{
    // ❌ BAD: Singleton service z mutable state - race conditions
    public class BadShoppingCartService
    {
        private List<CartItem> _items = new List<CartItem>();
        private decimal _totalPrice = 0;

        public void AddItem(string productName, decimal price, int quantity)
        {
            // PROBLEM: Wielu użytkowników współdzieli ten sam koszyk!
            _items.Add(new CartItem 
            { 
                ProductName = productName, 
                Price = price, 
                Quantity = quantity 
            });
            _totalPrice += price * quantity;
        }

        public decimal GetTotalPrice()
        {
            return _totalPrice;
        }

        public List<CartItem> GetItems()
        {
            return _items;
        }

        public void Clear()
        {
            _items.Clear();
            _totalPrice = 0;
        }
    }

    // ❌ BAD: Singleton z user-specific data
    public class BadUserContextService
    {
        private string _currentUserId;
        private string _currentUserRole;

        public void SetCurrentUser(string userId, string role)
        {
            // PROBLEM: W środowisku wielowątkowym różni użytkownicy nadpisują dane
            _currentUserId = userId;
            _currentUserRole = role;
        }

        public string GetCurrentUserId()
        {
            return _currentUserId;
        }

        public bool IsAdmin()
        {
            return _currentUserRole == "Admin";
        }
    }

    public class CartItem
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
