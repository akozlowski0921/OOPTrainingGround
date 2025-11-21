using System;
using System.Collections.Generic;

namespace CleanCodeFundamentals.DRY.Good2
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; }
    }

    // Rozwiązanie: Wydzielona klasa do formatowania cen
    public class PriceFormatter
    {
        public string FormatPrice(decimal price)
        {
            if (price >= 1000)
            {
                return $"{price:N2} zł";
            }
            
            if (price >= 100)
            {
                return $"{price:F2} zł";
            }
            
            return $"{price:F2} zł (promocja!)";
        }
    }

    public class ProductDisplayService
    {
        private readonly PriceFormatter _priceFormatter;

        public ProductDisplayService(PriceFormatter priceFormatter)
        {
            _priceFormatter = priceFormatter;
        }

        public string FormatProductForWebsite(Product product)
        {
            var formattedPrice = _priceFormatter.FormatPrice(product.Price);
            return $"<div>{product.Name}: {formattedPrice}</div>";
        }
    }

    public class ProductEmailService
    {
        private readonly PriceFormatter _priceFormatter;

        public ProductEmailService(PriceFormatter priceFormatter)
        {
            _priceFormatter = priceFormatter;
        }

        public string CreateProductEmailHtml(Product product)
        {
            var formattedPrice = _priceFormatter.FormatPrice(product.Price);
            return $"<html><body>Produkt: {product.Name}, Cena: {formattedPrice}</body></html>";
        }
    }

    public class ProductInvoiceService
    {
        private readonly PriceFormatter _priceFormatter;

        public ProductInvoiceService(PriceFormatter priceFormatter)
        {
            _priceFormatter = priceFormatter;
        }

        public string GenerateInvoiceLine(Product product, int quantity)
        {
            var formattedPrice = _priceFormatter.FormatPrice(product.Price);
            var total = product.Price * quantity;
            return $"{product.Name} x {quantity} @ {formattedPrice} = {total:N2} zł";
        }
    }

    public class ProductReportService
    {
        private readonly PriceFormatter _priceFormatter;

        public ProductReportService(PriceFormatter priceFormatter)
        {
            _priceFormatter = priceFormatter;
        }

        public void PrintProductReport(List<Product> products)
        {
            foreach (var product in products)
            {
                var formattedPrice = _priceFormatter.FormatPrice(product.Price);
                Console.WriteLine($"{product.Name}: {formattedPrice}");
            }
        }
    }
}
