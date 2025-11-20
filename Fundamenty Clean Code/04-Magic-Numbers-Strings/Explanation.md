# Magic Numbers i Magic Strings

## Problem w BadExample

* **Brak kontekstu:** Co oznacza `status == 4`? Co to jest `"admin"`?
* **Trudność w utrzymaniu:** Zmiana wartości wymaga przejrzenia całego kodu
* **Podatność na błędy:** Łatwo napisać `"Admin"` zamiast `"admin"` (case sensitivity)
* **Brak autocomplete:** IDE nie podpowie dostępnych opcji
* **Brak type safety:** Można przypisać dowolny int lub string

## Rozwiązanie w GoodExample

* **Enum dla statusów:** `OrderStatus.Cancelled` zamiast magicznej `4`
* **Enum dla typów:** `UserType.Admin` zamiast magicznego `"admin"`
* **Named constants:** `BusinessConstants.VipDiscountRate` zamiast magicznego `0.2`
* **Samodokumentujący się kod:** Nazwa mówi co wartość oznacza

## Korzyści biznesowe

* **Mniej błędów:** Type safety i autocomplete eliminują literówki
* **Łatwiejsze zmiany:** Zmiana wartości progowej w jednym miejscu
* **Lepsza komunikacja:** Nazwy odpowiadają terminologii biznesowej

## Korzyści techniczne

* **Autocomplete:** IDE podpowie wszystkie dostępne opcje
* **Type safety:** Kompilator wykryje błędne użycie
* **Łatwiejsze refaktoryzacje:** Rename propaguje się automatycznie
* **Czytelność:** Intencja kodu jest natychmiastowa
* **Centralizacja:** Wszystkie wartości biznesowe w jednym miejscu

## Praktyczne wskazówki

* Używaj `enum` dla ograniczonych zestawów wartości
* Używaj `const` dla wartości biznesowych (progi, stawki, limity)
* Nazywaj stałe według ich znaczenia biznesowego, nie wartości (`VipDiscountRate` nie `TwoZeroPercent`)
