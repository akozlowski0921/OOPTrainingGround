// ✅ GOOD: Strategy pattern for different algorithms

interface ProcessingStrategy {
  process(data: string[]): string[];
}

class UppercaseStrategy implements ProcessingStrategy {
  process(data: string[]): string[] {
    return data.map(item => item.toUpperCase());
  }
}

class LowercaseStrategy implements ProcessingStrategy {
  process(data: string[]): string[] {
    return data.map(item => item.toLowerCase());
  }
}

class CapitalizeStrategy implements ProcessingStrategy {
  process(data: string[]): string[] {
    return data.map(item => 
      item.charAt(0).toUpperCase() + item.slice(1).toLowerCase()
    );
  }
}

class ReverseStrategy implements ProcessingStrategy {
  process(data: string[]): string[] {
    return data.map(item => item.split("").reverse().join(""));
  }
}

class TrimStrategy implements ProcessingStrategy {
  process(data: string[]): string[] {
    return data.map(item => item.trim());
  }
}

// ✅ Context uses strategy
class DataProcessor {
  constructor(private strategy: ProcessingStrategy) {}

  setStrategy(strategy: ProcessingStrategy): void {
    this.strategy = strategy;
  }

  processData(data: string[]): string[] {
    return this.strategy.process(data);
  }
}

// ✅ Easy to add new strategies, easy to test
const processor = new DataProcessor(new UppercaseStrategy());
console.log(processor.processData(["hello", "WORLD"]));

processor.setStrategy(new CapitalizeStrategy());
console.log(processor.processData(["hello", "WORLD"]));
