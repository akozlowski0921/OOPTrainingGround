# DRY (Don't Repeat Yourself)

## Przykład 1: Obliczanie statusu zamówienia

### Problem w BadExample
* **Duplikacja logiki:** Ta sama logika obliczająca status zamówienia jest skopiowana w 3 różnych miejscach
* **Ryzyko desynchronizacji:** Zmiana wymagań biznesowych wymaga aktualizacji w 3 miejscach
* **Błędy ludzkie:** Łatwo zapomnieć o aktualizacji jednej z kopii, co prowadzi do niespójności
* **Trudność w utrzymaniu:** Każda zmiana wymaga więcej czasu i wprowadza ryzyko błędów

### Rozwiązanie w GoodExample
* **Centralizacja logiki:** Logika biznesowa wydzielona do klasy `OrderStatusCalculator`
* **Single Source of Truth:** Jedna definicja reguł biznesowych
* **Łatwość zmian:** Modyfikacja wymagań wymaga zmiany tylko w jednym miejscu
* **Testowalność:** Łatwiej napisać testy jednostkowe dla wydzielonej logiki

## Przykład 2: Formatowanie cen

### Problem w BadExample2
* **Duplikacja formatowania:** Ta sama logika formatowania cen powielona w 4 różnych serwisach
* **Niespójność prezentacji:** Każda zmiana reguł formatowania wymaga aktualizacji w wielu miejscach
* **Ryzyko błędów:** Łatwo pominąć jedno miejsce podczas aktualizacji
* **Trudna konserwacja:** Utrzymanie spójnego formatowania jest czasochłonne

### Rozwiązanie w GoodExample2
* **Wydzielona klasa formatująca:** `PriceFormatter` jako jedyne źródło prawdy
* **Spójne formatowanie:** Wszystkie serwisy używają tej samej logiki
* **Łatwa modyfikacja:** Zmiana formatu w jednym miejscu
* **Dependency Injection:** Łatwe testowanie i wymiana implementacji

## Przykład 3: Walidacja adresów email

### Problem w BadExample3
* **Powielona walidacja:** Ta sama logika walidacji email skopiowana w 4 miejscach
* **Różne implementacje:** Każda kopia może mieć subtelne różnice prowadzące do niespójności
* **Trudna aktualizacja:** Zmiana reguł walidacji wymaga znalezienia wszystkich kopii
* **Podatność na błędy:** Ręczna walidacja zamiast sprawdzonych narzędzi (regex)

### Rozwiązanie w GoodExample3
* **Wydzielona walidacja:** Klasa `EmailValidator` z jedną implementacją
* **Używa regex:** Profesjonalne, sprawdzone rozwiązanie zamiast ręcznej logiki
* **Compiled regex:** Optymalizacja wydajności dla często używanej walidacji
* **Łatwe testowanie:** Jedna klasa do przetestowania zamiast wielu kopii

## Korzyści biznesowe

* **Spójność:** Wszędzie te same reguły biznesowe i prezentacja
* **Szybsze wdrażanie zmian:** Nowe reguły wdrażane w jednym miejscu
* **Mniej błędów:** Eliminacja ryzyka niezgodności między komponentami
* **Niższe koszty:** Mniej czasu na utrzymanie i rozwój

## Korzyści techniczne

* **Reużywalność:** Wydzielone klasy mogą być używane w wielu miejscach
* **Dependency Injection:** Łatwa wymiana implementacji lub mockowanie w testach
* **Separation of Concerns:** Każda klasa ma swoją odpowiedzialność
* **Testowalność:** Jedna implementacja = jeden zestaw testów
