// Rozwiązanie: Prosta, czytelna implementacja z osobnymi metodami

interface Customer {
  id: number;
  name: string;
  loyaltyPoints: number;
  isPremium: boolean;
  yearsSinceRegistration: number;
}

interface Product {
  id: number;
  price: number;
  category: string;
}

export class DiscountCalculator {
  private readonly MAX_DISCOUNT = 0.5;

  calculateDiscount(customer: Customer, product: Product, quantity: number): number {
    const membershipDiscount = this.getMembershipDiscount(customer.yearsSinceRegistration);
    const loyaltyDiscount = this.getLoyaltyDiscount(customer.loyaltyPoints);
    const premiumDiscount = customer.isPremium ? 0.08 : 0;
    const quantityDiscount = this.getQuantityDiscount(quantity);
    const categoryDiscount = this.getCategoryDiscount(product, customer.isPremium);

    // Proste sumowanie rabatów
    const totalDiscount = membershipDiscount + loyaltyDiscount + premiumDiscount + 
                          quantityDiscount + categoryDiscount;

    // Ograniczenie do maksymalnego rabatu
    return Math.min(totalDiscount, this.MAX_DISCOUNT);
  }

  private getMembershipDiscount(years: number): number {
    if (years >= 5) return 0.15;
    if (years >= 3) return 0.10;
    if (years >= 2) return 0.07;
    if (years >= 1) return 0.05;
    return 0.02;
  }

  private getLoyaltyDiscount(points: number): number {
    if (points >= 5000) return 0.20;
    if (points >= 2000) return 0.15;
    if (points >= 1000) return 0.10;
    if (points > 0) return 0.05;
    return 0;
  }

  private getQuantityDiscount(quantity: number): number {
    if (quantity >= 100) return 0.25;
    if (quantity >= 50) return 0.20;
    if (quantity >= 10) return 0.15;
    if (quantity > 1) return 0.10;
    return 0;
  }

  private getCategoryDiscount(product: Product, isPremium: boolean): number {
    switch (product.category) {
      case "electronics":
        return product.price > 1000 ? 0.10 : 0.05;
      case "books":
        return 0.07;
      case "clothing":
        return isPremium ? 0.15 : 0.08;
      default:
        return 0;
    }
  }
}
