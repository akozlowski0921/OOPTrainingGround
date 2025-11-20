# SOLID Principles - Kompletna seria edukacyjna

Witamy w kompleksowym przewodniku po zasadach SOLID w C#! Ta seria zawiera praktyczne przykłady pokazujące kontrast między złym kodem (legacy) a czystym, dobrze zaprojektowanym rozwiązaniem.

## 📚 Struktura

Każdy z pięciu folderów zawiera:
- **BadExample** - Realistyczny przykład złego kodu (antywzorce, legacy code)
- **GoodExample** - Ten sam problem rozwiązany zgodnie z zasadą SOLID
- **Explanation.md** - Szczegółowe wyjaśnienie:
  - Jaki problem występuje w złym kodzie?
  - Jak zasada SOLID rozwiązuje ten problem?
  - Korzyści biznesowe i techniczne
  - Kiedy i jak stosować daną zasadę

## 🎯 Zasady SOLID

### 1. [SRP - Single Responsibility Principle](./SRP/)
**Zasada Pojedynczej Odpowiedzialności**

> "Klasa powinna mieć tylko jeden powód do zmiany"

- **BadExample**: God Class wykonująca walidację, hashowanie, zapis do DB i logowanie
- **GoodExample**: Podział na wyspecjalizowane klasy (Validator, Hasher, Repository, Logger)
- **Korzyści**: Łatwiejsze utrzymanie, reużywalność, testowalność

**Przykładowy scenariusz**: System rejestracji użytkowników

---

### 2. [OCP - Open/Closed Principle](./OCP/)
**Zasada Otwarte-Zamknięte**

> "Kod powinien być otwarty na rozszerzenia, ale zamknięty na modyfikacje"

- **BadExample**: Potężny switch statement dla różnych formatów raportów
- **GoodExample**: Interfejs `IReportFormatter` z klasami per format
- **Korzyści**: Rozbudowa bez modyfikacji, brak ryzyka regresji, łatwość dodawania funkcji

**Przykładowy scenariusz**: Generator raportów w różnych formatach (PDF, HTML, CSV, XML, JSON)

---

### 3. [LSP - Liskov Substitution Principle](./LSP/)
**Zasada Podstawienia Liskov**

> "Obiekty klasy potomnej powinny móc zastąpić obiekty klasy bazowej bez zmiany poprawności programu"

- **BadExample**: Dziedziczenie Ptak → Pingwin, gdzie Pingwin.Fly() rzuca wyjątek
- **GoodExample**: Podział na interfejsy `IFlyable`, `ISwimmable`
- **Korzyści**: Prawdziwy polimorfizm, brak niespodzianek, bezpieczne dziedziczenie

**Przykładowy scenariusz**: System modelowania ptaków z różnymi zdolnościami

---

### 4. [ISP - Interface Segregation Principle](./ISP/)
**Zasada Segregacji Interfejsów**

> "Żaden klient nie powinien być zmuszony do zależności od metod, których nie używa"

- **BadExample**: Zbyt duży interfejs `ISmartDevice` wymuszający puste metody/wyjątki
- **GoodExample**: Małe, wyspecjalizowane interfejsy (IPrinter, IScanner, IFax, etc.)
- **Korzyści**: Klasy implementują tylko to, czego potrzebują, brak martwego kodu

**Przykładowy scenariusz**: System zarządzania urządzeniami biurowymi (drukarki, skanery, smartfony)

---

### 5. [DIP - Dependency Inversion Principle](./DIP/)
**Zasada Odwrócenia Zależności**

> "Moduły wysokopoziomowe nie powinny zależeć od modułów niskopoziomowych. Oba powinny zależeć od abstrakcji"

- **BadExample**: `new EmailSender()` w konstruktorze - silne powiązanie
- **GoodExample**: Dependency Injection przez interfejs `IMessageSender`
- **Korzyści**: Testowalność, elastyczność, łatwa zamiana implementacji

**Przykładowy scenariusz**: System powiadomień (email, SMS, push notifications)

---

## 🚀 Jak korzystać z tego repozytorium?

### 1. Dla osób uczących się:
1. Przeczytaj `Explanation.md` w każdym folderze
2. Przeanalizuj `BadExample` - zrozum problemy
3. Przestudiuj `GoodExample` - zobacz rozwiązanie
4. Uruchom przykłady i eksperymentuj!

### 2. Dla zespołów:
- Użyj przykładów podczas code review
- Pokazuj junior developerom kontrast między podejściami
- Dyskutuj o konkretnych przypadkach z waszego projektu

### 3. Dla rekruterów:
- Pytania rekrutacyjne z realnym kontekstem
- Praktyczne zadania do oceny kandydatów
- Materiał do rozmów o architekturze kodu

## 💻 Jak uruchomić przykłady?

Każdy plik z klasą `Program` można uruchomić:

```bash
# Dla BadExample (SRP)
dotnet run --project SOLID/SRP/BadExample/UserRegistration.cs

# Dla GoodExample (SRP)
dotnet run --project SOLID/SRP/GoodExample/UserRegistrationService.cs

# Analogicznie dla innych zasad
```

Lub użyj swojego IDE (Visual Studio, Rider, VS Code):
1. Otwórz plik .cs
2. Znajdź klasę `Program` z metodą `Main`
3. Uruchom

## 📖 Kolejność nauki

Zalecana kolejność dla początkujących:

1. **SRP** - Fundament, najłatwiejsza do zrozumienia
2. **ISP** - Logiczne rozszerzenie SRP na interfejsy
3. **LSP** - Uczy poprawnego dziedziczenia
4. **OCP** - Pokazuje jak projektować pod rozbudowę
5. **DIP** - Najbardziej zaawansowana, wymaga zrozumienia pozostałych

## 🎓 Dodatkowe materiały

### Powiązane wzorce projektowe:
- **Strategy Pattern** (OCP, DIP)
- **Factory Pattern** (OCP, DIP)
- **Adapter Pattern** (ISP, DIP)
- **Template Method** (OCP, LSP)

### Powiązane praktyki:
- **Dependency Injection** (DIP)
- **Test-Driven Development** (wszystkie zasady)
- **Clean Architecture** (wszystkie zasady)
- **Domain-Driven Design** (wszystkie zasady)

## 🔗 Kontekst w szerszym ekosystemie

SOLID to część większego obrazu:

```
Clean Code Principles
        ↓
    SOLID
        ↓
Design Patterns
        ↓
Architectural Patterns
```

## ⚠️ Ważne uwagi

1. **Nie przesadzaj**: SOLID to wytyczne, nie dogmaty
2. **Kontekst ma znaczenie**: Mały skrypt nie potrzebuje pełnego SOLID
3. **Ewolucja kodu**: Refaktoryzuj do SOLID gdy kod rośnie
4. **Pragmatyzm**: Czasem prostsze rozwiązanie jest lepsze

## 🤝 Wkład w projekt

Ten materiał powstał jako część Code Mastery Dojo. Jeśli masz sugestie:
- Otwórz Issue z pytaniami lub propozycjami
- Zaproponuj Pull Request z ulepszeniami
- Podziel się swoimi przykładami naruszenia/zastosowania SOLID

## 📝 Licencja

Ten materiał edukacyjny jest dostępny dla wszystkich chcących się uczyć programowania obiektowego i zasad SOLID.

---

**Powodzenia w nauce SOLID!** 🚀

Pamiętaj: Najlepszy sposób nauki to praktyka. Przeanalizuj swój kod i poszukaj miejsc, gdzie SOLID może pomóc!
