# Magic Numbers i Magic Strings

## Przykład 1: Statusy zamówień i rabaty

### Problem w BadExample
* **Brak kontekstu:** Co oznacza `status == 4`? Co to jest `"admin"`?
* **Trudność w utrzymaniu:** Zmiana wartości wymaga przejrzenia całego kodu
* **Podatność na błędy:** Łatwo napisać `"Admin"` zamiast `"admin"` (case sensitivity)
* **Brak autocomplete:** IDE nie podpowie dostępnych opcji
* **Brak type safety:** Można przypisać dowolny int lub string

### Rozwiązanie w GoodExample
* **Enum dla statusów:** `OrderStatus.Cancelled` zamiast magicznej `4`
* **Enum dla typów:** `UserType.Admin` zamiast magicznego `"admin"`
* **Named constants:** `BusinessConstants.VipDiscountRate` zamiast magicznego `0.2`
* **Samodokumentujący się kod:** Nazwa mówi co wartość oznacza

## Przykład 2: Obliczenia podatkowe i kary

### Problem w BadExample2
* **Tajemnicze wartości:** Co oznacza 0.23? 0.15? 30? 90?
* **Brak dokumentacji:** Trzeba zgadywać co reprezentują liczby
* **Trudne zmiany:** Zmiana stawki VAT wymaga znalezienia wszystkich wystąpień
* **Magic strings:** "draft", "pending" - podatne na literówki

### Rozwiązanie w GoodExample2
* **Nazwane stałe VAT:** `TaxRates.StandardVatRate` jasno określa cel
* **Polityki płatności:** `PaymentPolicies` grupuje wszystkie reguły biznesowe
* **Enums dla statusów:** Type-safe statusy faktur
* **Komentarze przy stałych:** Wyjaśniają znaczenie wartości (23% VAT, 5% kara)

## Przykład 3: Koszty wysyłki i limity

### Problem w BadExample3
* **Niezrozumiałe liczby:** 10, 25, 45, 3.5 - co one oznaczają?
* **Magic strings dla lokalizacji:** "domestic", "eu", "world" - bez type safety
* **Wymiary i objętości:** 200, 150, 500000 - jakie to jednostki?
* **Mnożniki:** 1.5, 2.0 - co one robią?

### Rozwiązanie w GoodExample3
* **Enums dla typów:** `ShippingDestination`, `ShippingPriority` - jasne opcje
* **Nazwane stawki:** `ShippingRates.DomesticUpTo1Kg` - zrozumiałe progi
* **Limity z jednostkami:** `PackageLimits.MaxLengthCm` - jasne jednostki miary
* **Wydzielone metody:** Logika dla każdej destynacji osobno

## Korzyści biznesowe

* **Mniej błędów:** Type safety i autocomplete eliminują literówki
* **Łatwiejsze zmiany:** Zmiana wartości progowej w jednym miejscu
* **Lepsza komunikacja:** Nazwy odpowiadają terminologii biznesowej
* **Audytowalność:** Łatwo sprawdzić aktualne stawki i progi

## Korzyści techniczne

* **Autocomplete:** IDE podpowie wszystkie dostępne opcje
* **Type safety:** Kompilator wykryje błędne użycie
* **Łatwiejsze refaktoryzacje:** Rename propaguje się automatycznie
* **Czytelność:** Intencja kodu jest natychmiastowa
* **Centralizacja:** Wszystkie wartości biznesowe w jednym miejscu
* **Konsystencja:** Te same wartości używane wszędzie

## Praktyczne wskazówki

* Używaj `enum` dla ograniczonych zestawów wartości
* Używaj `const` dla wartości biznesowych (progi, stawki, limity)
* Nazywaj stałe według ich znaczenia biznesowego, nie wartości (`VipDiscountRate` nie `TwoZeroPercent`)
* Grupuj powiązane stałe w klasach statycznych (np. `TaxRates`, `PaymentPolicies`)
* Dodawaj komentarze z wyjaśnieniem przy stałych (np. `// 23% VAT`)

---

## 🎯 FAQ / INSIGHT

### Po co eliminować Magic Numbers i Magic Strings?

**Problem z magic values:**
- **Brak kontekstu** – co oznacza `42`, `"pending"`, `0.23`?
- **Duplikacja** – ta sama wartość powtórzona w wielu miejscach
- **Błędy** – łatwo o literówkę: `"Admin"` vs `"admin"`
- **Trudne zmiany** – zmiana wartości wymaga znalezienia wszystkich wystąpień
- **Brak type safety** – można przypisać dowolną wartość

**Korzyści z nazwanych stałych:**
- **Self-documenting code** – nazwa wyjaśnia znaczenie
- **Single source of truth** – wartość w jednym miejscu
- **Type safety** – enums zapobiegają błędnym wartościom
- **Autocomplete** – IDE podpowiada dostępne opcje
- **Łatwe zmiany** – zmiana w jednym miejscu propaguje się wszędzie

### W czym pomaga używanie nazwanych stałych?

✅ **Czytelność** – `if (status == OrderStatus.Cancelled)` vs `if (status == 4)`  
✅ **Maintainability** – zmiana VAT z 23% na 25% w jednym miejscu  
✅ **Refactoring** – rename stałej zmienia wszystkie użycia  
✅ **Documentation** – kod sam się dokumentuje  
✅ **Error prevention** – kompilator wykrywa błędne wartości  
✅ **Communication** – nazwy odpowiadają terminologii biznesowej  

### ⚖️ Zalety i wady

#### Zalety
✅ **Czytelność** – jasna intencja kodu  
✅ **Centralizacja** – wszystkie wartości w jednym miejscu  
✅ **Type safety** – kompilator pomaga wykrywać błędy  
✅ **Autocomplete** – IDE podpowiada opcje  
✅ **Konsystencja** – te same wartości wszędzie  
✅ **Łatwe zmiany** – modyfikacja w jednym miejscu  

#### "Wady" (rzadkie)
❌ **Więcej kodu** – definicje stałych/enumów (ale to inwestycja!)  
❌ **Overkill** – dla wartości używanej raz (np. `const TWO = 2`)  

### ⚠️ Na co uważać?

#### 1. **Nie każda liczba to magic number**
```csharp
// ❌ Overkill:
const int ZERO = 0;
const int ONE = 1;
const int TWO = 2;

if (count > ZERO) { }  // Przesada!

// ✅ GOOD: Tylko wartości biznesowe
const int MAX_LOGIN_ATTEMPTS = 3;
const decimal VIP_DISCOUNT_RATE = 0.15m;

if (loginAttempts > MAX_LOGIN_ATTEMPTS) { }  // Sens biznesowy!
```

#### 2. **Nazywaj według znaczenia, nie wartości**
```csharp
// ❌ BAD: Nazwa opisuje wartość
const decimal TWENTY_THREE_PERCENT = 0.23m;
const int THIRTY_DAYS = 30;

// ✅ GOOD: Nazwa opisuje znaczenie
const decimal STANDARD_VAT_RATE = 0.23m;  // Może się zmienić!
const int TRIAL_PERIOD_DAYS = 30;
```

#### 3. **Uważaj na string comparisons**
```csharp
// ❌ BAD: Case sensitive strings
if (userType == "admin") { }  // "Admin" nie zadziała!

// ✅ GOOD: Enum lub case-insensitive
public enum UserType { Admin, User, Guest }

if (userType == UserType.Admin) { }  // Type-safe!

// Lub:
if (userType.Equals("admin", StringComparison.OrdinalIgnoreCase)) { }
```

#### 4. **Nie duplikuj wartości w różnych miejscach**
```csharp
// ❌ BAD: Duplikacja
public class OrderService
{
    const decimal VIP_DISCOUNT = 0.15m;
}

public class PricingService
{
    const decimal VIP_DISCOUNT = 0.15m;  // Duplikacja!
}

// ✅ GOOD: Centralizacja
public static class BusinessConstants
{
    public const decimal VIP_DISCOUNT_RATE = 0.15m;
}

// Wszędzie używamy BusinessConstants.VIP_DISCOUNT_RATE
```

#### 5. **Dokumentuj jednostki i kontekst**
```csharp
// ❌ BAD: Niejasne jednostki
const int MAX_SIZE = 500;  // KB? MB? Pixele?

// ✅ GOOD: Jasne jednostki
const int MAX_FILE_SIZE_KB = 500;
const int MAX_IMAGE_WIDTH_PX = 1920;
const int SESSION_TIMEOUT_MINUTES = 30;

// Lub używaj TypedConstants:
public static class Limits
{
    /// <summary>
    /// Maximum file upload size in kilobytes
    /// </summary>
    public const int MaxFileSizeKB = 500;
}
```

### 🚨 Najczęstsze pomyłki

#### 1. **Magic numbers w testach**
```csharp
// ❌ BAD: Magic numbers w testach
[Test]
public void Should_Apply_Discount()
{
    var price = service.CalculatePrice(100, true);
    Assert.AreEqual(85, price);  // Co to 85? Skąd się wzięło?
}

// ✅ GOOD: Nazwane wartości
[Test]
public void Should_Apply_VIP_Discount()
{
    const decimal BasePrice = 100m;
    const decimal ExpectedDiscount = 0.15m;
    const decimal ExpectedPrice = BasePrice * (1 - ExpectedDiscount);  // 85
    
    var price = service.CalculatePrice(BasePrice, isVip: true);
    
    Assert.AreEqual(ExpectedPrice, price);
}
```

#### 2. **String literals w wielu miejscach**
```typescript
// ❌ BAD: String literals wszędzie
if (status === "pending") { }
emailService.send("pending");
logger.log("Order status: pending");

// ✅ GOOD: Enum lub const
enum OrderStatus {
    Pending = "pending",
    Confirmed = "confirmed",
    Shipped = "shipped"
}

if (status === OrderStatus.Pending) { }
emailService.send(OrderStatus.Pending);
logger.log(`Order status: ${OrderStatus.Pending}`);
```

#### 3. **Obliczenia z magic numbers**
```csharp
// ❌ BAD: Niejasne obliczenia
var total = price * 1.23 + (weight * 0.5) * 1.15;
// Co to 1.23? 0.5? 1.15?

// ✅ GOOD: Nazwane stałe
const decimal VAT_RATE = 1.23m;  // 23% VAT
const decimal SHIPPING_RATE_PER_KG = 0.5m;
const decimal HANDLING_FEE_MULTIPLIER = 1.15m;  // 15% handling

var priceWithVat = price * VAT_RATE;
var shippingCost = weight * SHIPPING_RATE_PER_KG;
var totalShipping = shippingCost * HANDLING_FEE_MULTIPLIER;
var total = priceWithVat + totalShipping;
```

#### 4. **Hardcoded URLs, paths, keys**
```csharp
// ❌ BAD: Hardcoded values
var response = await httpClient.GetAsync("https://api.example.com/users");
File.WriteAllText("C:\\logs\\app.log", message);

// ✅ GOOD: Configuration
public class ApiSettings
{
    public string BaseUrl { get; set; }
    public string UsersEndpoint { get; set; }
}

// appsettings.json
{
    "Api": {
        "BaseUrl": "https://api.example.com",
        "UsersEndpoint": "/users"
    }
}

// Usage
var url = $"{_settings.Api.BaseUrl}{_settings.Api.UsersEndpoint}";
var response = await httpClient.GetAsync(url);
```

#### 5. **Bool flags zamiast enum**
```csharp
// ❌ BAD: Bool dla wielu opcji
public void ProcessOrder(bool isPriority, bool isInternational, bool requiresSignature)
{
    // Trudno zrozumieć: ProcessOrder(true, false, true)
}

// ✅ GOOD: Enum lub Flag enum
[Flags]
public enum ShippingOptions
{
    None = 0,
    Priority = 1,
    International = 2,
    RequiresSignature = 4
}

public void ProcessOrder(ShippingOptions options)
{
    // Czytelne: ProcessOrder(ShippingOptions.Priority | ShippingOptions.RequiresSignature)
}
```

### 💼 Kontekst biznesowy

**Scenariusz: Zmiana stawki VAT z 23% na 25%**

**Bez nazwanych stałych:**
- Szukaj we wszystkich plikach `0.23`, `23`, `1.23`
- Sprawdź każde wystąpienie czy to VAT czy inna wartość
- 50+ miejsc do zmiany
- Ryzyko pomyłki: 2 dni pracy + testy

**Z nazwanymi stałymi:**
```csharp
public static class TaxRates
{
    public const decimal STANDARD_VAT_RATE = 0.23m;
}

// Zmiana:
public const decimal STANDARD_VAT_RATE = 0.25m;

// 1 linia, propagacja automatyczna, 10 minut pracy!
```

### 📝 Podsumowanie

- **Magic numbers/strings** – wartości bez kontekstu, trudne do zrozumienia i utrzymania
- **Używaj** nazwanych stałych, enumów, configuration dla wartości biznesowych
- **Uważaj** na overkill (nie każda liczba to magic number), duplikację, brak jednostek
- **Najczęstsze błędy:** literals w testach, brak centralizacji, niejasne jednostki, bool flags
- **Korzyść biznesowa:** szybsze zmiany parametrów biznesowych, mniej błędów
