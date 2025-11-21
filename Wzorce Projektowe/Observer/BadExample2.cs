using System;

namespace DesignPatterns.Observer.Bad2
{
    // ❌ BAD: Więcej problemów Observer

    // BŁĄD: Tight coupling z notification method
    public class Subject
    {
        private int _state;

        public void SetState(int state)
        {
            _state = state;
            // ❌ Hard-coded notification
            Console.WriteLine($"State changed to {_state}");
            SendEmail(_state);
            LogToFile(_state);
        }

        private void SendEmail(int state) { }
        private void LogToFile(int state) { }
    }

    // BŁĄD: Memory leaks - brak detach
    public class LeakyObserver
    {
        public void Subscribe(Subject subject)
        {
            // ❌ Nigdy nie odsubskrybuje
        }
    }
}
