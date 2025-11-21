// ✅ GOOD: Strategy pattern for payment methods

interface PaymentStrategy {
  processPayment(amount: number): void;
}

class CreditCardStrategy implements PaymentStrategy {
  processPayment(amount: number): void {
    console.log("Processing credit card payment");
    this.validateCreditCard();
    this.charge(amount);
  }

  private validateCreditCard(): void {
    console.log("Validating credit card");
  }

  private charge(amount: number): void {
    console.log(`Charging $${amount} to credit card`);
  }
}

class PayPalStrategy implements PaymentStrategy {
  processPayment(amount: number): void {
    console.log("Processing PayPal payment");
    this.authenticate();
    this.charge(amount);
  }

  private authenticate(): void {
    console.log("Authenticating PayPal");
  }

  private charge(amount: number): void {
    console.log(`Charging $${amount} via PayPal`);
  }
}

class BitcoinStrategy implements PaymentStrategy {
  processPayment(amount: number): void {
    console.log("Processing Bitcoin payment");
    this.validateAddress();
    this.transfer(amount);
  }

  private validateAddress(): void {
    console.log("Validating Bitcoin address");
  }

  private transfer(amount: number): void {
    console.log(`Transferring $${amount} worth of Bitcoin`);
  }
}

class BankTransferStrategy implements PaymentStrategy {
  processPayment(amount: number): void {
    console.log("Processing bank transfer");
    this.validateAccount();
    this.initiate(amount);
  }

  private validateAccount(): void {
    console.log("Validating bank account");
  }

  private initiate(amount: number): void {
    console.log(`Initiating transfer of $${amount}`);
  }
}

// ✅ Context with strategy
class PaymentProcessor {
  constructor(private strategy: PaymentStrategy) {}

  setPaymentStrategy(strategy: PaymentStrategy): void {
    this.strategy = strategy;
  }

  processPayment(amount: number): void {
    this.strategy.processPayment(amount);
  }
}

// ✅ Easy to add new payment methods
const processor = new PaymentProcessor(new CreditCardStrategy());
processor.processPayment(100);

processor.setPaymentStrategy(new BitcoinStrategy());
processor.processPayment(100);
