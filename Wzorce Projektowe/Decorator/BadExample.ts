// ❌ BAD EXAMPLE: Eksplozja klas przez dziedziczenie

abstract class Coffee {
    abstract getCost(): number;
    abstract getDescription(): string;
}

class SimpleCoffee extends Coffee {
    getCost(): number {
        return 5;
    }

    getDescription(): string {
        return "Simple Coffee";
    }
}

// Każda kombinacja wymaga nowej klasy!

class CoffeeWithMilk extends Coffee {
    getCost(): number {
        return 5 + 2;
    }

    getDescription(): string {
        return "Coffee with Milk";
    }
}

class CoffeeWithSugar extends Coffee {
    getCost(): number {
        return 5 + 1;
    }

    getDescription(): string {
        return "Coffee with Sugar";
    }
}

class CoffeeWithMilkAndSugar extends Coffee {
    getCost(): number {
        return 5 + 2 + 1;
    }

    getDescription(): string {
        return "Coffee with Milk and Sugar";
    }
}

class CoffeeWithWhippedCream extends Coffee {
    getCost(): number {
        return 5 + 3;
    }

    getDescription(): string {
        return "Coffee with Whipped Cream";
    }
}

class CoffeeWithMilkAndWhippedCream extends Coffee {
    getCost(): number {
        return 5 + 2 + 3;
    }

    getDescription(): string {
        return "Coffee with Milk and Whipped Cream";
    }
}

class CoffeeWithSugarAndWhippedCream extends Coffee {
    getCost(): number {
        return 5 + 1 + 3;
    }

    getDescription(): string {
        return "Coffee with Sugar and Whipped Cream";
    }
}

class CoffeeWithMilkSugarAndWhippedCream extends Coffee {
    getCost(): number {
        return 5 + 2 + 1 + 3;
    }

    getDescription(): string {
        return "Coffee with Milk, Sugar and Whipped Cream";
    }
}

// A co jeśli dodamy karmel?
class CoffeeWithCaramel extends Coffee {
    getCost(): number {
        return 5 + 2;
    }

    getDescription(): string {
        return "Coffee with Caramel";
    }
}

class CoffeeWithMilkAndCaramel extends Coffee {
    getCost(): number {
        return 5 + 2 + 2;
    }

    getDescription(): string {
        return "Coffee with Milk and Caramel";
    }
}

// ... i tak dalej, i tak dalej...
// 4 dodatki = 2^4 = 16 możliwych kombinacji = 16 klas!
// 5 dodatków = 32 klasy, 6 dodatków = 64 klasy...

// Użycie:
const coffee1 = new CoffeeWithMilkAndSugar();
console.log(`${coffee1.getDescription()}: $${coffee1.getCost()}`);

// Problemy:
// 1. EKSPLOZJA LICZBY KLAS - każda kombinacja wymaga nowej klasy
// 2. Duplikacja kodu - logika dodatków powtarza się w każdej klasie
// 3. Trudność w utrzymaniu - zmiana ceny dodatku wymaga zmian w wielu klasach
// 4. Niemożność dynamicznego dodawania dodatków w runtime
// 5. Naruszenie Open/Closed Principle - nowy dodatek = modyfikacja wielu klas
