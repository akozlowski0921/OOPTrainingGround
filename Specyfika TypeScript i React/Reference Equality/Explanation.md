# Reference Equality

## Problem w BadExample

* **Nowe referencje funkcji:** Funkcje takie jak `handleEdit`, `handleDelete` są tworzone na nowo przy każdym renderze komponentu rodzica.
* **Nowe referencje obiektów:** Obiekty jak `config` są tworzone na nowo przy każdym renderze.
* **React.memo nie działa:** Mimo użycia `React.memo`, komponenty potomne są re-renderowane, bo otrzymują "nowe" props (nowe referencje).
* **Inline funkcje:** Funkcje tworzone inline (np. `() => handleSomething(id)`) mają zawsze nową referencję.
* **Niepotrzebne re-rendery:** Kliknięcie przycisku "Increment Counter" powoduje re-render wszystkich komponentów, mimo że ich faktyczne dane się nie zmieniły.

## Problem z równością referencyjną w JavaScript

```typescript
// Dla prymitywów równość działa intuicyjnie
5 === 5 // true
'hello' === 'hello' // true

// Dla obiektów i funkcji porównywana jest referencja, nie zawartość
{} === {} // false - różne obiekty
[] === [] // false - różne tablice
(() => {}) === (() => {}) // false - różne funkcje
```

## Rozwiązanie w GoodExample

* **useCallback dla funkcji:** Zapewnia stabilną referencję funkcji między renderami, zmienia się tylko gdy zmienią się dependencies.
* **useMemo dla obiektów:** Cache'uje obiekty i zwraca tę samą referencję, jeśli dependencies się nie zmieniły.
* **Functional updates:** `setState(prev => ...)` pozwala uniknąć dodawania stanu do dependencies.
* **Custom comparison:** Dla complex objects można użyć custom comparison function w `React.memo`.

## Kiedy używać memoizacji?

**useCallback:**
* Funkcja jest przekazywana jako prop do zmemoizowanego komponentu
* Funkcja jest używana jako dependency w useEffect/useCallback
* Funkcja jest używana w event handlerach komponentów na dużych listach

**useMemo:**
* Obiekt/tablica są przekazywane jako prop do zmemoizowanego komponentu
* Wartość jest używana jako dependency w useEffect/useCallback
* Obliczenia są kosztowne

**React.memo:**
* Komponent często re-renderuje się z tymi samymi props
* Komponent jest na dużej liście elementów
* Renderowanie komponentu jest kosztowne

## Najczęstsze błędy

1. **Używanie React.memo bez useCallback/useMemo** - memo nie zadziała z nowymi referencjami
2. **Przedwczesna optymalizacja** - nie wszystko wymaga memoizacji, dodaje złożoność
3. **Niepełne dependencies w useCallback** - prowadzi do stale closures
4. **Memoizacja wszystkiego** - overhead większy niż korzyści dla prostych komponentów

## Korzyści

* **Wydajność:** Eliminacja niepotrzebnych re-renderów, szczególnie ważne dla dużych list.
* **Optymalizacja React.memo:** Memoizacja komponentów działa poprawnie tylko ze stabilnymi referencjami.
* **Przewidywalność:** Konsekwentne używanie useCallback/useMemo zwiększa czytelność kodu.

## Mierzenie przed optymalizacją

```typescript
// Użyj React DevTools Profiler lub console.log do zmierzenia
console.log('Component rendered');

// Nie optymalizuj bez potrzeby - prostota > przedwczesna optymalizacja
```

## Praktyczne wskazówki

1. Najpierw napisz działający kod bez optymalizacji
2. Zmierz wydajność (React DevTools Profiler)
3. Zoptymalizuj tylko problematyczne miejsca
4. Użyj ESLint rule `react-hooks/exhaustive-deps` do sprawdzania dependencies
