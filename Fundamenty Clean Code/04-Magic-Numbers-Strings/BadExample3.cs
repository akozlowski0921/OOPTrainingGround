using System;

namespace CleanCodeFundamentals.MagicNumbers.Bad3
{
    public class ShippingCalculator
    {
        public decimal CalculateShippingCost(decimal weight, string destination, string priority)
        {
            decimal baseCost = 0;

            // Magic strings dla destynacji
            if (destination == "domestic")
            {
                // Magic numbers dla wagi
                if (weight <= 1)
                {
                    baseCost = 10;
                }
                else if (weight <= 5)
                {
                    baseCost = 25;
                }
                else if (weight <= 10)
                {
                    baseCost = 45;
                }
                else
                {
                    baseCost = 45 + (weight - 10) * 3.5m;
                }
            }
            else if (destination == "eu")
            {
                if (weight <= 1)
                {
                    baseCost = 25;
                }
                else if (weight <= 5)
                {
                    baseCost = 60;
                }
                else
                {
                    baseCost = 60 + (weight - 5) * 8m;
                }
            }
            else if (destination == "world")
            {
                if (weight <= 1)
                {
                    baseCost = 40;
                }
                else
                {
                    baseCost = 40 + (weight - 1) * 12m;
                }
            }

            // Magic strings dla priorytetu
            if (priority == "express")
            {
                baseCost = baseCost * 1.5m;
            }
            else if (priority == "overnight")
            {
                baseCost = baseCost * 2.0m;
            }

            return baseCost;
        }

        public int EstimateDeliveryDays(string destination, string priority)
        {
            // Więcej magic numbers
            if (priority == "overnight")
            {
                return 1;
            }
            else if (priority == "express")
            {
                if (destination == "domestic")
                {
                    return 2;
                }
                else if (destination == "eu")
                {
                    return 3;
                }
                else
                {
                    return 5;
                }
            }
            else // standard
            {
                if (destination == "domestic")
                {
                    return 5;
                }
                else if (destination == "eu")
                {
                    return 10;
                }
                else
                {
                    return 20;
                }
            }
        }

        public bool CanShipOversized(decimal length, decimal width, decimal height)
        {
            // Magic numbers dla wymiarów
            if (length > 200 || width > 150 || height > 150)
            {
                return false;
            }

            // Objętość 0.5 m³
            if (length * width * height > 500000)
            {
                return false;
            }

            return true;
        }

        public string GetPackagingRequirement(decimal weight, bool fragile)
        {
            // Magic strings i numbers
            if (fragile)
            {
                if (weight > 5)
                {
                    return "double-box";
                }
                else
                {
                    return "bubble-wrap";
                }
            }
            else
            {
                if (weight > 10)
                {
                    return "reinforced";
                }
                else if (weight > 3)
                {
                    return "standard";
                }
                else
                {
                    return "minimal";
                }
            }
        }
    }
}
