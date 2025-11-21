// ✅ GOOD: Adapter pattern allows using different payment gateways

interface PaymentGateway {
  initialize(amount: number): void;
  process(amount: number, method: string): PaymentResult;
  checkStatus(transactionId: string): TransactionStatus;
}

interface PaymentResult {
  transactionId: string;
  success: boolean;
  amount: number;
}

interface TransactionStatus {
  transactionId: string;
  completed: boolean;
}

// PayPal SDK (third-party)
class PayPalSDK {
  initializePayment(config: any): void {
    console.log("PayPal initialized");
  }

  executeTransaction(data: any): any {
    return {
      transactionId: "PP-" + Date.now(),
      status: "SUCCESS",
      paymentMethod: data.paymentMethod
    };
  }

  getTransactionStatus(id: string): any {
    return { id, status: "COMPLETED" };
  }
}

// Stripe SDK (third-party)
class StripeSDK {
  setup(options: any): void {
    console.log("Stripe setup");
  }

  charge(amount: number, token: string): any {
    return {
      id: "ch_" + Date.now(),
      paid: true,
      amount: amount
    };
  }

  retrieveCharge(chargeId: string): any {
    return { chargeId, paid: true };
  }
}

// ✅ Adapter for PayPal
class PayPalAdapter implements PaymentGateway {
  constructor(private paypal: PayPalSDK) {}

  initialize(amount: number): void {
    this.paypal.initializePayment({ amount });
  }

  process(amount: number, method: string): PaymentResult {
    const result = this.paypal.executeTransaction({ amount, paymentMethod: method });
    return {
      transactionId: result.transactionId,
      success: result.status === "SUCCESS",
      amount: amount
    };
  }

  checkStatus(transactionId: string): TransactionStatus {
    const status = this.paypal.getTransactionStatus(transactionId);
    return {
      transactionId: status.id,
      completed: status.status === "COMPLETED"
    };
  }
}

// ✅ Adapter for Stripe
class StripeAdapter implements PaymentGateway {
  constructor(private stripe: StripeSDK) {}

  initialize(amount: number): void {
    this.stripe.setup({ amount });
  }

  process(amount: number, method: string): PaymentResult {
    const result = this.stripe.charge(amount, method);
    return {
      transactionId: result.id,
      success: result.paid,
      amount: result.amount
    };
  }

  checkStatus(transactionId: string): TransactionStatus {
    const status = this.stripe.retrieveCharge(transactionId);
    return {
      transactionId: status.chargeId,
      completed: status.paid
    };
  }
}

// ✅ Now can work with any gateway through common interface
export class PaymentProcessor {
  constructor(private gateway: PaymentGateway) {}

  processPayment(amount: number, method: string): void {
    this.gateway.initialize(amount);
    const result = this.gateway.process(amount, method);
    console.log("Payment result:", result);
  }
}

// Both work!
const paypalProcessor = new PaymentProcessor(new PayPalAdapter(new PayPalSDK()));
const stripeProcessor = new PaymentProcessor(new StripeAdapter(new StripeSDK()));
