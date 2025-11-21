using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace SpecyfikaDotNet.DependencyInjection.Good2
{
    // ✅ GOOD: Scoped service dla user-specific data
    public class GoodShoppingCartService
    {
        private readonly List<CartItem> _items = new List<CartItem>();
        private decimal _totalPrice = 0;

        // Każde HTTP request ma własną instancję - brak race conditions
        public void AddItem(string productName, decimal price, int quantity)
        {
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
            return new List<CartItem>(_items); // Return copy
        }

        public void Clear()
        {
            _items.Clear();
            _totalPrice = 0;
        }
    }

    // ✅ GOOD: Scoped service z user context
    public class GoodUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GoodUserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUserId()
        {
            // Pobiera z kontekstu HTTP, który jest unique per request
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        }

        public bool IsAdmin()
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ?? false;
        }
    }

    // ✅ GOOD: Singleton tylko dla stateless services
    public class ConfigurationService
    {
        private readonly Dictionary<string, string> _settings;

        public ConfigurationService()
        {
            // Immutable configuration loaded once
            _settings = new Dictionary<string, string>
            {
                { "ApiUrl", "https://api.example.com" },
                { "Timeout", "30" }
            };
        }

        public string GetSetting(string key)
        {
            return _settings.TryGetValue(key, out var value) ? value : null;
        }
    }

    public class CartItem
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    // Konfiguracja DI
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Scoped - per HTTP request
            services.AddScoped<GoodShoppingCartService>();
            services.AddScoped<GoodUserContextService>();

            // Singleton - shared, stateless
            services.AddSingleton<ConfigurationService>();
        }
    }

    // Dummy interfaces for compilation
    public interface IServiceCollection
    {
        void AddScoped<T>() where T : class;
        void AddSingleton<T>() where T : class;
    }

    public static class ServiceCollectionExtensions
    {
        public static void AddScoped<T>(this IServiceCollection services) where T : class { }
        public static void AddSingleton<T>(this IServiceCollection services) where T : class { }
    }
}
