using System;
using Microsoft.Extensions.DependencyInjection;

namespace DesignPatterns.Factory
{
    // ✅ GOOD: Factory Pattern

    // ✅ 1. Simple Factory
    public interface IPayment
    {
        void Process(decimal amount);
    }

    public class CreditCard : IPayment
    {
        public void Process(decimal amount) => Console.WriteLine($"Processing CC: ${amount}");
    }

    public class PayPal : IPayment
    {
        public void Process(decimal amount) => Console.WriteLine($"Processing PayPal: ${amount}");
    }

    public class BankTransfer : IPayment
    {
        public void Process(decimal amount) => Console.WriteLine($"Processing Bank: ${amount}");
    }

    public enum PaymentType { CreditCard, PayPal, BankTransfer }

    // ✅ Factory Method
    public class PaymentFactory
    {
        public static IPayment Create(PaymentType type)
        {
            return type switch
            {
                PaymentType.CreditCard => new CreditCard(),
                PaymentType.PayPal => new PayPal(),
                PaymentType.BankTransfer => new BankTransfer(),
                _ => throw new ArgumentException("Invalid payment type")
            };
        }
    }

    // ✅ Usage
    public class GoodPaymentProcessor
    {
        public void ProcessPayment(PaymentType type, decimal amount)
        {
            // ✅ Factory tworzy obiekt
            var payment = PaymentFactory.Create(type);
            payment.Process(amount);
        }
    }

    // ✅ 2. Factory Method Pattern (GOF)
    public abstract class PaymentCreator
    {
        // Factory Method
        public abstract IPayment CreatePayment();

        public void ProcessPayment(decimal amount)
        {
            var payment = CreatePayment();
            payment.Process(amount);
        }
    }

    public class CreditCardCreator : PaymentCreator
    {
        public override IPayment CreatePayment() => new CreditCard();
    }

    public class PayPalCreator : PaymentCreator
    {
        public override IPayment CreatePayment() => new PayPal();
    }

    // ✅ 3. Abstract Factory Pattern
    public interface INotification
    {
        void Send(string message);
    }

    public interface ILogger
    {
        void Log(string message);
    }

    // ✅ Abstract Factory interface
    public interface IServiceFactory
    {
        INotification CreateNotification();
        ILogger CreateLogger();
    }

    // Concrete products
    public class EmailNotification : INotification
    {
        public void Send(string message) => Console.WriteLine($"Email: {message}");
    }

    public class SmsNotification : INotification
    {
        public void Send(string message) => Console.WriteLine($"SMS: {message}");
    }

    public class FileLogger : ILogger
    {
        public void Log(string message) => Console.WriteLine($"File log: {message}");
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message) => Console.WriteLine($"Console: {message}");
    }

    // Concrete factories
    public class DevelopmentServiceFactory : IServiceFactory
    {
        public INotification CreateNotification() => new SmsNotification();
        public ILogger CreateLogger() => new ConsoleLogger();
    }

    public class ProductionServiceFactory : IServiceFactory
    {
        public INotification CreateNotification() => new EmailNotification();
        public ILogger CreateLogger() => new FileLogger();
    }

    // ✅ Usage
    public class GoodOrderService
    {
        private readonly IServiceFactory _factory;

        public GoodOrderService(IServiceFactory factory)
        {
            _factory = factory;
        }

        public void CreateOrder(string customer, decimal amount)
        {
            var notification = _factory.CreateNotification();
            var logger = _factory.CreateLogger();

            logger.Log($"Creating order for {customer}");
            notification.Send($"Order: ${amount}");
        }
    }

    // ✅ 4. Dependency Injection Integration
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPaymentServices(this IServiceCollection services)
        {
            // ✅ Factory jako Func<>
            services.AddTransient<Func<PaymentType, IPayment>>(provider => type =>
            {
                return type switch
                {
                    PaymentType.CreditCard => provider.GetRequiredService<CreditCard>(),
                    PaymentType.PayPal => provider.GetRequiredService<PayPal>(),
                    PaymentType.BankTransfer => provider.GetRequiredService<BankTransfer>(),
                    _ => throw new ArgumentException("Invalid type")
                };
            });

            services.AddTransient<CreditCard>();
            services.AddTransient<PayPal>();
            services.AddTransient<BankTransfer>();

            return services;
        }

        public static IServiceCollection AddServiceFactory(
            this IServiceCollection services, 
            bool isProduction)
        {
            // ✅ Rejestracja odpowiedniej factory w DI
            if (isProduction)
                services.AddSingleton<IServiceFactory, ProductionServiceFactory>();
            else
                services.AddSingleton<IServiceFactory, DevelopmentServiceFactory>();

            return services;
        }
    }

    // ✅ 5. Modern approach z named registrations
    public interface IPaymentFactory
    {
        IPayment Create(PaymentType type);
    }

    public class ModernPaymentFactory : IPaymentFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ModernPaymentFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPayment Create(PaymentType type)
        {
            // ✅ DI tworzy instancje z dependencies
            return type switch
            {
                PaymentType.CreditCard => _serviceProvider.GetRequiredService<CreditCard>(),
                PaymentType.PayPal => _serviceProvider.GetRequiredService<PayPal>(),
                PaymentType.BankTransfer => _serviceProvider.GetRequiredService<BankTransfer>(),
                _ => throw new ArgumentException("Invalid payment type")
            };
        }
    }

    // ✅ Usage with DI
    public class PaymentService
    {
        private readonly IPaymentFactory _factory;

        public PaymentService(IPaymentFactory factory)
        {
            _factory = factory;
        }

        public void Process(PaymentType type, decimal amount)
        {
            var payment = _factory.Create(type);
            payment.Process(amount);
        }
    }
}
