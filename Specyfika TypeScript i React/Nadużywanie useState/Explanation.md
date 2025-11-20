# Nadużywanie useState

## Problem w BadExample

* **Redundantny stan:** Przechowywanie wartości, które można łatwo wyliczyć z istniejącego stanu (totalPrice, totalItems, hasExpensiveItems).
* **Ryzyko desynchronizacji:** Musimy ręcznie synchronizować wiele stanów używając useEffect - łatwo o błędy.
* **Niepotrzebna złożoność:** Więcej kodu do utrzymania, więcej miejsc gdzie mogą wystąpić błędy.
* **Trudniejsze debugowanie:** Trudniej śledzić, który stan jest "source of truth" a który jest pochodny.
* **Więcej re-renderów:** Każda zmiana products powoduje 4 separate update'y stanu (products + 3 derived values).

## Rozwiązanie w GoodExample

* **Derived state:** Wartości obliczane bezpośrednio podczas renderowania - zawsze aktualne, zawsze spójne.
* **useMemo dla optymalizacji:** Jeśli obliczenia są kosztowne, `useMemo` cache'uje wynik i przelicza tylko gdy zależności się zmienią.
* **Minimalizacja stanu:** Przechowujemy tylko rzeczywiste źródła danych (products, searchTerm, showActiveOnly).
* **Brak synchronizacji:** Nie potrzeba useEffect do synchronizowania stanów - wszystko działa automatycznie.

## Kiedy używać useMemo?

**Używaj useMemo gdy:**
* Obliczenia są kosztowne (iteracja przez duże listy, złożone transformacje)
* Wartość jest przekazywana jako prop do zmemoizowanego komponentu
* Wartość jest używana jako dependency w useEffect lub useCallback

**Nie używaj useMemo gdy:**
* Obliczenia są proste i szybkie (pojedyncze operacje, małe tablice)
* Przedwczesna optymalizacja - najpierw zmierz wydajność

## Zasada "Single Source of Truth"

1. **Przechowuj w state tylko źródłowe dane** - to co faktycznie się zmienia przez akcje użytkownika lub API
2. **Obliczaj wszystko inne** - wartości pochodne mogą być derived podczas renderowania
3. **Optymalizuj tylko gdy to konieczne** - useMemo/useCallback dla kosztownych operacji

## Korzyści

* **Prostota:** Mniej stanu = mniej miejsc na błędy
* **Spójność:** Derived values są zawsze zgodne ze źródłem danych
* **Łatwość utrzymania:** Nie trzeba pamiętać o synchronizacji wielu stanów
* **Lepsza wydajność:** Mniej setState = mniej re-renderów
* **Czytelność:** Jasne co jest źródłem danych, a co jest obliczane

## Przykłady derived state

```typescript
// ❌ Zły sposób
const [firstName, setFirstName] = useState('Jan');
const [lastName, setLastName] = useState('Kowalski');
const [fullName, setFullName] = useState('Jan Kowalski'); // redundantny!

// ✅ Dobry sposób
const [firstName, setFirstName] = useState('Jan');
const [lastName, setLastName] = useState('Kowalski');
const fullName = `${firstName} ${lastName}`; // derived on the fly
```
