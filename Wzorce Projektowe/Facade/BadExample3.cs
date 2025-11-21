using System;
using System.Collections.Generic;

namespace DesignPatterns.Facade.Bad3
{
    // ❌ BAD: No facade for multiple external services - client orchestrates everything

    // ❌ External Email Service API
    public class SendGridEmailService
    {
        public void SetApiKey(string apiKey) 
        {
            Console.WriteLine("Setting SendGrid API key...");
        }
        
        public void CreateMessage(string from, string to, string subject, string body) 
        {
            Console.WriteLine($"Creating email message to {to}");
        }
        
        public void AddAttachment(byte[] content, string filename) 
        {
            Console.WriteLine($"Adding attachment: {filename}");
        }
        
        public void Send() 
        {
            Console.WriteLine("Sending email via SendGrid...");
        }
    }

    // ❌ External SMS Service API
    public class TwilioSmsService
    {
        public void Authenticate(string accountSid, string authToken) 
        {
            Console.WriteLine("Authenticating with Twilio...");
        }
        
        public void SetFromNumber(string phoneNumber) 
        {
            Console.WriteLine($"Setting from number: {phoneNumber}");
        }
        
        public void SendSms(string to, string message) 
        {
            Console.WriteLine($"Sending SMS to {to}: {message}");
        }
    }

    // ❌ External Push Notification Service API
    public class FirebasePushService
    {
        public void InitializeApp(string projectId, string apiKey) 
        {
            Console.WriteLine($"Initializing Firebase for project {projectId}");
        }
        
        public void SetTargetDevice(string deviceToken) 
        {
            Console.WriteLine($"Setting target device: {deviceToken}");
        }
        
        public void CreateNotification(string title, string body, Dictionary<string, string> data) 
        {
            Console.WriteLine($"Creating notification: {title}");
        }
        
        public void SendPush() 
        {
            Console.WriteLine("Sending push notification via Firebase...");
        }
    }

    // ❌ External Logging Service API
    public class DatadogLogger
    {
        public void Connect(string apiKey, string appKey) 
        {
            Console.WriteLine("Connecting to Datadog...");
        }
        
        public void SetTags(string[] tags) 
        {
            Console.WriteLine($"Setting tags: {string.Join(", ", tags)}");
        }
        
        public void LogInfo(string message) 
        {
            Console.WriteLine($"[Datadog INFO] {message}");
        }
        
        public void LogError(string message, Exception ex) 
        {
            Console.WriteLine($"[Datadog ERROR] {message}: {ex?.Message}");
        }
    }

    // ❌ External Analytics Service API
    public class SegmentAnalytics
    {
        public void Initialize(string writeKey) 
        {
            Console.WriteLine("Initializing Segment...");
        }
        
        public void IdentifyUser(string userId, Dictionary<string, object> traits) 
        {
            Console.WriteLine($"Identifying user: {userId}");
        }
        
        public void TrackEvent(string eventName, Dictionary<string, object> properties) 
        {
            Console.WriteLine($"Tracking event: {eventName}");
        }
    }

    // ❌ Client musi zarządzać wszystkimi external services
    public class UserOnboardingService
    {
        public void OnboardNewUser(string userId, string email, string phone, string deviceToken)
        {
            // ❌ Złożona inicjalizacja wielu external services
            var emailService = new SendGridEmailService();
            var smsService = new TwilioSmsService();
            var pushService = new FirebasePushService();
            var logger = new DatadogLogger();
            var analytics = new SegmentAnalytics();

            try
            {
                // ❌ Client musi znać szczegóły konfiguracji każdego serwisu
                emailService.SetApiKey("SG.xxxx");
                
                smsService.Authenticate("ACxxxx", "authtoken");
                smsService.SetFromNumber("+1234567890");
                
                pushService.InitializeApp("my-project-123", "AIzaxxxx");
                
                logger.Connect("datadog-api-key", "datadog-app-key");
                logger.SetTags(new[] { "env:prod", "service:onboarding" });
                
                analytics.Initialize("segment-write-key");

                // ❌ Orchestration welcome email
                logger.LogInfo($"Sending welcome email to {email}");
                emailService.CreateMessage("noreply@company.com", email, "Welcome!", "Welcome to our platform!");
                emailService.Send();

                // ❌ Orchestration welcome SMS
                logger.LogInfo($"Sending welcome SMS to {phone}");
                smsService.SendSms(phone, "Welcome! Thanks for joining us.");

                // ❌ Orchestration push notification
                if (!string.IsNullOrEmpty(deviceToken))
                {
                    logger.LogInfo($"Sending welcome push notification");
                    pushService.SetTargetDevice(deviceToken);
                    pushService.CreateNotification(
                        "Welcome!",
                        "Thanks for joining us",
                        new Dictionary<string, string> { { "type", "welcome" } });
                    pushService.SendPush();
                }

                // ❌ Orchestration analytics tracking
                analytics.IdentifyUser(userId, new Dictionary<string, object>
                {
                    { "email", email },
                    { "phone", phone },
                    { "signup_date", DateTime.UtcNow }
                });
                analytics.TrackEvent("user_onboarded", new Dictionary<string, object>
                {
                    { "userId", userId },
                    { "source", "web" }
                });

                logger.LogInfo($"User {userId} onboarded successfully");
            }
            catch (Exception ex)
            {
                logger.LogError($"Onboarding failed for user {userId}", ex);
                throw;
            }

            // ❌ PROBLEMY:
            // - Client musi znać API wszystkich external services
            // - Złożona inicjalizacja i konfiguracja
            // - Duplikacja orchestration logic
            // - Trudne testowanie (5 external dependencies)
            // - Credentials hardcoded
            // - Zmiany w API wymagają zmian w wielu miejscach
            // - Brak retry logic, error handling
        }
    }
}
