using System;
using System.Collections.Generic;

namespace CleanCodeFundamentals.DRY.Bad2
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; }
    }

    // Problem: Ta sama logika formatowania cen jest powielona w wielu miejscach
    public class ProductDisplayService
    {
        public string FormatProductForWebsite(Product product)
        {
            // Kopia #1 logiki formatowania ceny
            string formattedPrice;
            if (product.Price >= 1000)
            {
                formattedPrice = $"{product.Price:N2} zł";
            }
            else if (product.Price >= 100)
            {
                formattedPrice = $"{product.Price:F2} zł";
            }
            else
            {
                formattedPrice = $"{product.Price:F2} zł (promocja!)";
            }

            return $"<div>{product.Name}: {formattedPrice}</div>";
        }
    }

    public class ProductEmailService
    {
        public string CreateProductEmailHtml(Product product)
        {
            // Kopia #2 tej samej logiki formatowania
            string formattedPrice;
            if (product.Price >= 1000)
            {
                formattedPrice = $"{product.Price:N2} zł";
            }
            else if (product.Price >= 100)
            {
                formattedPrice = $"{product.Price:F2} zł";
            }
            else
            {
                formattedPrice = $"{product.Price:F2} zł (promocja!)";
            }

            return $"<html><body>Produkt: {product.Name}, Cena: {formattedPrice}</body></html>";
        }
    }

    public class ProductInvoiceService
    {
        public string GenerateInvoiceLine(Product product, int quantity)
        {
            // Kopia #3 tej samej logiki - ryzyko niezgodności!
            string formattedPrice;
            if (product.Price >= 1000)
            {
                formattedPrice = $"{product.Price:N2} zł";
            }
            else if (product.Price >= 100)
            {
                formattedPrice = $"{product.Price:F2} zł";
            }
            else
            {
                formattedPrice = $"{product.Price:F2} zł (promocja!)";
            }

            var total = product.Price * quantity;
            return $"{product.Name} x {quantity} @ {formattedPrice} = {total:N2} zł";
        }
    }

    public class ProductReportService
    {
        public void PrintProductReport(List<Product> products)
        {
            foreach (var product in products)
            {
                // Kopia #4 tej samej logiki!
                string formattedPrice;
                if (product.Price >= 1000)
                {
                    formattedPrice = $"{product.Price:N2} zł";
                }
                else if (product.Price >= 100)
                {
                    formattedPrice = $"{product.Price:F2} zł";
                }
                else
                {
                    formattedPrice = $"{product.Price:F2} zł (promocja!)";
                }

                Console.WriteLine($"{product.Name}: {formattedPrice}");
            }
        }
    }
}
