using System;

namespace DesignPatterns.Observer.Bad3
{
    // ❌ BAD: Thread safety issues

    public class BadThreadSafeSubject
    {
        private System.Collections.Generic.List<IObserver> _observers = new();
        
        public void Attach(IObserver observer)
        {
            // ❌ Not thread-safe
            _observers.Add(observer);
        }

        public void Notify()
        {
            // ❌ Collection może być modyfikowana podczas iteracji
            foreach (var observer in _observers)
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
