using System;

namespace DesignPatterns.Mediator
{
    // ❌ BAD: Komponenty znają się nawzajem - tight coupling

    public class BadChatRoom
    {
        private BadUser _user1;
        private BadUser _user2;
        private BadUser _user3;

        public void AddUser(BadUser user)
        {
            // ❌ Każdy user musi znać wszystkich innych
            if (_user1 == null) _user1 = user;
            else if (_user2 == null) _user2 = user;
            else if (_user3 == null) _user3 = user;
        }
    }

    public class BadUser
    {
        public string Name { get; set; }
        private BadUser _otherUser1;
        private BadUser _otherUser2;

        public BadUser(string name) => Name = name;

        // ❌ User musi znać innych userów
        public void SetOtherUsers(BadUser user1, BadUser user2)
        {
            _otherUser1 = user1;
            _otherUser2 = user2;
        }

        public void SendMessage(string message)
        {
            // ❌ Bezpośrednia komunikacja
            _otherUser1?.ReceiveMessage(Name, message);
            _otherUser2?.ReceiveMessage(Name, message);
        }

        public void ReceiveMessage(string from, string message)
        {
            Console.WriteLine($"{Name} received from {from}: {message}");
        }
    }
}
