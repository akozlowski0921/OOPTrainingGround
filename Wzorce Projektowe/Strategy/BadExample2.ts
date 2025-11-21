// ❌ BAD: Conditional logic for different algorithms

class DataProcessor {
  processData(data: string[], algorithm: string): string[] {
    if (algorithm === "uppercase") {
      return data.map(item => item.toUpperCase());
    } else if (algorithm === "lowercase") {
      return data.map(item => item.toLowerCase());
    } else if (algorithm === "capitalize") {
      return data.map(item => 
        item.charAt(0).toUpperCase() + item.slice(1).toLowerCase()
      );
    } else if (algorithm === "reverse") {
      return data.map(item => item.split("").reverse().join(""));
    } else if (algorithm === "trim") {
      return data.map(item => item.trim());
    }
    return data;
  }
}

// Problem: Adding new algorithm requires modifying the class
// Hard to test individual algorithms
const processor = new DataProcessor();
console.log(processor.processData(["hello", "WORLD"], "uppercase"));
