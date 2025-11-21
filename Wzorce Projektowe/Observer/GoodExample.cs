using System;
using System.Collections.Generic;

namespace DesignPatterns.Observer
{
    // ✅ GOOD: Observer Pattern z loose coupling

    // ✅ Observer interface
    public interface IObserver<T>
    {
        void Update(T data);
    }

    // ✅ Subject interface
    public interface ISubject<T>
    {
        void Attach(IObserver<T> observer);
        void Detach(IObserver<T> observer);
        void Notify();
    }

    // ✅ Concrete Subject
    public class StockMarket : ISubject<decimal>
    {
        private decimal _price;
        private readonly List<IObserver<decimal>> _observers = new();

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                Notify(); // Automatyczne powiadomienie
            }
        }

        public void Attach(IObserver<decimal> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Detach(IObserver<decimal> observer)
        {
            _observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.Update(_price);
            }
        }
    }

    // ✅ Concrete Observers
    public class Investor : IObserver<decimal>
    {
        public string Name { get; set; }

        public Investor(string name) => Name = name;

        public void Update(decimal price)
        {
            Console.WriteLine($"{Name} notified: Price is {price}");
        }
    }

    public class PriceLogger : IObserver<decimal>
    {
        public void Update(decimal price)
        {
            Console.WriteLine($"[LOG] Price changed to {price} at {DateTime.Now}");
        }
    }

    public class PriceAlert : IObserver<decimal>
    {
        private readonly decimal _threshold;

        public PriceAlert(decimal threshold) => _threshold = threshold;

        public void Update(decimal price)
        {
            if (price > _threshold)
                Console.WriteLine($"⚠️ ALERT: Price {price} exceeded threshold {_threshold}");
        }
    }

    // ✅ Event-based Observer (C# native events)
    public class EventBasedStockMarket
    {
        private decimal _price;

        // ✅ C# event - built-in Observer pattern
        public event EventHandler<decimal>? PriceChanged;

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                PriceChanged?.Invoke(this, _price);
            }
        }
    }

    // ✅ Usage example
    public class ObserverExample
    {
        public static void Run()
        {
            // Classic Observer
            var stockMarket = new StockMarket();
            
            // ✅ Dynamiczne dodawanie obserwatorów
            var investor1 = new Investor("John");
            var investor2 = new Investor("Jane");
            var logger = new PriceLogger();
            var alert = new PriceAlert(100);

            stockMarket.Attach(investor1);
            stockMarket.Attach(investor2);
            stockMarket.Attach(logger);
            stockMarket.Attach(alert);

            // Update - wszyscy obserwatorzy są notyfikowani
            stockMarket.Price = 95;
            stockMarket.Price = 105;

            // ✅ Dynamiczne usuwanie
            stockMarket.Detach(investor1);
            stockMarket.Price = 110;

            // Event-based
            var eventStock = new EventBasedStockMarket();
            eventStock.PriceChanged += (sender, price) => 
                Console.WriteLine($"Event: Price is {price}");
            
            eventStock.Price = 120;
        }
    }

    // ✅ Generic Subject base class
    public abstract class Subject<T> : ISubject<T>
    {
        private readonly List<IObserver<T>> _observers = new();

        public void Attach(IObserver<T> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Detach(IObserver<T> observer)
        {
            _observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.Update(GetState());
            }
        }

        protected abstract T GetState();
    }

    // ✅ Weather Station example
    public class WeatherData
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
    }

    public class WeatherStation : Subject<WeatherData>
    {
        private WeatherData _data = new();

        public void UpdateMeasurements(double temp, double humidity, double pressure)
        {
            _data = new WeatherData
            {
                Temperature = temp,
                Humidity = humidity,
                Pressure = pressure
            };
            Notify();
        }

        protected override WeatherData GetState() => _data;
    }

    public class CurrentConditionsDisplay : IObserver<WeatherData>
    {
        public void Update(WeatherData data)
        {
            Console.WriteLine($"Current: {data.Temperature}°C, {data.Humidity}% humidity");
        }
    }

    public class StatisticsDisplay : IObserver<WeatherData>
    {
        private readonly List<double> _temperatures = new();

        public void Update(WeatherData data)
        {
            _temperatures.Add(data.Temperature);
            var avg = _temperatures.Average();
            Console.WriteLine($"Avg temperature: {avg:F1}°C");
        }
    }
}
