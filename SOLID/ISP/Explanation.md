# ISP - Interface Segregation Principle (Zasada Segregacji Interfejsów)

## 🔴 Problem w BadExample

Zbyt duży interfejs `ISmartDevice` wymusza implementację WSZYSTKICH metod:

```csharp
public interface ISmartDevice
{
    void Print(string document);
    void Scan(string document);
    void Fax(string document);
    void Copy(string document);
    void SendEmail(string to, string subject, string body);
    void MakeCall(string number);
    void BrowseInternet(string url);
}
```

### Dlaczego to źle?

1. **SimplePrinter** musi implementować 7 metod, ale używa tylko 1 (Print)
   - 6 metod rzuca `NotImplementedException`
   - Ogromny "interfejs martwy"

2. **Smartphone** nie jest drukarką, ale musi mieć metodę `Print()`
   - Wymuszanie niepotrzebnych zależności

3. **Naruszenie SRP i LSP**:
   - Klasa ma odpowiedzialności, których nie potrzebuje
   - Nie można bezpiecznie użyć polimorfizmu (wyjątki w runtime)

4. **Trudność w utrzymaniu**:
   - Każda zmiana interfejsu wymusza zmiany we WSZYSTKICH klasach
   - Nawet jeśli zmiana ich nie dotyczy

### Symptomy "Fat Interface":

- Klasy implementujące interfejs mają puste metody
- Wiele `throw new NotImplementedException()`
- Dokumentacja typu "Nie używaj metody X w klasie Y"
- Obawy przed dodaniem nowej metody do interfejsu

## ✅ Rozwiązanie w GoodExample

### Podział na małe, wyspecjalizowane interfejsy:

```csharp
public interface IPrinter { void Print(string document); }
public interface IScanner { void Scan(string document); }
public interface IFax { void Fax(string document); }
public interface ICopier { void Copy(string document); }
public interface IEmailSender { void SendEmail(...); }
public interface IPhone { void MakeCall(string number); }
public interface IWebBrowser { void BrowseInternet(string url); }
```

### Implementacje wybierają tylko potrzebne interfejsy:

- `SimplePrinter : IPrinter` ← tylko drukowanie
- `MultiFunctionPrinter : IPrinter, IScanner, IFax, ICopier` ← wszystko biurowe
- `Smartphone : IScanner, IEmailSender, IPhone, IWebBrowser` ← funkcje mobilne
- `PhotoScanner : IScanner` ← tylko skanowanie

### Korzyści:

1. **Brak martwego kodu**: Każda klasa implementuje tylko to, czego używa
2. **Brak wyjątków**: Nie ma pustych implementacji ani NotImplementedException
3. **Flexibility**: Łatwo łączyć interfejsy w różne kombinacje
4. **Łatwiejsze utrzymanie**: Zmiana w `IPrinter` nie wpływa na `IPhone`
5. **Lepsze testowanie**: Mocki muszą implementować tylko potrzebne interfejsy

## 💼 Kontekst Biznesowy

### Scenariusz: System do zarządzania urządzeniami biurowymi

**BadExample** (❌):
```csharp
// Nowy wymóg: dodaj funkcję drukowania 3D
public interface ISmartDevice
{
    // ... wszystkie poprzednie metody ...
    void Print3D(string model);  // ← nowa metoda
}
```

Efekt:
- **Wszystkie** 50 klas muszą dodać tę metodę
- 48 z nich rzuci `NotImplementedException`
- Tydzień pracy + testy + code review
- Ryzyko błędów w każdej klasie

**GoodExample** (✅):
```csharp
// Nowy interfejs dla drukarek 3D
public interface I3DPrinter
{
    void Print3D(string model);
}

// Nowa klasa
public class ThreeDPrinter : I3DPrinter
{
    public void Print3D(string model) { ... }
}
```

Efekt:
- Zero zmian w istniejących klasach
- Jedna nowa klasa
- Godzina pracy
- Zero ryzyka regresji

### Oszczędności:

- Czas: 40 godz → 1 godz
- Ryzyko: wysokie → zero
- Koszty: tydzień zespołu → jedna osoba przez godzinę

## 🎯 Kiedy stosować ISP?

**Zawsze** gdy projektujesz interfejsy! Szczególnie ważne dla:

- **Plugin/extension systems**: Różne pluginy potrzebują różnych możliwości
- **API clients**: Różni klienci używają różnych części API
- **Repository patterns**: Read-only vs full CRUD
- **Service layers**: Różne poziomy dostępu
- **IoT devices**: Każde urządzenie ma inne funkcje

## 📏 Jak rozpoznać naruszenie ISP?

Czerwone flagi:
- ❌ Interfejs ma więcej niż 5-7 metod
- ❌ Większość implementacji rzuca `NotImplementedException`
- ❌ Puste implementacje metod interfejsu
- ❌ Komentarze "Nie używane w tej klasie"
- ❌ Strach przed dodaniem metody do interfejsu (wszystkie klasy będą płakać)
- ❌ Klasy implementujące tylko 20% metod interfejsu

## 🔧 Wzorce wspierające ISP:

1. **Role Interfaces**: Interfejsy według ról, nie klas
   - `IReadable`, `IWritable` zamiast `IRepository`

2. **Adapter Pattern**: Adaptuj duży interfejs do małego
   
3. **Facade Pattern**: Ukryj złożoność za prostym interfejsem

4. **Composition**: Składaj obiekty z wielu małych interfejsów

## 💡 Złota zasada ISP

> "Żaden klient nie powinien być zmuszony do zależności od metod, których nie używa"

W praktyce:
- Interfejs powinien reprezentować **jedną spójną zdolność**
- Jeśli klasa implementuje 50% metod → podziel interfejs
- Lepiej 5 małych interfejsów niż 1 duży

## 🎓 ISP vs SRP

**SRP** (Single Responsibility):
- Dotyczy **klas** - jedna klasa = jedna odpowiedzialność

**ISP** (Interface Segregation):
- Dotyczy **interfejsów** - jeden interfejs = jedna zdolność/rola

Razem tworzą potężne combo dla czystego kodu!

## 🧪 Pytania testowe dla interfejsu:

1. Czy każda klasa implementująca ten interfejs używa **wszystkich** metod?
   - Jeśli NIE → naruszenie ISP

2. Czy mogę opisać interfejs **jednym** rzeczownikiem lub rolą?
   - `IPrinter` ✅, `ISmartDeviceWithEverything` ❌

3. Czy dodanie metody wymusi zmiany w klasach, które jej nie potrzebują?
   - Jeśli TAK → naruszenie ISP

## 📊 Przykład z życia:

**Zły interfejs** (naruszenie ISP):
```csharp
interface IUser
{
    void Login();
    void Logout();
    void PostComment();
    void ModerateContent();
    void ManageUsers();
    void AccessAdminPanel();
}
```

**Dobre interfejsy** (zgodne z ISP):
```csharp
interface IAuthenticable { void Login(); void Logout(); }
interface ICommenter { void PostComment(); }
interface IModerator : ICommenter { void ModerateContent(); }
interface IAdmin : IModerator { void ManageUsers(); void AccessAdminPanel(); }
```

Teraz:
- Zwykły użytkownik: `IAuthenticable, ICommenter`
- Moderator: `IAuthenticable, IModerator`
- Admin: `IAuthenticable, IAdmin`

Każdy ma tylko to, czego potrzebuje!
