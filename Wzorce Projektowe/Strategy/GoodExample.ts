// ✅ GOOD EXAMPLE: Strategy Pattern z interfejsem

interface IShippingStrategy {
    calculateCost(weight: number, distance: number): number;
    getEstimatedDeliveryDays(distance: number): number;
}

class DHLStrategy implements IShippingStrategy {
    calculateCost(weight: number, distance: number): number {
        const baseCost = 10;
        const weightCost = weight * 0.5;
        const distanceCost = distance * 0.1;
        return baseCost + weightCost + distanceCost;
    }

    getEstimatedDeliveryDays(distance: number): number {
        return distance > 500 ? 3 : 1;
    }
}

class UPSStrategy implements IShippingStrategy {
    calculateCost(weight: number, distance: number): number {
        const baseCost = 12;
        const weightCost = weight * 0.45;
        const distanceCost = distance * 0.12;
        return baseCost + weightCost + distanceCost;
    }

    getEstimatedDeliveryDays(distance: number): number {
        return distance > 500 ? 4 : 2;
    }
}

class FedExStrategy implements IShippingStrategy {
    calculateCost(weight: number, distance: number): number {
        const baseCost = 15;
        const weightCost = weight * 0.4;
        const distanceCost = distance * 0.15;
        return baseCost + weightCost + distanceCost;
    }

    getEstimatedDeliveryDays(distance: number): number {
        return distance > 500 ? 2 : 1;
    }
}

class InPostStrategy implements IShippingStrategy {
    calculateCost(weight: number, distance: number): number {
        const baseCost = 8;
        const weightCost = weight * 0.3;
        return baseCost + weightCost;
    }

    getEstimatedDeliveryDays(distance: number): number {
        return 5;
    }
}

class ShippingService {
    private strategy: IShippingStrategy;

    constructor(strategy: IShippingStrategy) {
        this.strategy = strategy;
    }

    setStrategy(strategy: IShippingStrategy): void {
        this.strategy = strategy;
    }

    calculateShippingCost(weight: number, distance: number): number {
        return this.strategy.calculateCost(weight, distance);
    }

    getEstimatedDeliveryDays(distance: number): number {
        return this.strategy.getEstimatedDeliveryDays(distance);
    }
}

// Użycie:
const dhlService = new ShippingService(new DHLStrategy());
const cost1 = dhlService.calculateShippingCost(10, 200);
const days1 = dhlService.getEstimatedDeliveryDays(200);
console.log(`DHL: Cost=${cost1}, Days=${days1}`);

// Dynamiczna zmiana strategii:
const shippingService = new ShippingService(new DHLStrategy());
console.log("DHL:", shippingService.calculateShippingCost(10, 200));

shippingService.setStrategy(new UPSStrategy());
console.log("UPS:", shippingService.calculateShippingCost(10, 200));

// Korzyści:
// 1. Łatwe dodawanie nowych przewoźników bez modyfikacji istniejącego kodu
// 2. Każda strategia może być testowana niezależnie
// 3. Logika każdego przewoźnika jest enkapsulowana w jednej klasie
// 4. Możliwość dynamicznej zmiany strategii w runtime
