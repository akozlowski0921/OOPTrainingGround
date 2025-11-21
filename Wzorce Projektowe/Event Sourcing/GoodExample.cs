using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DesignPatterns.EventSourcing.Good
{
    // ✅ GOOD: Event Sourcing - store events, not state

    // ✅ Domain Events - immutable records of what happened
    public abstract class DomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public Guid AggregateId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Version { get; set; }
        public string UserId { get; set; } // Who made the change
    }

    public class AccountOpenedEvent : DomainEvent
    {
        public string AccountHolder { get; set; }
        public decimal InitialBalance { get; set; }
    }

    public class MoneyDepositedEvent : DomainEvent
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

    public class MoneyWithdrawnEvent : DomainEvent
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

    public class AccountClosedEvent : DomainEvent
    {
        public string Reason { get; set; }
    }

    // ✅ Aggregate Root - builds state from events
    public class BankAccount
    {
        // ✅ Current state (built from events)
        public Guid AccountId { get; private set; }
        public string AccountHolder { get; private set; }
        public decimal Balance { get; private set; }
        public int Version { get; private set; }
        public bool IsClosed { get; private set; }

        // ✅ Uncommitted events (new events not yet saved)
        private readonly List<DomainEvent> _uncommittedEvents = new();

        private BankAccount() { } // For reconstruction

        // ✅ Factory method - creates new account
        public static BankAccount Open(string accountHolder, decimal initialBalance, string userId)
        {
            if (initialBalance < 0)
                throw new InvalidOperationException("Initial balance cannot be negative");

            var account = new BankAccount();
            account.Apply(new AccountOpenedEvent
            {
                AggregateId = Guid.NewGuid(),
                AccountHolder = accountHolder,
                InitialBalance = initialBalance,
                Version = 1,
                UserId = userId
            });
            return account;
        }

        // ✅ Command handlers - generate events
        public void Deposit(decimal amount, string description, string userId)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Deposit amount must be positive");
            
            if (IsClosed)
                throw new InvalidOperationException("Cannot deposit to closed account");

            Apply(new MoneyDepositedEvent
            {
                AggregateId = AccountId,
                Amount = amount,
                Description = description,
                Version = Version + 1,
                UserId = userId
            });
        }

        public void Withdraw(decimal amount, string description, string userId)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Withdrawal amount must be positive");
            
            if (IsClosed)
                throw new InvalidOperationException("Cannot withdraw from closed account");

            if (Balance < amount)
                throw new InvalidOperationException("Insufficient funds");

            Apply(new MoneyWithdrawnEvent
            {
                AggregateId = AccountId,
                Amount = amount,
                Description = description,
                Version = Version + 1,
                UserId = userId
            });
        }

        public void Close(string reason, string userId)
        {
            if (IsClosed)
                throw new InvalidOperationException("Account already closed");

            if (Balance != 0)
                throw new InvalidOperationException("Cannot close account with non-zero balance");

            Apply(new AccountClosedEvent
            {
                AggregateId = AccountId,
                Reason = reason,
                Version = Version + 1,
                UserId = userId
            });
        }

        // ✅ Apply event (for new events)
        private void Apply(DomainEvent @event)
        {
            When(@event); // Update state
            _uncommittedEvents.Add(@event); // Track for persistence
        }

        // ✅ Load from history (for rebuilding from event store)
        public static BankAccount LoadFromHistory(IEnumerable<DomainEvent> history)
        {
            var account = new BankAccount();
            foreach (var @event in history)
            {
                account.When(@event);
            }
            return account;
        }

        // ✅ Event handlers - rebuild state from events
        private void When(DomainEvent @event)
        {
            switch (@event)
            {
                case AccountOpenedEvent e:
                    AccountId = e.AggregateId;
                    AccountHolder = e.AccountHolder;
                    Balance = e.InitialBalance;
                    Version = e.Version;
                    break;

                case MoneyDepositedEvent e:
                    Balance += e.Amount;
                    Version = e.Version;
                    break;

                case MoneyWithdrawnEvent e:
                    Balance -= e.Amount;
                    Version = e.Version;
                    break;

                case AccountClosedEvent e:
                    IsClosed = true;
                    Version = e.Version;
                    break;
            }
        }

        // ✅ Get uncommitted events for persistence
        public IEnumerable<DomainEvent> GetUncommittedEvents() => _uncommittedEvents.AsReadOnly();

        // ✅ Mark events as committed
        public void MarkEventsAsCommitted() => _uncommittedEvents.Clear();
    }

    // ✅ Event Store - append-only storage for events
    public interface IEventStore
    {
        Task SaveEventsAsync(Guid aggregateId, IEnumerable<DomainEvent> events, int expectedVersion, CancellationToken ct = default);
        Task<List<DomainEvent>> GetEventsAsync(Guid aggregateId, CancellationToken ct = default);
        Task<List<DomainEvent>> GetEventsAsync(Guid aggregateId, int fromVersion, CancellationToken ct = default);
        Task<List<DomainEvent>> GetAllEventsAsync(CancellationToken ct = default);
    }

    // ✅ In-memory implementation (for demo)
    public class InMemoryEventStore : IEventStore
    {
        private readonly List<DomainEvent> _events = new();

        public Task SaveEventsAsync(Guid aggregateId, IEnumerable<DomainEvent> events, int expectedVersion, CancellationToken ct = default)
        {
            // ✅ Optimistic concurrency check
            var currentVersion = _events
                .Where(e => e.AggregateId == aggregateId)
                .Select(e => e.Version)
                .DefaultIfEmpty(0)
                .Max();

            if (currentVersion != expectedVersion)
                throw new InvalidOperationException("Concurrency conflict");

            // ✅ Append events (never update or delete!)
            _events.AddRange(events);
            return Task.CompletedTask;
        }

        public Task<List<DomainEvent>> GetEventsAsync(Guid aggregateId, CancellationToken ct = default)
        {
            var events = _events
                .Where(e => e.AggregateId == aggregateId)
                .OrderBy(e => e.Version)
                .ToList();
            return Task.FromResult(events);
        }

        public Task<List<DomainEvent>> GetEventsAsync(Guid aggregateId, int fromVersion, CancellationToken ct = default)
        {
            var events = _events
                .Where(e => e.AggregateId == aggregateId && e.Version >= fromVersion)
                .OrderBy(e => e.Version)
                .ToList();
            return Task.FromResult(events);
        }

        public Task<List<DomainEvent>> GetAllEventsAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_events.OrderBy(e => e.Timestamp).ToList());
        }
    }

    // ✅ Repository - load and save aggregates
    public class BankAccountRepository
    {
        private readonly IEventStore _eventStore;

        public BankAccountRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        // ✅ Load aggregate by replaying events
        public async Task<BankAccount> GetByIdAsync(Guid accountId, CancellationToken ct = default)
        {
            var events = await _eventStore.GetEventsAsync(accountId, ct);
            if (!events.Any())
                return null;

            return BankAccount.LoadFromHistory(events);
        }

        // ✅ Save aggregate by persisting new events
        public async Task SaveAsync(BankAccount account, CancellationToken ct = default)
        {
            var newEvents = account.GetUncommittedEvents();
            if (!newEvents.Any())
                return;

            await _eventStore.SaveEventsAsync(
                account.AccountId,
                newEvents,
                account.Version - newEvents.Count(),
                ct);

            account.MarkEventsAsCommitted();
        }
    }

    // ✅ Usage example
    public class EventSourcingExample
    {
        public static async Task RunAsync()
        {
            var eventStore = new InMemoryEventStore();
            var repository = new BankAccountRepository(eventStore);

            // ✅ Create account
            var account = BankAccount.Open("John Doe", 1000m, "user123");
            await repository.SaveAsync(account);

            // ✅ Perform operations
            var accountId = account.AccountId;
            account = await repository.GetByIdAsync(accountId);
            
            account.Deposit(500m, "Salary", "user123");
            account.Withdraw(200m, "Shopping", "user123");
            account.Deposit(100m, "Refund", "user123");
            
            await repository.SaveAsync(account);

            // ✅ Reload from events
            var reloadedAccount = await repository.GetByIdAsync(accountId);
            Console.WriteLine($"Balance: {reloadedAccount.Balance}"); // 1400

            // ✅ View event history
            var events = await eventStore.GetEventsAsync(accountId);
            foreach (var evt in events)
            {
                Console.WriteLine($"{evt.Timestamp}: {evt.GetType().Name}");
            }
        }
    }

    // ✅ Benefits:
    // - Complete audit trail (who, when, what)
    // - Rebuild state at any point in time
    // - Debug by replaying events
    // - Temporal queries
    // - Event replay for new features
    // - Compliance and regulatory requirements
}
