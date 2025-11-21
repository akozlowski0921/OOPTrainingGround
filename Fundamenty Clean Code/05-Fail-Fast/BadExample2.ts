// Problem: Głębokie zagnieżdżenia zamiast wczesnej walidacji

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
  processPayment(
    order: Order | null,
    paymentMethod: PaymentMethod | null
  ): ProcessResult {
    // Zagnieżdżony "arrow code" - trudny do śledzenia
    if (order) {
      if (order.items && order.items.length > 0) {
        if (order.totalAmount > 0) {
          if (paymentMethod) {
            if (paymentMethod.isValid) {
              if (paymentMethod.type === "card") {
                // Płatność kartą
                if (order.shippingAddress) {
                  if (order.shippingAddress.country) {
                    // Przetwarzanie...
                    return { success: true, orderId: order.id };
                  } else {
                    return { success: false, error: "Country required" };
                  }
                } else {
                  return { success: false, error: "Shipping address required" };
                }
              } else if (paymentMethod.type === "paypal") {
                // PayPal
                if (paymentMethod.balance !== undefined) {
                  if (paymentMethod.balance >= order.totalAmount) {
                    // Przetwarzanie...
                    return { success: true, orderId: order.id };
                  } else {
                    return { success: false, error: "Insufficient balance" };
                  }
                } else {
                  return { success: false, error: "Balance not available" };
                }
              } else if (paymentMethod.type === "bank") {
                // Przelew bankowy
                if (order.totalAmount <= 50000) {
                  if (order.shippingAddress) {
                    if (order.shippingAddress.country === "PL") {
                      // Przetwarzanie...
                      return { success: true, orderId: order.id };
                    } else {
                      return {
                        success: false,
                        error: "Bank transfer only for Poland",
                      };
                    }
                  } else {
                    return {
                      success: false,
                      error: "Shipping address required",
                    };
                  }
                } else {
                  return {
                    success: false,
                    error: "Amount too high for bank transfer",
                  };
                }
              } else {
                return { success: false, error: "Unknown payment type" };
              }
            } else {
              return { success: false, error: "Invalid payment method" };
            }
          } else {
            return { success: false, error: "Payment method required" };
          }
        } else {
          return { success: false, error: "Order amount must be positive" };
        }
      } else {
        return { success: false, error: "Order has no items" };
      }
    } else {
      return { success: false, error: "Order not found" };
    }
  }
}
