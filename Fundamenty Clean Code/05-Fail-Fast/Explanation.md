# Fail Fast vs Arrow Code

## Przykład 1: Dostęp do dokumentów

### Problem w BadExample
* **Arrow Code:** Kod przesunięty coraz bardziej w prawo przez zagnieżdżenia
* **Trudna nawigacja:** Ciężko śledzić wszystkie ścieżki wykonania
* **Kognitywne obciążenie:** Trzeba trzymać w głowie wiele poziomów kontekstu
* **Trudne debugowanie:** Ciężko umieścić breakpoint w odpowiednim miejscu

### Rozwiązanie w GoodExample
* **Early return:** Sprawdzamy warunki brzegowe na początku i natychmiast wychodzimy
* **Guard clauses:** Najpierw eliminujemy nieprawidłowe przypadki
* **Liniowy flow:** Kod czyta się od góry do dołu bez zagnieżdżeń
* **Poziom wcięcia:** Maksymalnie 1-2 poziomy zamiast 6+

## Przykład 2: Przetwarzanie płatności

### Problem w BadExample2
* **10+ poziomów zagnieżdżenia:** Kod schodzi tak głęboko, że trudno go śledzić
* **Duplikacja warunków:** Te same sprawdzenia w różnych miejscach
* **Wszystkie błędy na końcu:** Logika błędów rozrzucona po całej funkcji
* **Trudna konserwacja:** Dodanie nowego typu płatności wymaga modyfikacji głębokich zagnieżdżeń

### Rozwiązanie w GoodExample2
* **Walidacja na początku:** Wszystkie podstawowe sprawdzenia jako guard clauses
* **Switch statement:** Czytelny routing do specjalizowanych metod
* **Wydzielone metody:** Każdy typ płatności ma swoją metodę
* **Spłaszczona struktura:** Maksymalnie 2 poziomy wcięcia

## Przykład 3: Rezerwacja hotelowa

### Problem w BadExample3
* **12+ poziomów zagnieżdżeń:** Niemożliwe do śledzenia bez przewijania
* **Logika zagubiona w środku:** Główna logika biznesowa ukryta między warunkami
* **Trudne testowanie:** Trzeba przygotować skomplikowane kombinacje danych
* **Ciężko znaleźć gdzie jest błąd:** Każdy if może być źródłem problemu

### Rozwiązanie in GoodExample3
* **Sekwencyjne guard clauses:** Każdy warunek eliminacyjny osobno
* **Wydzielone metody pomocnicze:** `calculateNights()`, `isDepositWaived()`
* **Nazwane stałe:** Jasne progi i limity
* **Happy path na końcu:** Sukces to ostatnia linijka, nie zagnieżdżenie

## Korzyści biznesowe

* **Szybsze code review:** Łatwiej zrozumieć logikę biznesową
* **Mniej błędów:** Prostszy flow = mniej pomyłek w logice
* **Łatwiejsze modyfikacje:** Dodanie nowego warunku to kilka linijek na początku
* **Lepsza komunikacja:** Kod czyta się jak lista wymagań biznesowych

## Korzyści techniczne

* **Lepsza czytelność:** Każdy warunek jest widoczny od razu
* **Łatwiejsze testowanie:** Każda ścieżka jest wyraźnie określona
* **Mniejsza złożoność cyklomatyczna:** Mniej zagnieżdżeń = prostsza struktura
* **Debugowanie:** Łatwo umieścić breakpoint i śledzić flow
* **Konserwacja:** Dodanie nowej walidacji nie wymaga modyfikacji istniejącego kodu

## Kluczowa zasada

**Sprawdzaj warunki błędu najpierw i wychodź natychmiast. Pozytywny flow na końcu.**

## Wzorzec

```typescript
// Najpierw warunki brzegowe (null checks, validation)
if (!input) return error;

// Potem warunki biznesowe (fail fast)
if (someCondition) return result;
if (anotherCondition) return result;

// Happy path na końcu
return successResult;
```
