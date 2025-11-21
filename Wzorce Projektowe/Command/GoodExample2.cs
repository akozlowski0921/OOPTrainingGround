using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DesignPatterns.Command.Good2
{
    // ✅ GOOD: Command pattern for task scheduling/queuing in microservices

    // ✅ Command interface
    public interface ICommand
    {
        Task ExecuteAsync(CancellationToken ct = default);
        string CommandId { get; }
        int Priority { get; }
        DateTime? ScheduledFor { get; }
        int RetryCount { get; set; }
        int MaxRetries { get; }
    }

    // ✅ Base command with retry logic
    public abstract class BaseCommand : ICommand
    {
        public string CommandId { get; } = Guid.NewGuid().ToString();
        public int Priority { get; protected set; } = 5;
        public DateTime? ScheduledFor { get; protected set; }
        public int RetryCount { get; set; }
        public int MaxRetries { get; protected set; } = 3;

        public abstract Task ExecuteAsync(CancellationToken ct = default);
    }

    // ✅ Concrete Commands
    public class SendEmailCommand : BaseCommand
    {
        private readonly string _to;
        private readonly string _subject;
        private readonly string _body;

        public SendEmailCommand(string to, string subject, string body, int priority = 5)
        {
            _to = to;
            _subject = subject;
            _body = body;
            Priority = priority;
        }

        public override async Task ExecuteAsync(CancellationToken ct = default)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Sending email to {_to}: {_subject}");
            await Task.Delay(100, ct); // Simulate async work
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Email sent to {_to}");
        }
    }

    public class SendSmsCommand : BaseCommand
    {
        private readonly string _phone;
        private readonly string _message;

        public SendSmsCommand(string phone, string message, DateTime? scheduledFor = null)
        {
            _phone = phone;
            _message = message;
            ScheduledFor = scheduledFor;
            Priority = 3; // Higher priority
        }

        public override async Task ExecuteAsync(CancellationToken ct = default)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Sending SMS to {_phone}: {_message}");
            await Task.Delay(100, ct);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] SMS sent to {_phone}");
        }
    }

    public class ProcessOrderCommand : BaseCommand
    {
        private readonly int _orderId;

        public ProcessOrderCommand(int orderId)
        {
            _orderId = orderId;
            Priority = 1; // Highest priority
            MaxRetries = 5;
        }

        public override async Task ExecuteAsync(CancellationToken ct = default)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Processing order {_orderId}");
            await Task.Delay(200, ct);
            
            // Simulate occasional failure
            if (RetryCount < 2 && new Random().Next(10) < 3)
            {
                throw new Exception("Temporary failure");
            }
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Order {_orderId} processed");
        }
    }

    // ✅ Command Queue with priority and scheduling
    public class CommandQueue
    {
        private readonly ConcurrentQueue<ICommand> _queue = new();
        private readonly SemaphoreSlim _signal = new(0);
        private readonly CancellationTokenSource _cts = new();
        private readonly List<Task> _workers = new();

        public void Start(int workerCount = 3)
        {
            for (int i = 0; i < workerCount; i++)
            {
                var worker = Task.Run(() => ProcessCommandsAsync(i), _cts.Token);
                _workers.Add(worker);
            }
        }

        // ✅ Enqueue command
        public void Enqueue(ICommand command)
        {
            _queue.Enqueue(command);
            _signal.Release();
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Command {command.CommandId} queued (Priority: {command.Priority})");
        }

        // ✅ Worker processing loop
        private async Task ProcessCommandsAsync(int workerId)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Worker {workerId} started");

            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    await _signal.WaitAsync(_cts.Token);

                    // Get next command (with priority)
                    var commands = new List<ICommand>();
                    while (_queue.TryDequeue(out var cmd))
                    {
                        commands.Add(cmd);
                    }

                    // Sort by priority and scheduled time
                    var command = commands
                        .Where(c => !c.ScheduledFor.HasValue || c.ScheduledFor.Value <= DateTime.UtcNow)
                        .OrderBy(c => c.Priority)
                        .FirstOrDefault();

                    if (command == null)
                    {
                        // Re-queue commands that are scheduled for later
                        foreach (var c in commands)
                        {
                            _queue.Enqueue(c);
                            _signal.Release();
                        }
                        await Task.Delay(100);
                        continue;
                    }

                    // Re-queue other commands
                    foreach (var c in commands.Where(c => c != command))
                    {
                        _queue.Enqueue(c);
                        _signal.Release();
                    }

                    // Execute command with retry logic
                    await ExecuteWithRetryAsync(command, workerId);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Worker {workerId} error: {ex.Message}");
                }
            }

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Worker {workerId} stopped");
        }

        // ✅ Execute with retry
        private async Task ExecuteWithRetryAsync(ICommand command, int workerId)
        {
            while (command.RetryCount <= command.MaxRetries)
            {
                try
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Worker {workerId} executing command {command.CommandId} (Attempt {command.RetryCount + 1})");
                    await command.ExecuteAsync(_cts.Token);
                    return; // Success
                }
                catch (Exception ex)
                {
                    command.RetryCount++;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Command {command.CommandId} failed: {ex.Message}");

                    if (command.RetryCount > command.MaxRetries)
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Command {command.CommandId} failed after {command.MaxRetries} retries");
                        // Could move to dead letter queue here
                        return;
                    }

                    // Exponential backoff
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, command.RetryCount)), _cts.Token);
                }
            }
        }

        public async Task StopAsync()
        {
            _cts.Cancel();
            await Task.WhenAll(_workers);
        }
    }

    // ✅ Usage example
    public class TaskSchedulingExample
    {
        public static async Task RunAsync()
        {
            var queue = new CommandQueue();
            queue.Start(workerCount: 3);

            // ✅ Enqueue commands with different priorities
            queue.Enqueue(new SendEmailCommand("user@example.com", "Welcome", "Welcome to our service", priority: 5));
            queue.Enqueue(new ProcessOrderCommand(123)); // High priority
            queue.Enqueue(new SendSmsCommand("555-1234", "Your order is ready"));
            
            // ✅ Schedule command for later
            var scheduledTime = DateTime.UtcNow.AddSeconds(5);
            queue.Enqueue(new SendEmailCommand("user@example.com", "Reminder", "Don't forget!", priority: 7));

            await Task.Delay(10000); // Let workers process
            await queue.StopAsync();
        }
    }

    // ✅ Benefits:
    // - Asynchronous execution (non-blocking)
    // - Task queuing with priority
    // - Retry logic with exponential backoff
    // - Scheduled execution
    // - Scalable with multiple workers
    // - Testable and maintainable
}
