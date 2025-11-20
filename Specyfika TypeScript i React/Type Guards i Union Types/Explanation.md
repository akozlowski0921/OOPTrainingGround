# Type Guards i Union Types

## Problem w BadExample

* **Rzutowanie "na siłę":** Użycie `as` do wymuszenia typu bez faktycznego sprawdzenia może prowadzić do błędów runtime.
* **Używanie `any`:** Całkowita utrata type safety - TypeScript nie może nas chronić przed błędami.
* **Brak discriminated unions:** String literale bez union types nie dają gwarancji poprawności.
* **Non-null assertion (`!`):** Używanie `!` mówi TypeScript "ufaj mi", ale może prowadzić do błędów jeśli się mylimy.
* **Niespójna struktura danych:** Przechowywanie wszystkich możliwych pól jednocześnie (cardNumber, bankAccount, paypalEmail) zamiast użycia union types.

## Czym są Type Guards?

Type Guard to funkcja lub wyrażenie, które pozwala TypeScript zawęzić typ zmiennej (type narrowing):

```typescript
// Predicate function - zwraca boolean z type predicate
function isDog(pet: Pet): pet is Dog {
  return pet.type === 'dog';
}

// Type narrowing - TypeScript wie jaki to typ
if (isDog(pet)) {
  pet.bark(); // OK - TypeScript wie że to Dog
}
```

## Discriminated Unions

Discriminated Union to pattern, gdzie każdy typ w union ma wspólne pole (discriminant), które jednoznacznie identyfikuje typ:

```typescript
type Shape =
  | { kind: 'circle'; radius: number }
  | { kind: 'square'; size: number }
  | { kind: 'rectangle'; width: number; height: number };

function area(shape: Shape): number {
  switch (shape.kind) {
    case 'circle':
      return Math.PI * shape.radius ** 2; // TypeScript wie że mamy radius
    case 'square':
      return shape.size ** 2; // TypeScript wie że mamy size
    case 'rectangle':
      return shape.width * shape.height; // TypeScript wie że mamy width i height
  }
}
```

## Rozwiązanie w GoodExample

* **Type Guards:** Funkcje z type predicate (`pet is Dog`) zapewniają type safety.
* **Discriminated Unions:** Typy z polem discriminant (`type`, `status`, `kind`) pozwalają na type narrowing.
* **Union Types zamiast any:** Precyzyjne typy zamiast utraty type safety.
* **Type Narrowing:** TypeScript automatycznie zawęża typ po sprawdzeniu warunku.
* **Exhaustive checking:** Switch/if ze wszystkimi przypadkami - TypeScript ostrzeże o brakujących.

## Rodzaje Type Guards

### 1. typeof (dla prymitywów)
```typescript
if (typeof value === 'string') {
  value.toUpperCase(); // OK
}
```

### 2. instanceof (dla klas)
```typescript
if (error instanceof Error) {
  console.log(error.message); // OK
}
```

### 3. in operator (dla właściwości)
```typescript
if ('bark' in pet) {
  pet.bark(); // OK
}
```

### 4. Custom type guards (predicate functions)
```typescript
function isDog(pet: Pet): pet is Dog {
  return pet.type === 'dog';
}
```

### 5. Discriminated unions (automatic narrowing)
```typescript
if (pet.type === 'dog') {
  pet.bark(); // OK - automatic narrowing
}
```

## Korzyści

* **Bezpieczeństwo typów:** Błędy są wykrywane podczas kompilacji, nie w runtime.
* **Inteligentne autouzupełnianie:** IDE wie dokładnie jakie właściwości są dostępne.
* **Refaktoryzacja:** Zmiana typu automatycznie pokazuje wszystkie miejsca wymagające aktualizacji.
* **Dokumentacja:** Typy służą jako dokumentacja - jasne jakie dane są oczekiwane.
* **Mniej bugów:** TypeScript zapobiega dostępowi do nieistniejących właściwości.

## Najlepsze praktyki

1. **Używaj discriminated unions** dla stanów komponentu (loading/success/error)
2. **Twórz custom type guards** dla złożonych warunków
3. **Unikaj `as` i `any`** - sygnał że coś jest nie tak z typami
4. **Używaj `unknown` zamiast `any`** - wymusza sprawdzenie typu przed użyciem
5. **Exhaustive checks** - użyj `never` aby TypeScript sprawdził wszystkie przypadki

## Exhaustive Check Pattern

```typescript
type Status = 'idle' | 'loading' | 'success' | 'error';

function assertNever(x: never): never {
  throw new Error('Unexpected value: ' + x);
}

function handleStatus(status: Status) {
  switch (status) {
    case 'idle': return 'Idle';
    case 'loading': return 'Loading';
    case 'success': return 'Success';
    case 'error': return 'Error';
    default:
      // TypeScript error jeśli nie obsłużyliśmy wszystkich przypadków
      return assertNever(status);
  }
}
```
