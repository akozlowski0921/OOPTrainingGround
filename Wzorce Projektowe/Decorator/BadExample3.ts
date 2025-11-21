// ❌ BAD: Modifying base class for every new feature

class TextMessage {
  constructor(private content: string) {}

  send(): void {
    console.log("Sending:", this.content);
  }

  sendEncrypted(): void {
    const encrypted = this.encrypt(this.content);
    console.log("Sending encrypted:", encrypted);
  }

  sendCompressed(): void {
    const compressed = this.compress(this.content);
    console.log("Sending compressed:", compressed);
  }

  sendEncryptedAndCompressed(): void {
    const encrypted = this.encrypt(this.content);
    const compressed = this.compress(encrypted);
    console.log("Sending encrypted and compressed:", compressed);
  }

  sendWithSignature(): void {
    const signed = this.content + "\n--Signature";
    console.log("Sending with signature:", signed);
  }

  // Problem: Need to add more methods for every combination
  sendEncryptedWithSignature(): void { }
  sendCompressedWithSignature(): void { }
  sendEncryptedCompressedWithSignature(): void { }

  private encrypt(text: string): string {
    return btoa(text);
  }

  private compress(text: string): string {
    return text.replace(/\s+/g, " ");
  }
}
