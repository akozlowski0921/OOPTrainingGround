using System;
using System.Collections.Generic;

namespace DesignPatterns.Observer.Good2
{
    // ✅ GOOD: Weak reference pattern

    public interface IObserver<T>
    {
        void Update(T data);
    }

    public class WeakObserverManager<T>
    {
        private readonly List<WeakReference<IObserver<T>>> _observers = new();

        public void Attach(IObserver<T> observer)
        {
            _observers.Add(new WeakReference<IObserver<T>>(observer));
        }

        public void Notify(T data)
        {
            _observers.RemoveAll(wr => !wr.TryGetTarget(out _));
            
            foreach (var weakRef in _observers)
            {
                if (weakRef.TryGetTarget(out var observer))
                {
                    observer.Update(data);
                }
            }
        }
    }
}
