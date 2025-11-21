// Rozwiązanie: Early return - guard clauses dla warunków brzegowych

interface PaymentMethod {
  type: "card" | "paypal" | "bank";
  isValid: boolean;
  balance?: number;
}

interface Order {
  id: number;
  totalAmount: number;
  items: OrderItem[];
  shippingAddress?: Address;
}

interface OrderItem {
  productId: number;
  quantity: number;
  price: number;
}

interface Address {
  country: string;
  city: string;
  zipCode: string;
}

interface ProcessResult {
  success: boolean;
  error?: string;
  orderId?: number;
}

export class PaymentProcessor {
  private static readonly MAX_BANK_TRANSFER_AMOUNT = 50000;

  processPayment(
    order: Order | null,
    paymentMethod: PaymentMethod | null
  ): ProcessResult {
    // Fail fast - walidacja ogólna
    if (!order) {
      return { success: false, error: "Order not found" };
    }

    if (!order.items || order.items.length === 0) {
      return { success: false, error: "Order has no items" };
    }

    if (order.totalAmount <= 0) {
      return { success: false, error: "Order amount must be positive" };
    }

    if (!paymentMethod) {
      return { success: false, error: "Payment method required" };
    }

    if (!paymentMethod.isValid) {
      return { success: false, error: "Invalid payment method" };
    }

    // Przetwarzanie według typu płatności
    switch (paymentMethod.type) {
      case "card":
        return this.processCardPayment(order);

      case "paypal":
        return this.processPayPalPayment(order, paymentMethod);

      case "bank":
        return this.processBankTransfer(order);

      default:
        return { success: false, error: "Unknown payment type" };
    }
  }

  private processCardPayment(order: Order): ProcessResult {
    if (!order.shippingAddress) {
      return { success: false, error: "Shipping address required" };
    }

    if (!order.shippingAddress.country) {
      return { success: false, error: "Country required" };
    }

    return { success: true, orderId: order.id };
  }

  private processPayPalPayment(
    order: Order,
    paymentMethod: PaymentMethod
  ): ProcessResult {
    if (paymentMethod.balance === undefined) {
      return { success: false, error: "Balance not available" };
    }

    if (paymentMethod.balance < order.totalAmount) {
      return { success: false, error: "Insufficient balance" };
    }

    return { success: true, orderId: order.id };
  }

  private processBankTransfer(order: Order): ProcessResult {
    if (order.totalAmount > PaymentProcessor.MAX_BANK_TRANSFER_AMOUNT) {
      return { success: false, error: "Amount too high for bank transfer" };
    }

    if (!order.shippingAddress) {
      return { success: false, error: "Shipping address required" };
    }

    if (order.shippingAddress.country !== "PL") {
      return { success: false, error: "Bank transfer only for Poland" };
    }

    return { success: true, orderId: order.id };
  }
}
