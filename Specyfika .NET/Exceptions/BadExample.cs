using System;
using System.IO;

namespace SpecyfikaDotNet.ExceptionHandling
{
    // ❌ BAD: Niewłaściwe obsługiwanie wyjątków
    public class BadFileProcessor
    {
        public string ReadFile(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                // BŁĄD 1: "Swallowing" wyjątku - ukrywanie problemu
                Console.WriteLine("Błąd odczytu pliku");
                return string.Empty; // Aplikacja nie wie, że wystąpił błąd
            }
        }

        public void ProcessData(string data)
        {
            try
            {
                if (string.IsNullOrEmpty(data))
                    throw new Exception("Dane są puste");
                
                // Przetwarzanie danych...
            }
            catch (Exception ex)
            {
                // BŁĄD 2: throw ex; - traci oryginalny StackTrace
                throw ex; // Utrata informacji o miejscu wystąpienia błędu
            }
        }

        public decimal CalculatePrice(int quantity, decimal unitPrice)
        {
            try
            {
                if (quantity <= 0)
                    throw new Exception("Invalid quantity"); // BŁĄD 3: Generyczny wyjątek
                
                return quantity * unitPrice;
            }
            catch
            {
                // BŁĄD 4: Pusta klauzula catch - najgorsze rozwiązanie
                return 0;
            }
        }

        public void SaveToDatabase(string data)
        {
            try
            {
                // Zapis do bazy...
                throw new InvalidOperationException("Database connection failed");
            }
            catch (InvalidOperationException ex)
            {
                // BŁĄD 5: Łapanie i ponowne rzucanie bez dodatkowego kontekstu
                throw ex;
            }
        }
    }
}
