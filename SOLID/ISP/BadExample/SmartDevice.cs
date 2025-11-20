using System;

namespace SOLID.ISP.BadExample
{
    /// <summary>
    /// Naruszenie ISP: Zbyt duży interfejs wymuszający niepotrzebne implementacje
    /// Wszystkie urządzenia muszą implementować wszystkie metody, nawet jeśli nie mają sensu
    /// </summary>
    public interface ISmartDevice
    {
        void Print(string document);
        void Scan(string document);
        void Fax(string document);
        void Copy(string document);
        void SendEmail(string to, string subject, string body);
        void MakeCall(string number);
        void BrowseInternet(string url);
    }

    /// <summary>
    /// Wielofunkcyjna drukarka - OK, wykorzystuje większość metod
    /// </summary>
    public class MultiFunctionPrinter : ISmartDevice
    {
        public void Print(string document)
        {
            Console.WriteLine($"Drukowanie: {document}");
        }

        public void Scan(string document)
        {
            Console.WriteLine($"Skanowanie: {document}");
        }

        public void Fax(string document)
        {
            Console.WriteLine($"Faksowanie: {document}");
        }

        public void Copy(string document)
        {
            Console.WriteLine($"Kopiowanie: {document}");
        }

        public void SendEmail(string to, string subject, string body)
        {
            // Drukarka nie powinna wysyłać emaili!
            throw new NotImplementedException("Drukarka nie może wysyłać emaili");
        }

        public void MakeCall(string number)
        {
            // Drukarka nie może dzwonić!
            throw new NotImplementedException("Drukarka nie może wykonywać połączeń");
        }

        public void BrowseInternet(string url)
        {
            // Drukarka nie ma przeglądarki!
            throw new NotImplementedException("Drukarka nie może przeglądać internetu");
        }
    }

    /// <summary>
    /// Prosta drukarka - większość metod nie ma sensu
    /// </summary>
    public class SimplePrinter : ISmartDevice
    {
        public void Print(string document)
        {
            Console.WriteLine($"Drukowanie: {document}");
        }

        public void Scan(string document)
        {
            throw new NotImplementedException("Ta drukarka nie ma skanera");
        }

        public void Fax(string document)
        {
            throw new NotImplementedException("Ta drukarka nie ma faksu");
        }

        public void Copy(string document)
        {
            throw new NotImplementedException("Ta drukarka nie może kopiować");
        }

        public void SendEmail(string to, string subject, string body)
        {
            throw new NotImplementedException("Drukarka nie może wysyłać emaili");
        }

        public void MakeCall(string number)
        {
            throw new NotImplementedException("Drukarka nie może dzwonić");
        }

        public void BrowseInternet(string url)
        {
            throw new NotImplementedException("Drukarka nie ma przeglądarki");
        }
    }

    /// <summary>
    /// Smartfon - część metod ma sens, część nie
    /// </summary>
    public class Smartphone : ISmartDevice
    {
        public void Print(string document)
        {
            // Smartfon nie drukuje bezpośrednio
            throw new NotImplementedException("Smartfon nie jest drukarką");
        }

        public void Scan(string document)
        {
            Console.WriteLine($"Skanowanie przez aparat: {document}");
        }

        public void Fax(string document)
        {
            throw new NotImplementedException("Smartfon nie wysyła faksów");
        }

        public void Copy(string document)
        {
            throw new NotImplementedException("Smartfon nie kopiuje dokumentów");
        }

        public void SendEmail(string to, string subject, string body)
        {
            Console.WriteLine($"Wysyłanie emaila do {to}: {subject}");
        }

        public void MakeCall(string number)
        {
            Console.WriteLine($"Dzwonienie na numer: {number}");
        }

        public void BrowseInternet(string url)
        {
            Console.WriteLine($"Otwieranie strony: {url}");
        }
    }

    // Program demonstracyjny
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("=== Test urządzeń ===\n");

            ISmartDevice[] devices = {
                new SimplePrinter(),
                new MultiFunctionPrinter(),
                new Smartphone()
            };

            Console.WriteLine("--- Próba użycia wszystkich funkcji ---\n");

            foreach (var device in devices)
            {
                Console.WriteLine($"Testowanie: {device.GetType().Name}");
                
                try
                {
                    device.Print("Dokument.pdf");
                }
                catch (NotImplementedException ex)
                {
                    Console.WriteLine($"  BŁĄD Print: {ex.Message}");
                }

                try
                {
                    device.SendEmail("test@example.com", "Test", "Hello");
                }
                catch (NotImplementedException ex)
                {
                    Console.WriteLine($"  BŁĄD SendEmail: {ex.Message}");
                }

                Console.WriteLine();
            }
        }
    }
}
