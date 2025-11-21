using System;
using System.Collections.Generic;

namespace DesignPatterns.Factory.Good3
{
    // ✅ GOOD: Registry pattern

    public interface IService
    {
        void Execute();
    }

    public class ServiceFactory
    {
        private readonly Dictionary<string, Func<IService>> _registry = new();

        public void Register(string key, Func<IService> creator)
        {
            _registry[key] = creator;
        }

        public IService Create(string key)
        {
            if (_registry.TryGetValue(key, out var creator))
                return creator();
            
            throw new ArgumentException($"Unknown service type: {key}");
        }
    }

    public class EmailService : IService
    {
        public void Execute() => Console.WriteLine("Email service");
    }

    public class SmsService : IService
    {
        public void Execute() => Console.WriteLine("SMS service");
    }
}
