# Fail Fast vs Arrow Code

## Przykład 1: Dostęp do dokumentów

### Problem w BadExample
* **Arrow Code:** Kod przesunięty coraz bardziej w prawo przez zagnieżdżenia
* **Trudna nawigacja:** Ciężko śledzić wszystkie ścieżki wykonania
* **Kognitywne obciążenie:** Trzeba trzymać w głowie wiele poziomów kontekstu
* **Trudne debugowanie:** Ciężko umieścić breakpoint w odpowiednim miejscu

### Rozwiązanie w GoodExample
* **Early return:** Sprawdzamy warunki brzegowe na początku i natychmiast wychodzimy
* **Guard clauses:** Najpierw eliminujemy nieprawidłowe przypadki
* **Liniowy flow:** Kod czyta się od góry do dołu bez zagnieżdżeń
* **Poziom wcięcia:** Maksymalnie 1-2 poziomy zamiast 6+

## Przykład 2: Przetwarzanie płatności

### Problem w BadExample2
* **10+ poziomów zagnieżdżenia:** Kod schodzi tak głęboko, że trudno go śledzić
* **Duplikacja warunków:** Te same sprawdzenia w różnych miejscach
* **Wszystkie błędy na końcu:** Logika błędów rozrzucona po całej funkcji
* **Trudna konserwacja:** Dodanie nowego typu płatności wymaga modyfikacji głębokich zagnieżdżeń

### Rozwiązanie w GoodExample2
* **Walidacja na początku:** Wszystkie podstawowe sprawdzenia jako guard clauses
* **Switch statement:** Czytelny routing do specjalizowanych metod
* **Wydzielone metody:** Każdy typ płatności ma swoją metodę
* **Spłaszczona struktura:** Maksymalnie 2 poziomy wcięcia

## Przykład 3: Rezerwacja hotelowa

### Problem w BadExample3
* **12+ poziomów zagnieżdżeń:** Niemożliwe do śledzenia bez przewijania
* **Logika zagubiona w środku:** Główna logika biznesowa ukryta między warunkami
* **Trudne testowanie:** Trzeba przygotować skomplikowane kombinacje danych
* **Ciężko znaleźć gdzie jest błąd:** Każdy if może być źródłem problemu

### Rozwiązanie in GoodExample3
* **Sekwencyjne guard clauses:** Każdy warunek eliminacyjny osobno
* **Wydzielone metody pomocnicze:** `calculateNights()`, `isDepositWaived()`
* **Nazwane stałe:** Jasne progi i limity
* **Happy path na końcu:** Sukces to ostatnia linijka, nie zagnieżdżenie

## Korzyści biznesowe

* **Szybsze code review:** Łatwiej zrozumieć logikę biznesową
* **Mniej błędów:** Prostszy flow = mniej pomyłek w logice
* **Łatwiejsze modyfikacje:** Dodanie nowego warunku to kilka linijek na początku
* **Lepsza komunikacja:** Kod czyta się jak lista wymagań biznesowych

## Korzyści techniczne

* **Lepsza czytelność:** Każdy warunek jest widoczny od razu
* **Łatwiejsze testowanie:** Każda ścieżka jest wyraźnie określona
* **Mniejsza złożoność cyklomatyczna:** Mniej zagnieżdżeń = prostsza struktura
* **Debugowanie:** Łatwo umieścić breakpoint i śledzić flow
* **Konserwacja:** Dodanie nowej walidacji nie wymaga modyfikacji istniejącego kodu

## Kluczowa zasada

**Sprawdzaj warunki błędu najpierw i wychodź natychmiast. Pozytywny flow na końcu.**

## Wzorzec

```typescript
// Najpierw warunki brzegowe (null checks, validation)
if (!input) return error;

// Potem warunki biznesowe (fail fast)
if (someCondition) return result;
if (anotherCondition) return result;

// Happy path na końcu
return successResult;
```

---

## 🎯 FAQ / INSIGHT

### Po co stosować Fail Fast i Guard Clauses?

**Problem z głębokim zagnieżdżeniem (Arrow Code):**
- **Trudna nawigacja** – kod przesunięty coraz bardziej w prawo
- **Kognitywne przeciążenie** – trzeba pamiętać wiele poziomów kontekstu
- **Trudne debugowanie** – ciężko umieścić breakpoint
- **Niska czytelność** – happy path zagubiony w środku
- **Wysoka złożoność cyklomatyczna** – więcej ścieżek = więcej błędów

**Korzyści z Fail Fast:**
- **Liniowy flow** – kod czyta się od góry do dołu
- **Jasne warunki** – każdy guard clause to osobna linia
- **Happy path na końcu** – główna logika nie jest zagnieżdżona
- **Mniejsza złożoność** – płaska struktura zamiast pyramidy
- **Łatwe testowanie** – każda ścieżka jest wyraźna

### W czym pomaga Fail Fast?

✅ **Czytelność** – kod czyta się jak proza, nie jak labirynt  
✅ **Maintainability** – łatwo dodać nowy warunek na początku  
✅ **Debugging** – jasne punkty wyjścia przy błędach  
✅ **Testing** – każdy guard clause to osobny test case  
✅ **Cognitive load** – mniej zagnieżdżeń = mniej do zapamiętania  
✅ **Code review** – recenzent widzi wszystkie warunki od razu  

### ⚖️ Zalety i wady

#### Zalety
✅ **Płaska struktura** – maksymalnie 1-2 poziomy wcięcia  
✅ **Early returns** – wyjście przy pierwszym problemie  
✅ **Separation of concerns** – walidacja oddzielona od logiki  
✅ **Łatwe rozszerzanie** – nowy warunek = nowa linijka na początku  
✅ **Self-documenting** – guard clauses wyjaśniają preconditions  
✅ **Mniejsza złożoność cyklomatyczna** – mniej zagnieżdżonych if-ów  

#### Wady (marginalne)
❌ **Więcej return statements** – niektórzy preferują single exit point (outdated practice)  
❌ **"Negatywne" myślenie** – najpierw błędy, potem sukces (ale to właśnie cel!)  

### ⚠️ Na co uważać?

#### 1. **Kolejność guard clauses ma znaczenie**
```csharp
// ❌ BAD: Sprawdzamy szczegóły przed basics
if (user.Age < 18) return "Too young";
if (user == null) return "User not found";  // Crash! Null reference

// ✅ GOOD: Najpierw null checks, potem business rules
if (user == null) return "User not found";
if (user.Age < 18) return "Too young";
```

#### 2. **Nie mieszaj validation z business logic**
```csharp
// ❌ BAD: Validation i logika pomieszane
public void ProcessOrder(Order order)
{
    if (order != null)
    {
        var discount = CalculateDiscount(order);
        if (order.Items.Count > 0)
        {
            var total = CalculateTotal(order);
            if (order.Customer != null)
            {
                // ...
            }
        }
    }
}

// ✅ GOOD: Wszystkie validations na początku
public void ProcessOrder(Order order)
{
    // Validation guards
    if (order == null) throw new ArgumentNullException(nameof(order));
    if (order.Items.Count == 0) throw new InvalidOperationException("No items");
    if (order.Customer == null) throw new InvalidOperationException("No customer");
    
    // Business logic (happy path)
    var discount = CalculateDiscount(order);
    var total = CalculateTotal(order);
    SaveOrder(order, total);
}
```

#### 3. **Guard clauses vs single exit point**
```csharp
// ❌ OLD STYLE: Single exit point (C legacy)
public string ProcessData(Data data)
{
    string result = null;
    
    if (data != null)
    {
        if (data.IsValid)
        {
            if (data.HasPermission)
            {
                result = DoProcessing(data);
            }
        }
    }
    
    return result;  // Single return, ale arrow code!
}

// ✅ MODERN: Multiple returns with guard clauses
public string ProcessData(Data data)
{
    if (data == null) return null;
    if (!data.IsValid) return null;
    if (!data.HasPermission) return null;
    
    return DoProcessing(data);  // Happy path!
}
```

**Uwaga:** Single exit point był ważny w C gdzie manual memory management wymagał cleanup. W nowoczesnych językach z GC i RAII (C# `using`, C++ destructors) jest to nieaktualne.

#### 4. **Zbyt wiele guard clauses**
```csharp
// ❌ BAD: 20 guard clauses
public void Process(Order order)
{
    if (order == null) return;
    if (order.Id == 0) return;
    if (order.Customer == null) return;
    if (order.Customer.Name == null) return;
    // ... 16 more guards
    
    // Happy path zagubiony!
}

// ✅ GOOD: Grupuj validation w helper methods
public void Process(Order order)
{
    ValidateOrder(order);  // Rzuca exception przy błędzie
    ValidateCustomer(order.Customer);
    
    // Happy path wyraźny
    ProcessValidOrder(order);
}

private void ValidateOrder(Order order)
{
    if (order == null) throw new ArgumentNullException(nameof(order));
    if (order.Id == 0) throw new ArgumentException("Invalid order ID");
    if (order.Items.Count == 0) throw new ArgumentException("No items");
}
```

#### 5. **Return vs Throw w guard clauses**
```csharp
// ❌ INCONSISTENT: Mieszanie return i throw
public Result ProcessOrder(Order order)
{
    if (order == null) return Result.Error("Null order");
    if (order.Id == 0) throw new ArgumentException("Invalid ID");  // Inconsistent!
    if (!order.IsValid) return Result.Error("Invalid");
}

// ✅ GOOD: Spójne podejście
// Opcja 1: Return Result
public Result ProcessOrder(Order order)
{
    if (order == null) return Result.Error("Null order");
    if (order.Id == 0) return Result.Error("Invalid ID");
    if (!order.IsValid) return Result.Error("Invalid order");
    
    return Result.Success(Process(order));
}

// Opcja 2: Throw exceptions (dla preconditions)
public void ProcessOrder(Order order)
{
    if (order == null) throw new ArgumentNullException(nameof(order));
    if (order.Id == 0) throw new ArgumentException("Invalid ID");
    if (!order.IsValid) throw new InvalidOperationException("Invalid");
    
    Process(order);
}
```

### 🚨 Najczęstsze pomyłki

#### 1. **Arrow code (pyramid of doom)**
```typescript
// ❌ BAD: 8 poziomów zagnieżdżenia
function processPayment(order: Order): Result {
    if (order) {
        if (order.items.length > 0) {
            if (order.customer) {
                if (order.customer.paymentMethod) {
                    if (order.total > 0) {
                        if (order.currency === 'USD') {
                            if (order.customer.balance >= order.total) {
                                // Happy path zagubiony!
                                return processTransaction(order);
                            }
                        }
                    }
                }
            }
        }
    }
    return { success: false };
}

// ✅ GOOD: Płaska struktura z guard clauses
function processPayment(order: Order): Result {
    if (!order) return { success: false, error: 'No order' };
    if (order.items.length === 0) return { success: false, error: 'No items' };
    if (!order.customer) return { success: false, error: 'No customer' };
    if (!order.customer.paymentMethod) return { success: false, error: 'No payment method' };
    if (order.total <= 0) return { success: false, error: 'Invalid total' };
    if (order.currency !== 'USD') return { success: false, error: 'Invalid currency' };
    if (order.customer.balance < order.total) return { success: false, error: 'Insufficient funds' };
    
    return processTransaction(order);  // Happy path!
}
```

#### 2. **Duplikacja warunków**
```csharp
// ❌ BAD: Warunek powtórzony
public void Process(User user)
{
    if (user != null)
    {
        if (user.IsActive)
        {
            DoSomething(user);
        }
    }
    
    // Gdzie indziej:
    if (user != null && user.IsActive)
    {
        DoSomethingElse(user);
    }
}

// ✅ GOOD: Raz sprawdź, potem używaj
public void Process(User user)
{
    if (user == null) return;
    if (!user.IsActive) return;
    
    // Tutaj wiemy że user jest valid
    DoSomething(user);
    DoSomethingElse(user);
}
```

#### 3. **Zbyt ogólne error messages**
```csharp
// ❌ BAD: Niejasne błędy
if (order == null) return false;
if (order.Items.Count == 0) return false;
if (order.Total == 0) return false;

// ✅ GOOD: Konkretne komunikaty
if (order == null) 
    throw new ArgumentNullException(nameof(order), "Order cannot be null");
if (order.Items.Count == 0) 
    throw new InvalidOperationException("Order must have at least one item");
if (order.Total <= 0) 
    throw new InvalidOperationException("Order total must be greater than zero");
```

#### 4. **Walidacja w środku logiki**
```csharp
// ❌ BAD: Validation zagubiona w logice
public void ProcessOrder(Order order)
{
    var discount = CalculateDiscount(order);
    
    if (order.Items == null) return;  // Za późno!
    
    var total = order.Items.Sum(i => i.Price);
    
    if (total <= 0) return;  // Za późno!
    
    SaveOrder(order);
}

// ✅ GOOD: Wszystkie validations na początku
public void ProcessOrder(Order order)
{
    // All guards upfront
    if (order == null) throw new ArgumentNullException(nameof(order));
    if (order.Items == null || order.Items.Count == 0) 
        throw new InvalidOperationException("No items");
    
    // Business logic
    var discount = CalculateDiscount(order);
    var total = order.Items.Sum(i => i.Price);
    
    if (total <= 0) throw new InvalidOperationException("Invalid total");
    
    SaveOrder(order);
}
```

### 💼 Kontekst biznesowy

**Scenariusz: Code review złożonej funkcji biznesowej**

**Bez Fail Fast (Arrow Code):**
```csharp
// Reviewer musi "odwijać" zagnieżdżenia mentalnie:
if (condition1) {
    if (condition2) {
        if (condition3) {
            // Co jest preconditions? Co jest business logic?
            // Gdzie jest happy path?
        }
    }
}
```
- Code review: 30 minut
- Pytania: 10+ "co jeśli...?"
- Bugs found: 3
- Time to understand: 15 minut

**Z Fail Fast:**
```csharp
// Reviewer widzi wszystko od razu:
if (!condition1) return error1;  // Precondition 1
if (!condition2) return error2;  // Precondition 2
if (!condition3) return error3;  // Precondition 3

// Happy path - główna logika biznesowa
return ProcessLogic();
```
- Code review: 10 minut
- Pytania: 2 "czy to wszystkie warunki?"
- Bugs found: 0
- Time to understand: 3 minuty

**ROI:** Fail Fast oszczędza 20 minut na code review + 12 minut onboarding = 32 minuty na funkcję!

### 📝 Podsumowanie

- **Fail Fast** – sprawdzaj błędy na początku, wychodź natychmiast, happy path na końcu
- **Stosuj** guard clauses dla walidacji, płaską strukturę zamiast arrow code
- **Uważaj** na kolejność guards (null checks first!), spójność return vs throw
- **Najczęstsze błędy:** arrow code, duplikacja warunków, walidacja w środku logiki
- **Korzyść biznesowa:** szybszy code review, łatwiejszy onboarding, mniej bugów
