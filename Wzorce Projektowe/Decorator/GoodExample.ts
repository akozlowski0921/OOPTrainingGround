// ✅ GOOD EXAMPLE: Decorator Pattern

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

// Bazowy dekorator
abstract class CoffeeDecorator implements Coffee {
    protected coffee: Coffee;

    constructor(coffee: Coffee) {
        this.coffee = coffee;
    }

    getCost(): number {
        return this.coffee.getCost();
    }

    getDescription(): string {
        return this.coffee.getDescription();
    }
}

// Konkretne dekoratory
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

class CaramelDecorator extends CoffeeDecorator {
    getCost(): number {
        return this.coffee.getCost() + 2;
    }

    getDescription(): string {
        return this.coffee.getDescription() + ", Caramel";
    }
}

// Użycie - dynamiczne budowanie kombinacji:
let coffee1: Coffee = new SimpleCoffee();
console.log(`${coffee1.getDescription()}: $${coffee1.getCost()}`);
// Output: Simple Coffee: $5

let coffee2: Coffee = new SimpleCoffee();
coffee2 = new MilkDecorator(coffee2);
coffee2 = new SugarDecorator(coffee2);
console.log(`${coffee2.getDescription()}: $${coffee2.getCost()}`);
// Output: Simple Coffee, Milk, Sugar: $8

let coffee3: Coffee = new SimpleCoffee();
coffee3 = new MilkDecorator(coffee3);
coffee3 = new WhippedCreamDecorator(coffee3);
coffee3 = new CaramelDecorator(coffee3);
console.log(`${coffee3.getDescription()}: $${coffee3.getCost()}`);
// Output: Simple Coffee, Milk, Whipped Cream, Caramel: $12

// Jeszcze czytelniej z fluent interface:
class CoffeeBuilder {
    private coffee: Coffee;

    constructor() {
        this.coffee = new SimpleCoffee();
    }

    addMilk(): CoffeeBuilder {
        this.coffee = new MilkDecorator(this.coffee);
        return this;
    }

    addSugar(): CoffeeBuilder {
        this.coffee = new SugarDecorator(this.coffee);
        return this;
    }

    addWhippedCream(): CoffeeBuilder {
        this.coffee = new WhippedCreamDecorator(this.coffee);
        return this;
    }

    addCaramel(): CoffeeBuilder {
        this.coffee = new CaramelDecorator(this.coffee);
        return this;
    }

    build(): Coffee {
        return this.coffee;
    }
}

// Fluent usage:
const coffee4 = new CoffeeBuilder()
    .addMilk()
    .addSugar()
    .addWhippedCream()
    .build();

console.log(`${coffee4.getDescription()}: $${coffee4.getCost()}`);
// Output: Simple Coffee, Milk, Sugar, Whipped Cream: $11

// Korzyści:
// 1. Tylko 1 klasa na dodatek (zamiast 2^n kombinacji!)
// 2. Dynamiczne dodawanie funkcjonalności w runtime
// 3. Łatwa zmiana ceny dodatku w jednym miejscu
// 4. Nowy dodatek = nowa klasa dekoratora (Open/Closed Principle)
// 5. Dowolne kombinacje bez eksplozji klas
