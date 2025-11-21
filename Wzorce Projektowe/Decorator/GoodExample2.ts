// ✅ GOOD: Decorator pattern for flexible combinations

interface Coffee {
  getCost(): number;
  getDescription(): string;
}

class SimpleCoffee implements Coffee {
  getCost(): number {
    return 5;
  }

  getDescription(): string {
    return "Simple Coffee";
  }
}

// ✅ Base decorator
abstract class CoffeeDecorator implements Coffee {
  constructor(protected coffee: Coffee) {}

  abstract getCost(): number;
  abstract getDescription(): string;
}

// ✅ Concrete decorators
class MilkDecorator extends CoffeeDecorator {
  getCost(): number {
    return this.coffee.getCost() + 2;
  }

  getDescription(): string {
    return this.coffee.getDescription() + ", Milk";
  }
}

class SugarDecorator extends CoffeeDecorator {
  getCost(): number {
    return this.coffee.getCost() + 1;
  }

  getDescription(): string {
    return this.coffee.getDescription() + ", Sugar";
  }
}

class WhippedCreamDecorator extends CoffeeDecorator {
  getCost(): number {
    return this.coffee.getCost() + 3;
  }

  getDescription(): string {
    return this.coffee.getDescription() + ", Whipped Cream";
  }
}

// ✅ Flexible combinations
const coffee1 = new SimpleCoffee();
const coffee2 = new MilkDecorator(new SimpleCoffee());
const coffee3 = new SugarDecorator(new MilkDecorator(new SimpleCoffee()));
const coffee4 = new WhippedCreamDecorator(new SugarDecorator(new MilkDecorator(new SimpleCoffee())));

console.log(coffee4.getDescription(), coffee4.getCost());
