using System;
using System.Collections.Generic;

namespace DesignPatterns.Observer
{
    // ❌ BAD: Tight coupling bez Observer pattern

    // BŁĄD 1: Bezpośrednie wywołania - tight coupling
    public class BadStockMarket
    {
        private decimal _price;
        private readonly BadInvestor _investor1;
        private readonly BadInvestor _investor2;
        private readonly BadLogger _logger;

        public BadStockMarket(BadInvestor inv1, BadInvestor inv2, BadLogger logger)
        {
            _investor1 = inv1;
            _investor2 = inv2;
            _logger = logger;
        }

        public void UpdatePrice(decimal newPrice)
        {
            _price = newPrice;
            
            // ❌ Musi znać wszystkie zainteresowane obiekty
            _investor1.NotifyPriceChange(_price);
            _investor2.NotifyPriceChange(_price);
            _logger.LogPriceChange(_price);
            
            // ❌ Dodanie nowego obserwera wymaga zmiany tej klasy (naruszenie OCP)
        }
    }

    public class BadInvestor
    {
        public string Name { get; set; }

        public void NotifyPriceChange(decimal price)
        {
            Console.WriteLine($"{Name} notified: Price is {price}");
        }
    }

    public class BadLogger
    {
        public void LogPriceChange(decimal price)
        {
            Console.WriteLine($"Log: Price changed to {price}");
        }
    }

    // BŁĄD 2: Brak możliwości dynamicznego dodawania/usuwania obserwatorów
    public class BadWeatherStation
    {
        private double _temperature;
        
        // ❌ Fixed list - nie można dodać nowych konsumentów w runtime
        public void UpdateTemperature(double temp)
        {
            _temperature = temp;
            // Hardcoded notifications
            Console.WriteLine($"Display: {_temperature}°C");
            Console.WriteLine($"Alert: Temperature is {_temperature}°C");
        }
    }
}
