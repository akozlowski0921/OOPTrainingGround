using System;

namespace SOLID.ISP.GoodExample
{
    /// <summary>
    /// Prosta drukarka - implementuje TYLKO to, co potrafi
    /// </summary>
    public class SimplePrinter : IPrinter
    {
        public void Print(string document)
        {
            Console.WriteLine($"[SimplePrinter] Drukowanie: {document}");
        }
    }

    /// <summary>
    /// Wielofunkcyjna drukarka - implementuje wiele interfejsów
    /// </summary>
    public class MultiFunctionPrinter : IPrinter, IScanner, IFax, ICopier
    {
        public void Print(string document)
        {
            Console.WriteLine($"[MultiFunctionPrinter] Drukowanie: {document}");
        }

        public void Scan(string document)
        {
            Console.WriteLine($"[MultiFunctionPrinter] Skanowanie: {document}");
        }

        public void Fax(string document)
        {
            Console.WriteLine($"[MultiFunctionPrinter] Faksowanie: {document}");
        }

        public void Copy(string document)
        {
            Console.WriteLine($"[MultiFunctionPrinter] Kopiowanie: {document}");
        }
    }

    /// <summary>
    /// Smartfon - implementuje tylko sensowne dla niego interfejsy
    /// </summary>
    public class Smartphone : IScanner, IEmailSender, IPhone, IWebBrowser
    {
        public void Scan(string document)
        {
            Console.WriteLine($"[Smartphone] Skanowanie przez aparat: {document}");
        }

        public void SendEmail(string to, string subject, string body)
        {
            Console.WriteLine($"[Smartphone] Wysyłanie emaila do {to}: {subject}");
        }

        public void MakeCall(string number)
        {
            Console.WriteLine($"[Smartphone] Dzwonienie na numer: {number}");
        }

        public void BrowseInternet(string url)
        {
            Console.WriteLine($"[Smartphone] Otwieranie strony: {url}");
        }
    }

    /// <summary>
    /// Scanner fotograficzny - tylko skanuje
    /// </summary>
    public class PhotoScanner : IScanner
    {
        public void Scan(string document)
        {
            Console.WriteLine($"[PhotoScanner] Skanowanie wysokiej jakości: {document}");
        }
    }

    /// <summary>
    /// Tradycyjny faks - tylko faksuje
    /// </summary>
    public class FaxMachine : IFax
    {
        public void Fax(string document)
        {
            Console.WriteLine($"[FaxMachine] Faksowanie: {document}");
        }
    }

    // Program demonstracyjny
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("=== Test urządzeń (zgodny z ISP) ===\n");

            // Każde urządzenie używa tylko interfejsów, które implementuje
            Console.WriteLine("--- Drukarki ---");
            IPrinter simplePrinter = new SimplePrinter();
            simplePrinter.Print("Dokument1.pdf");

            IPrinter multiFunctionPrinter = new MultiFunctionPrinter();
            multiFunctionPrinter.Print("Dokument2.pdf");

            Console.WriteLine("\n--- Skanery ---");
            IScanner[] scanners = {
                new MultiFunctionPrinter(),
                new Smartphone(),
                new PhotoScanner()
            };

            foreach (var scanner in scanners)
            {
                scanner.Scan("Zdjęcie.jpg");
            }

            Console.WriteLine("\n--- Urządzenia z emailem ---");
            IEmailSender emailDevice = new Smartphone();
            emailDevice.SendEmail("jan@example.com", "Test", "Wiadomość testowa");

            Console.WriteLine("\n--- Telefony ---");
            IPhone phone = new Smartphone();
            phone.MakeCall("+48 123 456 789");

            Console.WriteLine("\n--- Kopiowanie (tylko wielofunkcyjna) ---");
            var mfp = new MultiFunctionPrinter();
            (mfp as ICopier)?.Copy("Raport.docx");

            Console.WriteLine("\n--- Demonstracja składania funkcji ---");
            ProcessPrintJob(new SimplePrinter(), "Test.pdf");
            ProcessPrintJob(new MultiFunctionPrinter(), "Test2.pdf");
            // ProcessPrintJob(new Smartphone(), "Test3.pdf"); // Błąd kompilacji - dobrze!

            Console.WriteLine("\n--- Pełny workflow na MFP ---");
            WorkflowScanAndPrint(new MultiFunctionPrinter(), "Dokument.pdf");
        }

        // Funkcja przyjmuje TYLKO drukarki
        public static void ProcessPrintJob(IPrinter printer, string document)
        {
            Console.WriteLine($"Przetwarzanie zadania drukowania...");
            printer.Print(document);
        }

        // Funkcja wymaga urządzenia, które potrafi skanować I drukować
        public static void WorkflowScanAndPrint(object device, string document)
        {
            if (device is IScanner scanner && device is IPrinter printer)
            {
                scanner.Scan(document);
                printer.Print(document);
            }
            else
            {
                Console.WriteLine("Urządzenie nie wspiera pełnego workflow");
            }
        }
    }
}
