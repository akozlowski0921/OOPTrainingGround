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
