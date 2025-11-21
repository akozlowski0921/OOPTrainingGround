// ✅ GOOD: Decorator pattern for message processing

interface Message {
  getContent(): string;
  send(): void;
}

class TextMessage implements Message {
  constructor(private content: string) {}

  getContent(): string {
    return this.content;
  }

  send(): void {
    console.log("Sending:", this.content);
  }
}

// ✅ Base decorator
abstract class MessageDecorator implements Message {
  constructor(protected message: Message) {}

  abstract getContent(): string;

  send(): void {
    console.log("Sending:", this.getContent());
  }
}

// ✅ Concrete decorators
class EncryptedMessage extends MessageDecorator {
  getContent(): string {
    const content = this.message.getContent();
    return btoa(content); // Simple encryption
  }
}

class CompressedMessage extends MessageDecorator {
  getContent(): string {
    const content = this.message.getContent();
    return content.replace(/\s+/g, " ");
  }
}

class SignedMessage extends MessageDecorator {
  getContent(): string {
    const content = this.message.getContent();
    return content + "\n--Digital Signature";
  }
}

// ✅ Flexible combinations
const message = new TextMessage("Hello World");
const encrypted = new EncryptedMessage(message);
const compressed = new CompressedMessage(message);
const encryptedAndCompressed = new CompressedMessage(new EncryptedMessage(message));
const signedEncrypted = new SignedMessage(new EncryptedMessage(message));

encryptedAndCompressed.send();
