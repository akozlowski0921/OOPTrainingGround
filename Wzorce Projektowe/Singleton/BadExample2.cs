using System;
using System.Collections.Generic;

namespace DesignPatterns.Singleton.Bad2
{
    // ❌ BAD: Multiple instances causing inconsistency
    public class ConfigurationManager
    {
        private Dictionary<string, string> _settings;

        public ConfigurationManager()
        {
            _settings = new Dictionary<string, string>
            {
                { "ApiUrl", "https://api.example.com" },
                { "Timeout", "30" }
            };
        }

        public string GetSetting(string key)
        {
            return _settings.ContainsKey(key) ? _settings[key] : null;
        }

        public void SetSetting(string key, string value)
        {
            _settings[key] = value;
        }
    }

    // Problem: Each instance has its own state
    public class Example
    {
        public void Run()
        {
            var config1 = new ConfigurationManager();
            var config2 = new ConfigurationManager();

            config1.SetSetting("ApiUrl", "https://newapi.example.com");
            
            // Problem: config2 still has old value!
            Console.WriteLine(config2.GetSetting("ApiUrl")); // https://api.example.com
        }
    }
}
