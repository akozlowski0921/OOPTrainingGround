# DRY (Don't Repeat Yourself)

## Problem w BadExample

* **Duplikacja logiki:** Ta sama logika obliczająca status zamówienia jest skopiowana w 3 różnych miejscach
* **Ryzyko desynchronizacji:** Zmiana wymagań biznesowych wymaga aktualizacji w 3 miejscach
* **Błędy ludzkie:** Łatwo zapomnieć o aktualizacji jednej z kopii, co prowadzi do niespójności
* **Trudność w utrzymaniu:** Każda zmiana wymaga więcej czasu i wprowadza ryzyko błędów

## Rozwiązanie w GoodExample

* **Centralizacja logiki:** Logika biznesowa wydzielona do klasy `OrderStatusCalculator`
* **Single Source of Truth:** Jedna definicja reguł biznesowych
* **Łatwość zmian:** Modyfikacja wymagań wymaga zmiany tylko w jednym miejscu
* **Testowalność:** Łatwiej napisać testy jednostkowe dla wydzielonej logiki

## Korzyści biznesowe

* **Spójność:** Wszędzie ten sam status dla tego samego zamówienia
* **Szybsze wdrażanie zmian:** Nowe reguły biznesowe wdrażane w jednym miejscu
* **Mniej błędów:** Eliminacja ryzyka niezgodności między komponentami

## Korzyści techniczne

* **Reużywalność:** Klasa `OrderStatusCalculator` może być używana w innych miejscach
* **Dependency Injection:** Łatwa wymiana implementacji lub mockowanie w testach
* **Separation of Concerns:** Każda klasa ma swoją odpowiedzialność
