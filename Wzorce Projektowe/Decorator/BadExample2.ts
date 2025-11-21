// ❌ BAD: Subclassing explosion for feature combinations

abstract class Coffee {
  abstract getCost(): number;
  abstract getDescription(): string;
}

class SimpleCoffee extends Coffee {
  getCost(): number { return 5; }
  getDescription(): string { return "Simple Coffee"; }
}

// Problem: Need a class for every combination
class CoffeeWithMilk extends Coffee {
  getCost(): number { return 7; }
  getDescription(): string { return "Coffee with Milk"; }
}

class CoffeeWithSugar extends Coffee {
  getCost(): number { return 6; }
  getDescription(): string { return "Coffee with Sugar"; }
}

class CoffeeWithMilkAndSugar extends Coffee {
  getCost(): number { return 8; }
  getDescription(): string { return "Coffee with Milk and Sugar"; }
}

class CoffeeWithMilkSugarAndWhip extends Coffee {
  getCost(): number { return 10; }
  getDescription(): string { return "Coffee with Milk, Sugar and Whipped Cream"; }
}

// Need many more classes for all combinations!
// CoffeeWithSugarAndWhip, CoffeeWithMilkAndWhip, etc.
