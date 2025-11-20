# Fail Fast vs Arrow Code

## Problem w BadExample

* **Arrow Code:** Kod przesunięty coraz bardziej w prawo przez zagnieżdżenia
* **Trudna nawigacja:** Ciężko śledzić wszystkie ścieżki wykonania
* **Kognitywne obciążenie:** Trzeba trzymać w głowie wiele poziomów kontekstu
* **Trudne debugowanie:** Ciężko umieścić breakpoint w odpowiednim miejscu

## Rozwiązanie w GoodExample

* **Early return:** Sprawdzamy warunki brzegowe na początku i natychmiast wychodzimy
* **Guard clauses:** Najpierw eliminujemy nieprawidłowe przypadki
* **Liniowy flow:** Kod czyta się od góry do dołu bez zagnieżdżeń
* **Poziom wcięcia:** Maksymalnie 1-2 poziomy zamiast 6+

## Korzyści biznesowe

* **Szybsze code review:** Łatwiej zrozumieć logikę biznesową
* **Mniej błędów:** Prostszy flow = mniej pomyłek w logice
* **Łatwiejsze modyfikacje:** Dodanie nowego warunku to kilka linijek na końcu

## Korzyści techniczne

* **Lepsza czytelność:** Każdy warunek jest widoczny od razu
* **Łatwiejsze testowanie:** Każda ścieżka jest wyraźnie określona
* **Mniejsza złożoność cyklomatyczna:** Mniej zagnieżdżeń = prostsza struktura
* **Debugowanie:** Łatwo umieścić breakpoint i śledzić flow

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
