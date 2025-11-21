using System;
using System.Collections.Generic;

namespace DesignPatterns.Observer.Good3
{
    // ✅ GOOD: Thread-safe implementation

    public class ThreadSafeSubject
    {
        private readonly object _lock = new();
        private readonly List<IObserver> _observers = new();

        public void Attach(IObserver observer)
        {
            lock (_lock)
            {
                if (!_observers.Contains(observer))
                    _observers.Add(observer);
            }
        }

        public void Detach(IObserver observer)
        {
            lock (_lock)
            {
                _observers.Remove(observer);
            }
        }

        public void Notify()
        {
            List<IObserver> observersCopy;
            lock (_lock)
            {
                observersCopy = new List<IObserver>(_observers);
            }

            foreach (var observer in observersCopy)
            {
                observer.Update();
            }
        }
    }

    public interface IObserver
    {
        void Update();
    }
}
