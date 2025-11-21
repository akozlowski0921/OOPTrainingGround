using System;
using System.Collections.Generic;

namespace DesignPatterns.Singleton.Good2
{
    // ✅ GOOD: Singleton ensures single instance
    public class ConfigurationManager
    {
        private static ConfigurationManager _instance;
        private static readonly object _lock = new object();
        private Dictionary<string, string> _settings;

        private ConfigurationManager()
        {
            _settings = new Dictionary<string, string>
            {
                { "ApiUrl", "https://api.example.com" },
                { "Timeout", "30" }
            };
        }

        public static ConfigurationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConfigurationManager();
                        }
                    }
                }
                return _instance;
            }
        }

        public string GetSetting(string key)
        {
            return _settings.ContainsKey(key) ? _settings[key] : null;
        }

        public void SetSetting(string key, string value)
        {
            lock (_lock)
            {
                _settings[key] = value;
            }
        }
    }

    // ✅ All code uses same instance
    public class Example
    {
        public void Run()
        {
            var config1 = ConfigurationManager.Instance;
            var config2 = ConfigurationManager.Instance;

            config1.SetSetting("ApiUrl", "https://newapi.example.com");
            
            // Both see the same value
            Console.WriteLine(config2.GetSetting("ApiUrl")); // https://newapi.example.com
            Console.WriteLine(ReferenceEquals(config1, config2)); // True
        }
    }
}
