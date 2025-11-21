// ❌ BAD: Direct coupling to third-party library without adapter

interface ThirdPartyPaymentGateway {
  initializePayment(config: any): void;
  executeTransaction(data: any): any;
  getTransactionStatus(id: string): any;
}

class PayPalGateway implements ThirdPartyPaymentGateway {
  initializePayment(config: any): void {
    console.log("PayPal initialized with", config);
  }

  executeTransaction(data: any): any {
    // PayPal specific format
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

class StripeGateway {
  // Problem: Different interface than PayPal
  setup(options: any): void {
    console.log("Stripe setup with", options);
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

// Problem: Cannot use Stripe with PaymentProcessor without modifications
export class PaymentProcessor {
  constructor(private gateway: ThirdPartyPaymentGateway) {}

  processPayment(amount: number, method: string): void {
    this.gateway.initializePayment({ amount });
    const result = this.gateway.executeTransaction({ amount, paymentMethod: method });
    console.log("Payment result:", result);
  }
}

// Can only use PayPal, not Stripe!
// const processor = new PaymentProcessor(new StripeGateway()); // Error!
