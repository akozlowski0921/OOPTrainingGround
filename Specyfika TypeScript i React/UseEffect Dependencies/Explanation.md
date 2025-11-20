# UseEffect Dependencies

## Problem w BadExample

* **Stale Closure:** Zmienna `retryCount` jest używana wewnątrz useEffect, ale nie jest dodana do tablicy dependencies. Funkcja effect "zamyka" początkową wartość zmiennej i nigdy nie widzi jej aktualizacji.
* **Brakująca funkcja w dependencies:** Funkcja `performSearch` jest używana w useEffect, ale nie jest w tablicy zależności, co powoduje że effect zawsze używa starej wersji funkcji ze starymi wartościami closure.
* **Nieprzewidywalne zachowanie:** Komponent może nie reagować na zmiany stanu, które powinny wywołać ponowne wykonanie effectu.

## Rozwiązanie w GoodExample

* **Kompletna tablica dependencies:** Wszystkie zmienne i funkcje używane w useEffect są dodane do tablicy zależności.
* **useCallback dla funkcji:** Funkcje używane w useEffect są opakowane w `useCallback`, co zapewnia stabilną referencję i kontrolowaną re-kreację.
* **Functional updates:** Użycie `setState(prev => ...)` pozwala uniknąć dodawania zmiennej stanu do dependencies, gdy potrzebujemy tylko jej aktualnej wartości.

## Korzyści

* **Przewidywalność:** Effect wykonuje się dokładnie wtedy, gdy powinien - po zmianie którejkolwiek ze swoich zależności.
* **Brak stale closures:** Zawsze mamy dostęp do aktualnych wartości zmiennych.
* **Łatwiejsze debugowanie:** ESLint z regułą `exhaustive-deps` automatycznie wykrywa problemy.
* **Mniej bugów:** Eliminacja trudnych do wyśledzenia problemów z niewłaściwym odświeżaniem danych.

## Najważniejsze zasady

1. **Zawsze dodawaj wszystkie dependencies** używane wewnątrz useEffect
2. **Używaj useCallback** dla funkcji, które są dependencies dla useEffect
3. **Używaj functional updates** (`setState(prev => ...)`) gdy potrzebujesz tylko aktualnej wartości stanu
4. **Włącz ESLint regułę** `react-hooks/exhaustive-deps` - ostrzeże Cię o brakujących dependencies
