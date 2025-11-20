# Specyfika TypeScript i React

## 🎯 Cel sekcji

Ta sekcja zawiera praktyczne przykłady typowych problemów i ich rozwiązań specyficznych dla ekosystemu **TypeScript** i **React**. Każdy przykład ilustruje powszechne błędy i pokazuje jak je naprawić, używając najlepszych praktyk i wzorców React.

## 📚 Tematy

### 1. UseEffect Dependencies
**Problem:** Stale closure i brakujące dependencies w useEffect prowadzą do nieprzewidywalnego zachowania.

**Rozwiązanie:** Kompletna tablica dependencies, useCallback dla funkcji, functional updates dla stanu.

**Najważniejsze:**
* Wszystkie zmienne i funkcje używane w useEffect muszą być w dependencies
* useCallback zapewnia stabilną referencję funkcji
* ESLint rule `exhaustive-deps` automatycznie wykrywa problemy

---

### 2. Prop Drilling
**Problem:** Przekazywanie props przez wiele poziomów komponentów, które ich nie używają.

**Rozwiązanie:** Context API dla globalnego stanu, Component Composition dla lokalnego.

**Najważniejsze:**
* Context API dla danych używanych w wielu miejscach (theme, auth, language)
* Component Composition dla prostszych przypadków
* Custom hooks dla wygodnego dostępu do Context

---

### 3. Nadużywanie useState
**Problem:** Przechowywanie redundantnego stanu - wartości, które można wyliczyć z istniejącego stanu.

**Rozwiązanie:** Derived state - obliczanie wartości on the fly, useMemo dla optymalizacji kosztownych obliczeń.

**Najważniejsze:**
* Przechowuj tylko źródłowe dane w state
* Obliczaj wartości pochodne podczas renderowania
* useMemo tylko dla kosztownych operacji

---

### 4. Reference Equality
**Problem:** React.memo nie działa przez nowe referencje funkcji i obiektów przy każdym renderze.

**Rozwiązanie:** useCallback dla funkcji, useMemo dla obiektów, aby zapewnić stabilne referencje.

**Najważniejsze:**
* useCallback dla funkcji przekazywanych do zmemoizowanych komponentów
* useMemo dla obiektów/tablic przekazywanych jako props
* React.memo dla komponentów na dużych listach lub z kosztownym renderowaniem

---

### 5. Type Guards i Union Types
**Problem:** Rzutowanie typów "na siłę" przez `as`, używanie `any` - utrata bezpieczeństwa typów.

**Rozwiązanie:** Discriminated Unions, Type Guards, type narrowing - pełne bezpieczeństwo typów.

**Najważniejsze:**
* Discriminated unions z polem discriminant (type, status, kind)
* Custom type guards z predicate functions
* Unikaj `as` i `any` - używaj union types

---

## 🎓 Jak korzystać z tej sekcji?

1. **Zacznij od BadExample** - Zobacz typowy problem i spróbuj zrozumieć dlaczego jest problematyczny.
2. **Przeanalizuj GoodExample** - Zobacz jak rozwiązać problem używając best practices.
3. **Przeczytaj Explanation.md** - Zrozum dlaczego rozwiązanie jest lepsze i jakie ma korzyści.
4. **Porównaj kod** - Zobacz różnice między złym a dobrym kodem.
5. **Zastosuj w praktyce** - Użyj tych wzorców w swoich projektach.

## 💡 Najważniejsze zasady React/TypeScript

### React Hooks
* **useEffect:** Zawsze pełna lista dependencies, useCallback dla funkcji
* **useState:** Minimalizuj stan, obliczaj wartości pochodne
* **useMemo/useCallback:** Używaj dla optymalizacji, nie przedwcześnie

### TypeScript
* **Type Safety:** Unikaj `any`, używaj union types i type guards
* **Type Narrowing:** Pozwól TypeScript automatycznie zawężać typy
* **Discriminated Unions:** Pattern dla stanów i wariantów danych

### Wydajność
* **React.memo:** Tylko z useCallback/useMemo
* **Reference Equality:** Stabilne referencje dla props zmemoizowanych komponentów
* **Derived State:** Zamiast redundantnego stanu

## 🔗 Powiązane tematy

* **Clean Code Basics:** DRY, KISS, YAGNI stosowane w React
* **SOLID:** Szczególnie Single Responsibility i Dependency Inversion w React
* **Design Patterns:** Observer (useContext), Strategy (custom hooks), Composite (component composition)

## 📖 Dodatkowe zasoby

* [React Documentation](https://react.dev/)
* [TypeScript Handbook](https://www.typescriptlang.org/docs/handbook/intro.html)
* [React TypeScript Cheatsheet](https://react-typescript-cheatsheet.netlify.app/)
* [Kent C. Dodds Blog](https://kentcdodds.com/blog) - React best practices

---

**Uwaga:** Przykłady w tej sekcji używają React 18+ i TypeScript 5+. Niektóre wzorce mogą wymagać dostosowania dla starszych wersji.
