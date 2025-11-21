using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesignPatterns.Facade.Good3
{
    // ✅ GOOD: Facade for multiple external services - single unified interface

    // ✅ External services (same as BadExample3)
    public class SendGridEmailService
    {
        public void SetApiKey(string apiKey) => Console.WriteLine("Setting SendGrid key...");
        public void CreateMessage(string from, string to, string subject, string body) { }
        public void AddAttachment(byte[] content, string filename) { }
        public void Send() => Console.WriteLine("Email sent via SendGrid");
    }

    public class TwilioSmsService
    {
        public void Authenticate(string accountSid, string authToken) => Console.WriteLine("Twilio authenticated");
        public void SetFromNumber(string phoneNumber) { }
        public void SendSms(string to, string message) => Console.WriteLine($"SMS sent to {to}");
    }

    public class FirebasePushService
    {
        public void InitializeApp(string projectId, string apiKey) => Console.WriteLine("Firebase initialized");
        public void SetTargetDevice(string deviceToken) { }
        public void CreateNotification(string title, string body, Dictionary<string, string> data) { }
        public void SendPush() => Console.WriteLine("Push notification sent");
    }

    public class DatadogLogger
    {
        public void Connect(string apiKey, string appKey) => Console.WriteLine("Datadog connected");
        public void SetTags(string[] tags) { }
        public void LogInfo(string message) => Console.WriteLine($"[INFO] {message}");
        public void LogError(string message, Exception ex) => Console.WriteLine($"[ERROR] {message}");
    }

    public class SegmentAnalytics
    {
        public void Initialize(string writeKey) => Console.WriteLine("Segment initialized");
        public void IdentifyUser(string userId, Dictionary<string, object> traits) { }
        public void TrackEvent(string eventName, Dictionary<string, object> properties) 
        {
            Console.WriteLine($"Event tracked: {eventName}");
        }
    }

    // ✅ Configuration class
    public class NotificationConfig
    {
        public string SendGridApiKey { get; set; }
        public string TwilioAccountSid { get; set; }
        public string TwilioAuthToken { get; set; }
        public string TwilioFromNumber { get; set; }
        public string FirebaseProjectId { get; set; }
        public string FirebaseApiKey { get; set; }
        public string DatadogApiKey { get; set; }
        public string DatadogAppKey { get; set; }
        public string SegmentWriteKey { get; set; }
    }

    // ✅ Notification request DTO
    public class WelcomeNotificationRequest
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DeviceToken { get; set; }
        public Dictionary<string, object> UserTraits { get; set; }
    }

    // ✅ Facade - unified interface over multiple external services
    public class NotificationFacade
    {
        private readonly SendGridEmailService _emailService;
        private readonly TwilioSmsService _smsService;
        private readonly FirebasePushService _pushService;
        private readonly DatadogLogger _logger;
        private readonly SegmentAnalytics _analytics;

        public NotificationFacade(NotificationConfig config)
        {
            _emailService = new SendGridEmailService();
            _smsService = new TwilioSmsService();
            _pushService = new FirebasePushService();
            _logger = new DatadogLogger();
            _analytics = new SegmentAnalytics();

            // ✅ Inicjalizacja ukryta w konstruktorze
            InitializeServices(config);
        }

        private void InitializeServices(NotificationConfig config)
        {
            // ✅ Complexity ukryta - centralized configuration
            _emailService.SetApiKey(config.SendGridApiKey);
            
            _smsService.Authenticate(config.TwilioAccountSid, config.TwilioAuthToken);
            _smsService.SetFromNumber(config.TwilioFromNumber);
            
            _pushService.InitializeApp(config.FirebaseProjectId, config.FirebaseApiKey);
            
            _logger.Connect(config.DatadogApiKey, config.DatadogAppKey);
            _logger.SetTags(new[] { "env:prod", "service:notifications" });
            
            _analytics.Initialize(config.SegmentWriteKey);
        }

        // ✅ Single method for complete welcome flow
        public async Task SendWelcomeNotificationsAsync(WelcomeNotificationRequest request)
        {
            try
            {
                _logger.LogInfo($"Sending welcome notifications to user {request.UserId}");

                // ✅ Send all notifications in parallel
                var tasks = new List<Task>
                {
                    SendWelcomeEmailAsync(request.Email),
                    SendWelcomeSmsAsync(request.Phone),
                    TrackUserOnboardingAsync(request)
                };

                // ✅ Optional push notification
                if (!string.IsNullOrEmpty(request.DeviceToken))
                {
                    tasks.Add(SendWelcomePushAsync(request.DeviceToken));
                }

                await Task.WhenAll(tasks);

                _logger.LogInfo($"All welcome notifications sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send welcome notifications", ex);
                throw;
            }
        }

        // ✅ Individual notification methods (complexity ukryta)
        private Task SendWelcomeEmailAsync(string email)
        {
            return Task.Run(() =>
            {
                _emailService.CreateMessage(
                    "noreply@company.com",
                    email,
                    "Welcome to Our Platform!",
                    "Thank you for joining us. We're excited to have you!");
                _emailService.Send();
            });
        }

        private Task SendWelcomeSmsAsync(string phone)
        {
            return Task.Run(() =>
            {
                _smsService.SendSms(phone, "Welcome! Thanks for joining our platform.");
            });
        }

        private Task SendWelcomePushAsync(string deviceToken)
        {
            return Task.Run(() =>
            {
                _pushService.SetTargetDevice(deviceToken);
                _pushService.CreateNotification(
                    "Welcome!",
                    "Thanks for joining us",
                    new Dictionary<string, string> { { "type", "welcome" } });
                _pushService.SendPush();
            });
        }

        private Task TrackUserOnboardingAsync(WelcomeNotificationRequest request)
        {
            return Task.Run(() =>
            {
                var traits = request.UserTraits ?? new Dictionary<string, object>();
                traits["email"] = request.Email;
                traits["phone"] = request.Phone;
                traits["signup_date"] = DateTime.UtcNow;

                _analytics.IdentifyUser(request.UserId, traits);
                _analytics.TrackEvent("user_onboarded", new Dictionary<string, object>
                {
                    { "userId", request.UserId },
                    { "source", "web" }
                });
            });
        }

        // ✅ Additional convenience methods
        public async Task SendTransactionalEmailAsync(string to, string subject, string body)
        {
            try
            {
                _logger.LogInfo($"Sending transactional email to {to}");
                await Task.Run(() =>
                {
                    _emailService.CreateMessage("noreply@company.com", to, subject, body);
                    _emailService.Send();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send email", ex);
                throw;
            }
        }

        public async Task SendAlertSmsAsync(string phone, string message)
        {
            try
            {
                _logger.LogInfo($"Sending alert SMS to {phone}");
                await Task.Run(() =>
                {
                    _smsService.SendSms(phone, message);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send SMS", ex);
                throw;
            }
        }

        public void TrackEvent(string userId, string eventName, Dictionary<string, object> properties = null)
        {
            try
            {
                _logger.LogInfo($"Tracking event {eventName} for user {userId}");
                _analytics.TrackEvent(eventName, properties ?? new Dictionary<string, object>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to track event {eventName}", ex);
                // Don't throw - analytics failure shouldn't break flow
            }
        }
    }

    // ✅ Client - simple, clean code
    public class UserOnboardingService
    {
        private readonly NotificationFacade _notificationFacade;

        public UserOnboardingService(NotificationConfig config)
        {
            // ✅ Single facade wraps all external services
            _notificationFacade = new NotificationFacade(config);
        }

        public async Task OnboardNewUserAsync(string userId, string email, string phone, string deviceToken)
        {
            // ✅ Simple, one-line call - facade handles everything!
            await _notificationFacade.SendWelcomeNotificationsAsync(new WelcomeNotificationRequest
            {
                UserId = userId,
                Email = email,
                Phone = phone,
                DeviceToken = deviceToken,
                UserTraits = new Dictionary<string, object>
                {
                    { "plan", "free" },
                    { "source", "web" }
                }
            });

            Console.WriteLine($"User {userId} onboarded successfully!");

            // ✅ Brak orchestration
            // ✅ Brak inicjalizacji external services
            // ✅ Brak error handling (handled in facade)
            // ✅ Brak retry logic (can be added in facade)
            // ✅ Wszystko ukryte w facade!
        }

        public async Task SendOrderConfirmationAsync(string userId, string email, string orderNumber)
        {
            // ✅ Other operations also simple
            await _notificationFacade.SendTransactionalEmailAsync(
                email,
                "Order Confirmation",
                $"Your order {orderNumber} has been confirmed!");

            _notificationFacade.TrackEvent(userId, "order_confirmed", new Dictionary<string, object>
            {
                { "order_number", orderNumber }
            });
        }
    }

    // ✅ Benefits:
    // - Single facade wraps multiple external services
    // - Unified interface (email, SMS, push, analytics together)
    // - Configuration centralized
    // - Error handling centralized
    // - Easy to add retry logic, circuit breakers
    // - Client code is simple and clean
    // - Easy testing (mock facade)
    // - Can add caching, rate limiting in facade
    // - Parallel execution hidden in facade
}
