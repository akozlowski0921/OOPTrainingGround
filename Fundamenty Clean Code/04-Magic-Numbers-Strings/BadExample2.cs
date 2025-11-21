using System;

namespace CleanCodeFundamentals.MagicNumbers.Bad2
{
    public class Invoice
    {
        public decimal Amount { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
    }

    // Problem: Magic numbers w obliczeniach podatkowych i karach
    public class InvoiceCalculator
    {
        public decimal CalculateTotalWithTax(Invoice invoice)
        {
            // Co oznacza 0.23? Jaka stawka VAT?
            decimal tax = invoice.Amount * 0.23m;
            return invoice.Amount + tax;
        }

        public decimal CalculateLateFee(Invoice invoice)
        {
            // Co oznacza 30? Dni? Godziny?
            var daysPastDue = (DateTime.Now - invoice.DueDate).Days;
            
            // Co to za liczby 7, 30, 90?
            if (daysPastDue > 90)
            {
                // 0.15 - co to za procent?
                return invoice.Amount * 0.15m;
            }
            else if (daysPastDue > 30)
            {
                return invoice.Amount * 0.10m;
            }
            else if (daysPastDue > 7)
            {
                return invoice.Amount * 0.05m;
            }

            return 0;
        }

        public string GetInvoiceType(decimal amount)
        {
            // Magic strings i numbers
            if (amount > 10000)
            {
                return "enterprise"; // Co jeśli ktoś napisze "Enterprise"?
            }
            else if (amount > 1000)
            {
                return "business";
            }
            else if (amount > 100)
            {
                return "standard";
            }
            else
            {
                return "small";
            }
        }

        public bool RequiresApproval(Invoice invoice)
        {
            // Co oznacza "draft", "pending"? Czy są inne statusy?
            if (invoice.Status == "draft")
            {
                return false;
            }

            // 5000 - co to za próg?
            if (invoice.Amount > 5000)
            {
                return true;
            }

            // 45 - co to za liczba?
            var age = (DateTime.Now - invoice.IssueDate).Days;
            if (age > 45)
            {
                return true;
            }

            return false;
        }

        public decimal CalculateEarlyPaymentDiscount(Invoice invoice)
        {
            var daysUntilDue = (invoice.DueDate - DateTime.Now).Days;

            // Co oznaczają 14, 7, 3 i procenty?
            if (daysUntilDue > 14)
            {
                return invoice.Amount * 0.03m;
            }
            else if (daysUntilDue > 7)
            {
                return invoice.Amount * 0.02m;
            }
            else if (daysUntilDue > 3)
            {
                return invoice.Amount * 0.01m;
            }

            return 0;
        }
    }
}
