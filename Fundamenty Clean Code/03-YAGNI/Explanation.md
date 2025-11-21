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
