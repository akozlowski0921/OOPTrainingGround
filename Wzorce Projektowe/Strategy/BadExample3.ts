// ❌ BAD: Switch statement for payment methods

class PaymentProcessor {
  processPayment(amount: number, method: string): void {
    switch (method) {
      case "creditcard":
        console.log("Processing credit card payment");
        this.validateCreditCard();
        this.chargeCreditCard(amount);
        break;
      case "paypal":
        console.log("Processing PayPal payment");
        this.authenticatePayPal();
        this.chargePayPal(amount);
        break;
      case "bitcoin":
        console.log("Processing Bitcoin payment");
        this.validateBitcoinAddress();
        this.transferBitcoin(amount);
        break;
      case "banktransfer":
        console.log("Processing bank transfer");
        this.validateBankAccount();
        this.initiateBankTransfer(amount);
        break;
      default:
        throw new Error("Unknown payment method");
    }
  }

  private validateCreditCard(): void { }
  private chargeCreditCard(amount: number): void { }
  private authenticatePayPal(): void { }
  private chargePayPal(amount: number): void { }
  private validateBitcoinAddress(): void { }
  private transferBitcoin(amount: number): void { }
  private validateBankAccount(): void { }
  private initiateBankTransfer(amount: number): void { }
}
