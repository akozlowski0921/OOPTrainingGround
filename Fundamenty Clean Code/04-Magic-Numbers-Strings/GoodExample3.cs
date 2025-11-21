using System;

namespace CleanCodeFundamentals.MagicNumbers.Good3
{
    public enum ShippingDestination
    {
        Domestic,
        EuropeanUnion,
        International
    }

    public enum ShippingPriority
    {
        Standard,
        Express,
        Overnight
    }

    public enum PackagingType
    {
        Minimal,
        Standard,
        Reinforced,
        BubbleWrap,
        DoubleBox
    }

    public static class ShippingRates
    {
        // Domestic rates (kg, PLN)
        public const decimal DomesticUpTo1Kg = 10m;
        public const decimal DomesticUpTo5Kg = 25m;
        public const decimal DomesticUpTo10Kg = 45m;
        public const decimal DomesticAdditionalPerKg = 3.5m;
        public const decimal DomesticWeightThreshold10Kg = 10m;

        // EU rates
        public const decimal EuUpTo1Kg = 25m;
        public const decimal EuUpTo5Kg = 60m;
        public const decimal EuAdditionalPerKg = 8m;
        public const decimal EuWeightThreshold5Kg = 5m;

        // International rates
        public const decimal InternationalUpTo1Kg = 40m;
        public const decimal InternationalAdditionalPerKg = 12m;

        // Priority multipliers
        public const decimal ExpressMultiplier = 1.5m;
        public const decimal OvernightMultiplier = 2.0m;
    }

    public static class DeliveryTimes
    {
        // Delivery times in days
        public const int OvernightDelivery = 1;
        
        public const int DomesticExpress = 2;
        public const int EuExpress = 3;
        public const int InternationalExpress = 5;
        
        public const int DomesticStandard = 5;
        public const int EuStandard = 10;
        public const int InternationalStandard = 20;
    }

    public static class PackageLimits
    {
        // Dimensions in cm
        public const decimal MaxLengthCm = 200m;
        public const decimal MaxWidthCm = 150m;
        public const decimal MaxHeightCm = 150m;
        
        // Volume in cm³ (0.5 m³)
        public const decimal MaxVolumeCm3 = 500000m;

        // Weight thresholds for packaging
        public const decimal MinimalPackagingWeightKg = 3m;
        public const decimal StandardPackagingWeightKg = 10m;
        public const decimal FragilePackagingWeightKg = 5m;
    }

    public class ShippingCalculator
    {
        public decimal CalculateShippingCost(decimal weight, ShippingDestination destination, ShippingPriority priority)
        {
            decimal baseCost = CalculateBaseCost(weight, destination);
            return ApplyPriorityMultiplier(baseCost, priority);
        }

        private decimal CalculateBaseCost(decimal weight, ShippingDestination destination)
        {
            return destination switch
            {
                ShippingDestination.Domestic => CalculateDomesticCost(weight),
                ShippingDestination.EuropeanUnion => CalculateEuCost(weight),
                ShippingDestination.International => CalculateInternationalCost(weight),
                _ => throw new ArgumentException($"Unknown destination: {destination}")
            };
        }

        private decimal CalculateDomesticCost(decimal weight)
        {
            if (weight <= 1) return ShippingRates.DomesticUpTo1Kg;
            if (weight <= 5) return ShippingRates.DomesticUpTo5Kg;
            if (weight <= 10) return ShippingRates.DomesticUpTo10Kg;
            
            return ShippingRates.DomesticUpTo10Kg + 
                   (weight - ShippingRates.DomesticWeightThreshold10Kg) * ShippingRates.DomesticAdditionalPerKg;
        }

        private decimal CalculateEuCost(decimal weight)
        {
            if (weight <= 1) return ShippingRates.EuUpTo1Kg;
            if (weight <= 5) return ShippingRates.EuUpTo5Kg;
            
            return ShippingRates.EuUpTo5Kg + 
                   (weight - ShippingRates.EuWeightThreshold5Kg) * ShippingRates.EuAdditionalPerKg;
        }

        private decimal CalculateInternationalCost(decimal weight)
        {
            if (weight <= 1) return ShippingRates.InternationalUpTo1Kg;
            
            return ShippingRates.InternationalUpTo1Kg + 
                   (weight - 1) * ShippingRates.InternationalAdditionalPerKg;
        }

        private decimal ApplyPriorityMultiplier(decimal baseCost, ShippingPriority priority)
        {
            return priority switch
            {
                ShippingPriority.Express => baseCost * ShippingRates.ExpressMultiplier,
                ShippingPriority.Overnight => baseCost * ShippingRates.OvernightMultiplier,
                ShippingPriority.Standard => baseCost,
                _ => baseCost
            };
        }

        public int EstimateDeliveryDays(ShippingDestination destination, ShippingPriority priority)
        {
            if (priority == ShippingPriority.Overnight)
            {
                return DeliveryTimes.OvernightDelivery;
            }

            if (priority == ShippingPriority.Express)
            {
                return destination switch
                {
                    ShippingDestination.Domestic => DeliveryTimes.DomesticExpress,
                    ShippingDestination.EuropeanUnion => DeliveryTimes.EuExpress,
                    ShippingDestination.International => DeliveryTimes.InternationalExpress,
                    _ => DeliveryTimes.DomesticStandard
                };
            }

            return destination switch
            {
                ShippingDestination.Domestic => DeliveryTimes.DomesticStandard,
                ShippingDestination.EuropeanUnion => DeliveryTimes.EuStandard,
                ShippingDestination.International => DeliveryTimes.InternationalStandard,
                _ => DeliveryTimes.DomesticStandard
            };
        }

        public bool CanShipOversized(decimal lengthCm, decimal widthCm, decimal heightCm)
        {
            if (lengthCm > PackageLimits.MaxLengthCm || 
                widthCm > PackageLimits.MaxWidthCm || 
                heightCm > PackageLimits.MaxHeightCm)
            {
                return false;
            }

            decimal volumeCm3 = lengthCm * widthCm * heightCm;
            if (volumeCm3 > PackageLimits.MaxVolumeCm3)
            {
                return false;
            }

            return true;
        }

        public PackagingType GetPackagingRequirement(decimal weightKg, bool fragile)
        {
            if (fragile)
            {
                return weightKg > PackageLimits.FragilePackagingWeightKg 
                    ? PackagingType.DoubleBox 
                    : PackagingType.BubbleWrap;
            }

            if (weightKg > PackageLimits.StandardPackagingWeightKg)
            {
                return PackagingType.Reinforced;
            }
            
            if (weightKg > PackageLimits.MinimalPackagingWeightKg)
            {
                return PackagingType.Standard;
            }
            
            return PackagingType.Minimal;
        }
    }
}
