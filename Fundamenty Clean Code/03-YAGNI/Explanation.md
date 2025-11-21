# YAGNI (You Aren't Gonna Need It)

## Przykład 1: Niepotrzebne metody w UserService

### Problem w BadExample
* **Martwy kod:** 9 metod napisanych "na przyszłość", które nigdy nie są używane
* **Zwiększona złożoność:** Więcej kodu do utrzymania, testowania i rozumienia
* **Ryzyko dezinformacji:** Nowi deweloperzy myślą, że kod jest używany
* **Koszt utrzymania:** Każda zmiana w strukturze `User` wymaga aktualizacji nieużywanego kodu
* **Trudniejsze refaktoryzacje:** Więcej miejsc do modyfikacji przy zmianach

### Rozwiązanie w GoodExample
* **Minimalizm:** Tylko 2 metody faktycznie potrzebne biznesowi
* **Dodawanie na żądanie:** Nowe metody dodajemy dopiero gdy się pojawi realne wymaganie
* **Prostota:** Mniej kodu = łatwiej czytać, testować i utrzymywać

## Przykład 2: Nadmiarowy system pluginów dla formatów

### Problem w BadExample2
* **Over-engineering:** Złożony system pluginów dla 8 formatów, gdy używany jest tylko PDF
* **Zbędne słowniki:** Cztery słowniki do zarządzania formaterami, które nigdy nie są potrzebne
* **Martwe klasy:** 7 klas formaterów, które nigdy nie są używane
* **Złożone API:** Metody enable/disable, priority - wszystko zbędne

### Rozwiązanie w GoodExample2
* **Prosta implementacja:** Bezpośrednie użycie PdfFormatter
* **Brak abstrakcji:** Gdy będziemy potrzebować innych formatów, wtedy dodamy interfejs
* **Jasność intencji:** Kod pokazuje co faktycznie robimy (generujemy PDF)

## Przykład 3: Przewidywanie niepotrzebnych funkcji

### Problem w BadExample3
* **Funkcjonalność "na zapas":** Tagi, grupy, statystyki logowania - wszystko nieużywane
* **Pięć słowników:** Złożone struktury danych dla funkcji, których nikt nie potrzebuje
* **15+ metod martwych:** Kod do tagowania, grupowania, statystyk - zero użyć
* **Utrudniona konserwacja:** Każda zmiana w User wymaga aktualizacji wielu słowników

### Rozwiązanie w GoodExample3
* **Tylko wymagane:** Dwie metody: AddUser i GetUser
* **Możliwość rozbudowy:** Gdy pojawi się potrzeba tagów lub grup, dodamy je wtedy
* **Czytelność:** Natychmiastowe zrozumienie co klasa robi

## Korzyści biznesowe

* **Szybsze delivery:** Nie tracimy czasu na pisanie kodu, który nie jest potrzebny
* **Niższe koszty:** Mniej kodu do testowania i utrzymania
* **Elastyczność:** Gdy pojawi się wymaganie, projektujemy je właściwie, a nie używamy "zgadywanki"
* **Focus na value:** Koncentracja na rzeczywistych potrzebach biznesowych

## Korzyści techniczne

* **Łatwiejsze testy:** Testujemy tylko to, co jest używane
* **Mniejsza powierzchnia ataku:** Mniej kodu = mniej potencjalnych błędów
* **Lepsza czytelność:** Klasa pokazuje rzeczywiste potrzeby biznesowe
* **Szybsza refaktoryzacja:** Mniej kodu do modyfikacji przy zmianach
* **Mniejszy footprint:** Mniej pamięci, mniejsze binaria

## Kluczowa zasada

**Dodaj funkcjonalność gdy jest POTRZEBNA, nie gdy "może kiedyś będzie potrzebna"**

---

## 🎯 FAQ / INSIGHT

### Po co stosować YAGNI?

**YAGNI chroni przed:**
- **Przeprojektowaniem** – tworzeniem złożonych rozwiązań na problemy, które nie istnieją
- **Zmarnowanym czasem** – pisaniem kodu, który nigdy nie będzie użyty
- **Technicznym długiem** – martwy kod wymaga utrzymania, testowania, dokumentacji
- **Złymi decyzjami projektowymi** – zgadywanie przyszłości prowadzi do niewłaściwych abstrakcji

**YAGNI zapewnia:**
- **Prostotę** – kod robi tylko to, co jest potrzebne teraz
- **Elastyczność** – projektowanie pod rzeczywiste wymagania, nie spekulacje
- **Szybkość** – więcej czasu na rozwiązywanie prawdziwych problemów
- **Lepsze rozwiązania** – gdy wymaganie się pojawi, będziesz mieć więcej kontekstu

### W czym pomaga stosowanie YAGNI?

✅ **Redukcja złożoności** – mniej kodu = łatwiej zrozumieć system  
✅ **Lepsza testowalność** – tylko używany kod wymaga testów  
✅ **Szybsze iteracje** – mniej kodu do napisania i przetestowania  
✅ **Lepsza maintainability** – brak martwego kodu do utrzymania  
✅ **Oszczędność zasobów** – zespół pracuje nad tym, co przynosi wartość  
✅ **Łatwiejsza refaktoryzacja** – mniej kodu do przepisywania  

### ⚖️ Zalety i wady YAGNI

#### Zalety
✅ **Prostszy kod** – tylko niezbędne funkcje  
✅ **Mniejsze koszty** – brak utrzymania nieużywanego kodu  
✅ **Szybsze delivery** – skupienie na MVP i rzeczywistych potrzebach  
✅ **Lepsze decyzje projektowe** – projektujesz z pełnym kontekstem  
✅ **Mniej błędów** – mniej kodu = mniejsza powierzchnia do błędów  
✅ **Wyższa jakość** – więcej czasu na dopracowanie używanych funkcji  

#### Wady (jeśli źle rozumiane)
❌ **Ryzyko "short-sighted"** – ignorowanie oczywistych przyszłych potrzeb  
❌ **Trudności ze skalowaniem** – jeśli architektura jest zbyt sztywna  
❌ **Refaktoryzacja** – czasem łatwiej było zrobić od razu elastycznie  

**Ważne:** YAGNI NIE oznacza "pisz złą architekturę". Oznacza "nie implementuj funkcji, które nie są potrzebne".

### ⚠️ Na co uważać przy stosowaniu YAGNI?

#### 1. **Nie mylić YAGNI z brakiem projektowania**
```csharp
// ❌ ZŁE rozumienie YAGNI:
public class UserService
{
    // Wszystko w jednej metodzie bo "YAGNI"
    public void DoEverything(User user) { ... 100 linii ... }
}

// ✅ DOBRE rozumienie YAGNI:
public class UserService
{
    // Dobry design, ale bez nieużywanych metod
    public void CreateUser(User user) { ... }
    // Dodamy inne metody gdy będą potrzebne
}
```

#### 2. **Rozróżnij "nie potrzebne" od "oczywiste potrzeby"**
```csharp
// ❌ ZŁE: To jest OCZYWISTE, że będzie potrzebne
public interface IRepository
{
    void Add(T item);
    // Brak GetById - "YAGNI"? NIE! To podstawowa funkcja repository!
}

// ✅ DOBRE: Podstawowe operacje to nie YAGNI
public interface IRepository
{
    void Add(T item);
    T GetById(int id);
    void Update(T item);
    void Delete(int id);
    // Ale nie dodawaj: GetByTagsWithFilterAndPaginationAndSorting
}
```

#### 3. **Nie ignoruj ekstensibility points**
```csharp
// ❌ Sztywna implementacja bez extension points
public class PaymentProcessor
{
    public void Process(decimal amount)
    {
        // Hardcoded logika
        SendToCreditCardGateway(amount);
    }
}

// ✅ Podstawowa abstrakcja pozwala na przyszłą rozbudowę
public class PaymentProcessor
{
    private readonly IPaymentGateway _gateway;
    
    public PaymentProcessor(IPaymentGateway gateway)
    {
        _gateway = gateway;
    }
    
    public void Process(decimal amount)
    {
        _gateway.Process(amount);
    }
}
```

#### 4. **YAGNI ≠ brak dokumentacji lub testów**
```csharp
// ❌ ZŁE:
public void ProcessOrder(Order order)
{
    // Brak testów bo "YAGNI"? NIE!
}

// ✅ DOBRE:
/// <summary>
/// Processes order payment and inventory update
/// </summary>
public void ProcessOrder(Order order)
{
    // Jest używane = musi mieć testy i dokumentację!
}
```

### 🚨 Najczęstsze pomyłki

#### 1. **"Będę potrzebować 20 metod sortowania"**
```csharp
// ❌ BAD: Przewidywanie przyszłości
public class UserRepository
{
    public List<User> GetAllSortedByName() { }
    public List<User> GetAllSortedByAge() { }
    public List<User> GetAllSortedByEmail() { }
    public List<User> GetAllSortedByCreatedDate() { }
    // ... 16 więcej metod, które NIE SĄ używane
}

// ✅ GOOD: Dodaj gdy potrzeba
public class UserRepository
{
    public List<User> GetAll() { }
    // Gdy będzie wymaganie sortowania, dodaj:
    // public List<User> GetAllSorted(Expression<Func<User, object>> sortBy)
}
```

#### 2. **Over-engineered plugin system dla jednej funkcji**
```csharp
// ❌ BAD: Złożony system pluginów dla PDF
public interface IDocumentFormatter { }
public interface IFormatterPlugin { }
public class FormatterRegistry { }
public class PluginLoader { }
// ... 10 klas dla jednej funkcji

// ✅ GOOD: Prosta implementacja
public class PdfGenerator
{
    public byte[] GeneratePdf(Document doc) { }
}
// Gdy pojawi się nowy format, dodaj interfejs IDocumentGenerator
```

#### 3. **"Na pewno będziemy potrzebować cache"**
```csharp
// ❌ BAD: Przedwczesna optymalizacja
public class ProductService
{
    private readonly ICacheProvider _cache;
    private readonly IDistributedCache _distCache;
    private readonly IMemoryCache _memCache;
    // Złożony system cache gdy nie ma problemów z wydajnością!
}

// ✅ GOOD: Dodaj cache gdy pojawią się problemy z performance
public class ProductService
{
    public Product GetById(int id)
    {
        return _repository.GetById(id);
    }
    // Zmierz performance, dopiero wtedy dodaj cache!
}
```

#### 4. **Dodawanie "future-proof" abstrakcji**
```csharp
// ❌ BAD: Abstrakcja dla jednej implementacji
public interface IEmailConfigurationProviderFactoryStrategy { }
public class DefaultEmailConfigurationProviderFactoryStrategy 
    : IEmailConfigurationProviderFactoryStrategy
{
    // Tylko jedna implementacja, nigdy nie będzie więcej
}

// ✅ GOOD: Bezpośrednia implementacja
public class EmailConfiguration
{
    public string SmtpServer { get; set; }
    public int Port { get; set; }
}
// Dodaj abstrakcję gdy będzie druga implementacja!
```

#### 5. **"Ale co jeśli klient zechce..."**
```csharp
// ❌ BAD: Spekulacja bez potwierdzenia
public class Order
{
    // 30 pól "na wszelki wypadek"
    public string? AlternativeShippingAddress2 { get; set; }
    public string? SecondaryBillingContact { get; set; }
    public string? InternalNotes { get; set; }
    public string? CustomerPreferences { get; set; }
    // Żadne z tych nie jest używane!
}

// ✅ GOOD: Tylko potrzebne pola
public class Order
{
    public string ShippingAddress { get; set; }
    public string BillingAddress { get; set; }
}
// Dodaj pola gdy klient o nie poprosi!
```

### 💡 Jak stosować YAGNI w praktyce?

#### Pytaj przed implementacją:
1. **"Czy ta funkcja jest TERAZ używana?"** – Nie? Nie implementuj.
2. **"Czy mamy konkretne wymaganie biznesowe?"** – Nie? Nie implementuj.
3. **"Czy ktoś płaci za tę funkcję?"** – Nie? Nie implementuj.
4. **"Czy brak tej funkcji blokuje rozwój?"** – Nie? Nie implementuj.

#### Zbalansuj YAGNI z dobrym designem:
```csharp
// ✅ Dobry balans:
public interface IPaymentGateway  // Podstawowa abstrakcja
{
    Task<PaymentResult> ProcessAsync(decimal amount);
}

public class CreditCardGateway : IPaymentGateway
{
    public Task<PaymentResult> ProcessAsync(decimal amount)
    {
        // Jedyna implementacja na razie, ale design pozwala na rozbudowę
    }
}

// NIE dodawaj metod jak:
// Task<PaymentResult> ProcessWithRetryAndLoggingAndMetricsAndCaching(...)
// Dopóki nie są potrzebne!
```

### 🎓 YAGNI a inne zasady

**YAGNI + KISS:**
- KISS: Pisz prosty kod
- YAGNI: Pisz tylko potrzebny kod
- Razem: Pisz prosty, potrzebny kod

**YAGNI + DRY:**
- DRY: Nie duplikuj kodu
- YAGNI: Nie pisz kodu "na zapas"
- Razem: Duplikację usuwaj gdy się pojawi, nie "na przyszłość"

**YAGNI + TDD:**
- TDD: Pisz test → implementuj minimum
- YAGNI: Nie implementuj więcej niż test wymaga
- Razem: Idealne combo dla minimalnego, działającego kodu

### 📊 Mierzenie YAGNI

**Metryki "martwego kodu":**
- Code coverage < 80% → dużo nieużywanego kodu?
- Metody wywołane zero razy (code profiling)
- Features nigdy nie klikane (analytics)
- Branches nigdy nie wykonane (coverage reports)

**Narzędzia:**
- ReSharper: "Unused code" analysis
- SonarQube: Dead code detection
- dotCover / Visual Studio: Code coverage
- Application Insights: Feature usage tracking
