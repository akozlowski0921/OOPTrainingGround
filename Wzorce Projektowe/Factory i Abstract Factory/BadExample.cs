using System;

namespace DesignPatterns.Factory
{
    // ❌ BAD: Tworzenie obiektów bezpośrednio w kodzie

    public enum PaymentType { CreditCard, PayPal, BankTransfer }

    // BŁĄD 1: Switch/if statements dla tworzenia obiektów
    public class BadPaymentProcessor
    {
        public void ProcessPayment(PaymentType type, decimal amount)
        {
            // ❌ Tworzenie obiektów z switch - trudne w rozszerzaniu
            switch (type)
            {
                case PaymentType.CreditCard:
                    var cc = new CreditCardPayment();
                    cc.Process(amount);
                    break;
                case PaymentType.PayPal:
                    var pp = new PayPalPayment();
                    pp.Process(amount);
                    break;
                case PaymentType.BankTransfer:
                    var bt = new BankTransferPayment();
                    bt.Process(amount);
                    break;
            }
            // ❌ Dodanie nowego typu płatności wymaga zmiany tej metody (OCP violation)
        }
    }

    // BŁĄD 2: Bezpośrednie użycie new w business logic
    public class BadOrderService
    {
        public void CreateOrder(string customerName, decimal amount)
        {
            // ❌ Tight coupling do konkretnej klasy
            var notification = new EmailNotification();
            notification.Send($"Order for {customerName}: ${amount}");
            
            // ❌ Trudne w testowaniu - nie można podmienić implementacji
        }
    }

    // Pomocnicze klasy
    public class CreditCardPayment
    {
        public void Process(decimal amount) => Console.WriteLine($"CC: ${amount}");
    }

    public class PayPalPayment
    {
        public void Process(decimal amount) => Console.WriteLine($"PayPal: ${amount}");
    }

    public class BankTransferPayment
    {
        public void Process(decimal amount) => Console.WriteLine($"Bank: ${amount}");
    }

    public class EmailNotification
    {
        public void Send(string message) => Console.WriteLine($"Email: {message}");
    }
}
