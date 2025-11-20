# YAGNI (You Aren't Gonna Need It)

## Problem w BadExample

* **Martwy kod:** 9 metod napisanych "na przyszłość", które nigdy nie są używane
* **Zwiększona złożoność:** Więcej kodu do utrzymania, testowania i rozumienia
* **Ryzyko dezinformacji:** Nowi deweloperzy myślą, że kod jest używany
* **Koszt utrzymania:** Każda zmiana w strukturze `User` wymaga aktualizacji nieużywanego kodu
* **Trudniejsze refaktoryzacje:** Więcej miejsc do modyfikacji przy zmianach

## Rozwiązanie w GoodExample

* **Minimalizm:** Tylko 2 metody faktycznie potrzebne biznesowi
* **Dodawanie na żądanie:** Nowe metody dodajemy dopiero gdy się pojawi realne wymaganie
* **Prostota:** Mniej kodu = łatwiej czytać, testować i utrzymywać

## Korzyści biznesowe

* **Szybsze delivery:** Nie tracimy czasu na pisanie kodu, który nie jest potrzebny
* **Niższe koszty:** Mniej kodu do testowania i utrzymania
* **Elastyczność:** Gdy pojawi się wymaganie, projektujemy je właściwie, a nie używamy "zgadywanki"

## Korzyści techniczne

* **Łatwiejsze testy:** Testujemy tylko to, co jest używane
* **Mniejsza powierzchnia ataku:** Mniej kodu = mniej potencjalnych błędów
* **Lepsza czytelność:** Klasa pokazuje rzeczywiste potrzeby biznesowe
* **Refaktoryzacja:** Mniej kodu do modyfikacji przy zmianach

## Kluczowa zasada

**Dodaj funkcjonalność gdy jest POTRZEBNA, nie gdy "może kiedyś będzie potrzebna"**
