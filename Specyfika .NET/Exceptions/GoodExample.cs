using System;
using System.IO;

namespace SpecyfikaDotNet.ExceptionHandling
{
    // ✅ GOOD: Prawidłowa obsługa wyjątków z zachowaniem StackTrace
    public class GoodFileProcessor
    {
        public string ReadFile(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (FileNotFoundException ex)
            {
                // Specyficzny wyjątek i logowanie z pełnym kontekstem
                Console.WriteLine($"Plik nie został znaleziony: {path}. Szczegóły: {ex}");
                throw; // Zachowuje oryginalny StackTrace
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Brak uprawnień do pliku: {path}");
                throw; // Propagacja wyjątku wyżej
            }
        }

        public void ProcessData(string data)
        {
            try
            {
                if (string.IsNullOrEmpty(data))
                    throw new ArgumentNullException(nameof(data), "Dane nie mogą być puste");
                
                // Przetwarzanie danych...
            }
            catch (ArgumentNullException)
            {
                // Jeśli musimy obsłużyć, używamy throw; bez ex
                throw; // Zachowuje pełny StackTrace
            }
        }

        public decimal CalculatePrice(int quantity, decimal unitPrice)
        {
            // Walidacja argumentów - rzucamy specyficzne wyjątki
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Ilość musi być większa od zera");
            
            if (unitPrice < 0)
                throw new ArgumentOutOfRangeException(nameof(unitPrice), "Cena nie może być ujemna");
            
            return quantity * unitPrice;
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
                // Opakowujemy w custom exception z dodatkowym kontekstem
                throw new DatabaseException("Nie udało się zapisać danych do bazy", ex);
            }
        }
    }

    // Custom exception z dodatkowym kontekstem
    public class DatabaseException : Exception
    {
        public DatabaseException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
