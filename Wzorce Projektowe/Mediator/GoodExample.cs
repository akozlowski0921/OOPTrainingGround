using System;
using System.Collections.Generic;

namespace DesignPatterns.Mediator
{
    // ✅ GOOD: Mediator Pattern

    // ✅ Mediator interface
    public interface IChatMediator
    {
        void SendMessage(string message, User sender);
        void AddUser(User user);
    }

    // ✅ Concrete Mediator
    public class ChatRoom : IChatMediator
    {
        private readonly List<User> _users = new();

        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public void SendMessage(string message, User sender)
        {
            // ✅ Mediator zarządza komunikacją
            foreach (var user in _users)
            {
                if (user != sender)
                    user.ReceiveMessage(message, sender.Name);
            }
        }
    }

    // ✅ Colleague
    public class User
    {
        private readonly IChatMediator _mediator;
        public string Name { get; }

        public User(string name, IChatMediator mediator)
        {
            Name = name;
            _mediator = mediator;
            _mediator.AddUser(this);
        }

        public void SendMessage(string message)
        {
            Console.WriteLine($"{Name} sends: {message}");
            _mediator.SendMessage(message, this);
        }

        public void ReceiveMessage(string message, string from)
        {
            Console.WriteLine($"{Name} received from {from}: {message}");
        }
    }

    // ✅ Usage
    public class MediatorExample
    {
        public static void Run()
        {
            var chatRoom = new ChatRoom();

            var user1 = new User("Alice", chatRoom);
            var user2 = new User("Bob", chatRoom);
            var user3 = new User("Charlie", chatRoom);

            user1.SendMessage("Hello everyone!");
            user2.SendMessage("Hi Alice!");
        }
    }

    // ✅ Advanced: MediatR library example (ASP.NET Core)
    public interface IRequest<out TResponse> { }
    public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken ct);
    }

    public class CreateOrderCommand : IRequest<int>
    {
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
    }

    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
    {
        public Task<int> Handle(CreateOrderCommand request, CancellationToken ct)
        {
            // Create order logic
            Console.WriteLine($"Creating order for {request.CustomerName}");
            return Task.FromResult(123); // Order ID
        }
    }
}
