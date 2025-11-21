// Problem: Przekombinowana funkcja do obliczania rabatu z wieloma zagnieżdżonymi warunkami

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
  calculateDiscount(customer: Customer, product: Product, quantity: number): number {
    let discount = 0;
    let baseDiscount = 0;
    let loyaltyDiscount = 0;
    let premiumDiscount = 0;
    let quantityDiscount = 0;
    let categoryDiscount = 0;
    let finalDiscount = 0;
    let multiplier = 1;
    let bonusMultiplier = 1;

    // Krok 1: Oblicz bazowy rabat według lat członkostwa
    if (customer.yearsSinceRegistration > 0) {
      if (customer.yearsSinceRegistration >= 1) {
        if (customer.yearsSinceRegistration >= 2) {
          if (customer.yearsSinceRegistration >= 3) {
            if (customer.yearsSinceRegistration >= 5) {
              baseDiscount = 0.15;
            } else {
              baseDiscount = 0.10;
            }
          } else {
            baseDiscount = 0.07;
          }
        } else {
          baseDiscount = 0.05;
        }
      } else {
        baseDiscount = 0.02;
      }
    }

    // Krok 2: Oblicz rabat lojalnościowy
    if (customer.loyaltyPoints > 0) {
      if (customer.loyaltyPoints >= 1000) {
        if (customer.loyaltyPoints >= 2000) {
          if (customer.loyaltyPoints >= 5000) {
            loyaltyDiscount = 0.20;
          } else {
            loyaltyDiscount = 0.15;
          }
        } else {
          loyaltyDiscount = 0.10;
        }
      } else {
        loyaltyDiscount = 0.05;
      }
    }

    // Krok 3: Rabat premium
    if (customer.isPremium === true) {
      if (customer.yearsSinceRegistration >= 2) {
        premiumDiscount = 0.12;
        bonusMultiplier = 1.1;
      } else {
        premiumDiscount = 0.08;
      }
    }

    // Krok 4: Rabat ilościowy
    if (quantity > 1) {
      if (quantity >= 10) {
        if (quantity >= 50) {
          if (quantity >= 100) {
            quantityDiscount = 0.25;
          } else {
            quantityDiscount = 0.20;
          }
        } else {
          quantityDiscount = 0.15;
        }
      } else {
        quantityDiscount = 0.10;
      }
    }

    // Krok 5: Rabat kategorii
    if (product.category === "electronics") {
      if (product.price > 1000) {
        categoryDiscount = 0.10;
      } else {
        categoryDiscount = 0.05;
      }
    } else if (product.category === "books") {
      categoryDiscount = 0.07;
    } else if (product.category === "clothing") {
      if (customer.isPremium) {
        categoryDiscount = 0.15;
      } else {
        categoryDiscount = 0.08;
      }
    }

    // Krok 6: Kombinowanie rabatów (złożona logika)
    discount = baseDiscount;
    if (loyaltyDiscount > discount) {
      multiplier = 1.2;
      discount = loyaltyDiscount;
    }
    if (premiumDiscount > 0) {
      discount = discount + premiumDiscount * 0.5;
    }
    discount = discount + quantityDiscount;
    discount = discount + categoryDiscount;
    discount = discount * multiplier * bonusMultiplier;

    // Krok 7: Normalizacja (nie może przekroczyć 50%)
    if (discount > 0.50) {
      finalDiscount = 0.50;
    } else {
      if (discount < 0) {
        finalDiscount = 0;
      } else {
        finalDiscount = discount;
      }
    }

    return finalDiscount;
  }
}
