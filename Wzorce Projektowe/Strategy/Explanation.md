# Strategy Pattern

## 📌 Problem w Bad Example
W złym przykładzie logika obsługi różnych przewoźników jest rozsiana po metodach poprzez liczne instrukcje `if/else`. To prowadzi do:
- **Naruszenia Open/Closed Principle** – każdy nowy przewoźnik wymaga modyfikacji istniejących metod
- **Trudności w testowaniu** – niemożliwe jest testowanie logiki pojedynczego przewoźnika w izolacji
- **Rozrzucenia logiki** – zachowanie jednego przewoźnika jest rozdzielone między różne metody
- **Rosnącej złożoności** – dodanie nowych metod wymaga kolejnych bloków if/else

## ✅ Rozwiązanie: Strategy Pattern
Strategy to wzorzec behawioralny, który **enkapsuluje algorytmy w osobnych klasach** i umożliwia ich wymianę w czasie wykonania programu.

### Kluczowe elementy:
1. **Interfejs strategii** (`IShippingStrategy`) – definiuje wspólny kontrakt
2. **Konkretne strategie** (`DHLStrategy`, `UPSStrategy`, etc.) – implementują różne algorytmy
3. **Kontekst** (`ShippingService`) – używa strategii przez interfejs, nie znając szczegółów implementacji

## 🎯 Korzyści

### 1. Elastyczność
```typescript
// Łatwa zmiana algorytmu w runtime:
service.setStrategy(new FedExStrategy());
```

### 2. Testowalność
```typescript
// Każda strategia może być testowana niezależnie:
const dhlStrategy = new DHLStrategy();
expect(dhlStrategy.calculateCost(10, 200)).toBe(17);
```

### 3. Open/Closed Principle
Nowy przewoźnik = nowa klasa strategii. **Bez modyfikacji istniejącego kodu:**
```typescript
class DHLExpressStrategy implements IShippingStrategy {
    // Nowa strategia bez dotykania innych klas
}
```

### 4. Single Responsibility
Każda klasa strategii odpowiada **tylko** za logikę jednego przewoźnika.

## 🔄 Kiedy stosować?
- Masz wiele podobnych algorytmów różniących się detalami implementacji
- Chcesz uniknąć wielokrotnych instrukcji warunkowych
- Potrzebujesz możliwości zmiany algorytmu w czasie wykonania
- Algorytmy zawierają dane, które klient nie powinien znać

## ⚠️ Uwagi
- Klient musi znać dostępne strategie (można to rozwiązać przez Factory)
- Zwiększa liczbę obiektów w aplikacji
- Jeśli algorytmy są bardzo proste, wzorzec może być przesadą
