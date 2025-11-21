using System;

namespace CleanCodeFundamentals.MagicNumbers.Good2
{
    public enum InvoiceStatus
    {
        Draft,
        Pending,
        Paid,
        Overdue,
        Cancelled
    }

    public enum InvoiceType
    {
        Small,
        Standard,
        Business,
        Enterprise
    }

    public static class TaxRates
    {
        public const decimal StandardVatRate = 0.23m; // 23% VAT
    }

    public static class PaymentPolicies
    {
        // Late fee thresholds (in days)
        public const int GracePeriodDays = 7;
        public const int StandardLatePeriodDays = 30;
        public const int SevereLatePeriodDays = 90;

        // Late fee rates
        public const decimal GracePeriodLateFeeRate = 0.05m;   // 5%
        public const decimal StandardLateFeeRate = 0.10m;      // 10%
        public const decimal SevereLateFeeRate = 0.15m;        // 15%

        // Early payment discount thresholds (days before due date)
        public const int EarlyPaymentPremiumDays = 14;
        public const int EarlyPaymentStandardDays = 7;
        public const int EarlyPaymentBasicDays = 3;

        // Early payment discount rates
        public const decimal EarlyPaymentPremiumRate = 0.03m;  // 3%
        public const decimal EarlyPaymentStandardRate = 0.02m; // 2%
        public const decimal EarlyPaymentBasicRate = 0.01m;    // 1%

        // Approval thresholds
        public const decimal ApprovalAmountThreshold = 5000m;
        public const int ApprovalAgeDaysThreshold = 45;
    }

    public static class InvoiceClassification
    {
        public const decimal SmallInvoiceThreshold = 100m;
        public const decimal StandardInvoiceThreshold = 1000m;
        public const decimal BusinessInvoiceThreshold = 10000m;
    }

    public class Invoice
    {
        public decimal Amount { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public InvoiceStatus Status { get; set; }
    }

    public class InvoiceCalculator
    {
        public decimal CalculateTotalWithTax(Invoice invoice)
        {
            decimal tax = invoice.Amount * TaxRates.StandardVatRate;
            return invoice.Amount + tax;
        }

        public decimal CalculateLateFee(Invoice invoice)
        {
            var daysPastDue = (DateTime.Now - invoice.DueDate).Days;
            
            if (daysPastDue > PaymentPolicies.SevereLatePeriodDays)
            {
                return invoice.Amount * PaymentPolicies.SevereLateFeeRate;
            }
            else if (daysPastDue > PaymentPolicies.StandardLatePeriodDays)
            {
                return invoice.Amount * PaymentPolicies.StandardLateFeeRate;
            }
            else if (daysPastDue > PaymentPolicies.GracePeriodDays)
            {
                return invoice.Amount * PaymentPolicies.GracePeriodLateFeeRate;
            }

            return 0;
        }

        public InvoiceType GetInvoiceType(decimal amount)
        {
            if (amount > InvoiceClassification.BusinessInvoiceThreshold)
            {
                return InvoiceType.Enterprise;
            }
            else if (amount > InvoiceClassification.StandardInvoiceThreshold)
            {
                return InvoiceType.Business;
            }
            else if (amount > InvoiceClassification.SmallInvoiceThreshold)
            {
                return InvoiceType.Standard;
            }
            else
            {
                return InvoiceType.Small;
            }
        }

        public bool RequiresApproval(Invoice invoice)
        {
            if (invoice.Status == InvoiceStatus.Draft)
            {
                return false;
            }

            if (invoice.Amount > PaymentPolicies.ApprovalAmountThreshold)
            {
                return true;
            }

            var age = (DateTime.Now - invoice.IssueDate).Days;
            if (age > PaymentPolicies.ApprovalAgeDaysThreshold)
            {
                return true;
            }

            return false;
        }

        public decimal CalculateEarlyPaymentDiscount(Invoice invoice)
        {
            var daysUntilDue = (invoice.DueDate - DateTime.Now).Days;

            if (daysUntilDue > PaymentPolicies.EarlyPaymentPremiumDays)
            {
                return invoice.Amount * PaymentPolicies.EarlyPaymentPremiumRate;
            }
            else if (daysUntilDue > PaymentPolicies.EarlyPaymentStandardDays)
            {
                return invoice.Amount * PaymentPolicies.EarlyPaymentStandardRate;
            }
            else if (daysUntilDue > PaymentPolicies.EarlyPaymentBasicDays)
            {
                return invoice.Amount * PaymentPolicies.EarlyPaymentBasicRate;
            }

            return 0;
        }
    }
}
