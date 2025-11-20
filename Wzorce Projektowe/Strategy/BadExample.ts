// ❌ BAD EXAMPLE: if/else dla każdego przewoźnika

class ShippingService {
    calculateShippingCost(carrierType: string, weight: number, distance: number): number {
        if (carrierType === "DHL") {
            const baseCost = 10;
            const weightCost = weight * 0.5;
            const distanceCost = distance * 0.1;
            return baseCost + weightCost + distanceCost;
        } else if (carrierType === "UPS") {
            const baseCost = 12;
            const weightCost = weight * 0.45;
            const distanceCost = distance * 0.12;
            return baseCost + weightCost + distanceCost;
        } else if (carrierType === "FedEx") {
            const baseCost = 15;
            const weightCost = weight * 0.4;
            const distanceCost = distance * 0.15;
            return baseCost + weightCost + distanceCost;
        } else if (carrierType === "InPost") {
            const baseCost = 8;
            const weightCost = weight * 0.3;
            return baseCost + weightCost; // InPost nie uwzględnia dystansu
        } else {
            throw new Error("Unknown carrier type");
        }
    }

    getEstimatedDeliveryDays(carrierType: string, distance: number): number {
        if (carrierType === "DHL") {
            return distance > 500 ? 3 : 1;
        } else if (carrierType === "UPS") {
            return distance > 500 ? 4 : 2;
        } else if (carrierType === "FedEx") {
            return distance > 500 ? 2 : 1;
        } else if (carrierType === "InPost") {
            return 5;
        } else {
            throw new Error("Unknown carrier type");
        }
    }
}

// Użycie:
const shippingService = new ShippingService();
const cost1 = shippingService.calculateShippingCost("DHL", 10, 200);
const days1 = shippingService.getEstimatedDeliveryDays("DHL", 200);

console.log(`DHL: Cost=${cost1}, Days=${days1}`);

// Problemy:
// 1. Dodanie nowego przewoźnika wymaga modyfikacji istniejących metod (Open/Closed Principle violation)
// 2. Niemożliwość łatwego testowania pojedynczego przewoźnika
// 3. Trudność w utrzymaniu spójności logiki dla przewoźnika (rozrzucona po metodach)
// 4. Rozrastający się kod w metodach (God Object antipattern)
