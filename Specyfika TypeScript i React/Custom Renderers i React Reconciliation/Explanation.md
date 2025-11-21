# Custom Renderers i React Reconciliation

## Problem w BadExample

### BadExample.tsx - Naiwny renderer
* **Brak właściwego renderer'a:** Próba renderowania React komponentów bez używania `react-reconciler` - hooks nie działają.
* **Brak lifecycle management:** Komponenty są wywoływane jako zwykłe funkcje, co omija cały mechanizm React.
* **Brak Context API:** Nie ma mechanizmu do propagacji context przez drzewo komponentów.
* **Brak state updates:** Brak mechanizmu do re-renderowania gdy stan się zmienia.

### BadExample2.tsx - Bezpośrednia manipulacja DOM
* **Omijanie React Fiber:** Bezpośrednia manipulacja DOM zamiast pozwolić React zarządzać przez Fiber architecture.
* **Niespójna synchronizacja:** Stan i DOM są aktualizowane niezależnie, co prowadzi do race conditions.
* **Hakowanie internals:** Próba dostępu do `_reactInternalInstance` i manipulacji Fiber node - bardzo niebezpieczne.
* **Ręczna reconciliation:** Próba implementacji własnej logiki reconciliation zamiast użyć React.

### BadExample3.tsx - Złe keys i reconciliation
* **Index jako key:** Używanie array index jako key w dynamicznych listach - React myli elementy.
* **Niestabilne keys:** Keys bazujące na `Math.random()` powodują niepotrzebne unmount/remount.
* **Duplicate keys:** Nieunikaln keys łamią mechanizm reconciliation.
* **Brak keys w fragmentach:** Fragmenty bez keys uniemożliwiają śledzenie zmian.
* **Mixed controlled/uncontrolled:** Zmiana z uncontrolled na controlled component powoduje błędy.

## Rozwiązanie w GoodExample

### GoodExample.tsx - Właściwy custom renderer
* **react-reconciler:** Pełna implementacja custom renderer'a używając oficjalnego API.
* **Lifecycle support:** Wszystkie React features działają: hooks, context, effects.
* **Proper reconciliation:** React Fiber automatycznie zarządza updates i reconciliation.
* **Performance:** React optymalizuje rendering przez batching i prioritization.

### GoodExample2.tsx - Współpraca z React Fiber
* **State-driven updates:** Wszystkie zmiany przez React state, brak bezpośredniej manipulacji DOM.
* **useTransition:** Wykorzystanie Concurrent Features dla non-urgent updates.
* **flushSync:** Rzadko używane synchronous updates gdy absolutnie konieczne.
* **Profiler API:** Monitoring performance przez React Profiler zamiast zewnętrznych narzędzi.

### GoodExample3.tsx - Proper keys i reconciliation
* **Unique, stable keys:** Keys bazujące na unikalnych ID, nie index czy random.
* **Fragment keys:** Prawidłowe keys dla fragmentów.
* **Controlled components:** Zawsze inicjalizowane z wartością, nigdy undefined.
* **Key reset pattern:** Świadome użycie key do wymuszenia remount komponentu.
* **React.memo optimization:** Custom comparator dla precyzyjnej kontroli re-renders.

## Kluczowe koncepcje

### React Reconciliation
React używa algorytmu "reconciliation" do wydajnego aktualizowania DOM:
1. **Virtual DOM comparison:** React porównuje nowe i stare drzewo elementów.
2. **Minimal updates:** Wykonuje tylko minimalne zmiany w rzeczywistym DOM.
3. **Key-based tracking:** Używa keys do śledzenia elementów między renderami.

### React Fiber Architecture
Fiber to reimplementacja React core algorithm:
* **Incremental rendering:** Możliwość przerwania i wznowienia renderowania.
* **Priority-based updates:** Różne priorytety dla różnych typów updates.
* **Concurrent features:** useTransition, useDeferredValue, Suspense.

### Custom Renderers
`react-reconciler` pozwala tworzyć własne renderer'y dla:
* **CLI applications:** react-ink, react-blessed
* **Mobile:** React Native
* **3D/WebGL:** react-three-fiber
* **PDF:** react-pdf
* **Hardware:** Johnny-Five (robotics)

## Kiedy używać?

### Custom Renderer
* Renderowanie React do non-DOM targets (CLI, mobile, 3D, etc.)
* Potrzebujesz pełnego wsparcia React features w custom environment
* Chcesz wykorzystać React component model w nietypowym kontekście

### Keys
* **Zawsze:** W listach renderowanych przez .map()
* **Unique:** Bazuj na unikalnym ID z danych, nie index
* **Stable:** Keys nie powinny się zmieniać między renderami
* **Fragments:** Fragmenty w mapowanych listach też potrzebują keys

### Fiber Awareness
* Nie manipuluj Fiber internals bezpośrednio
* Używaj oficjalnych API: Profiler, useTransition, flushSync
* Pozwól React zarządzać reconciliation

## Korzyści

* **Predictable updates:** React gwarantuje spójność stanu i UI.
* **Performance:** Fiber architecture optymalizuje rendering automatycznie.
* **Debugging:** React DevTools pokazują Fiber tree i performance metrics.
* **Concurrent features:** useTransition, Suspense działają out of the box.
* **Ecosystem:** Wszystkie React biblioteki i narzędzia działają z custom renderers.

## Potencjalne pułapki

* **Don't fight React:** Nie próbuj "optymalizować" przez omijanie reconciliation.
* **Keys matter:** Złe keys to źródło trudnych do debugowania bugów.
* **No direct DOM:** Nigdy nie manipuluj DOM bezpośrednio w React komponentach.
* **No Fiber hacking:** Nie używaj `_reactInternalInstance` ani innych internals.
* **Custom renderer complexity:** Implementacja custom renderer'a jest zaawansowana - używaj istniejących gdy możliwe.

## Zasoby

* [React Reconciliation Docs](https://react.dev/learn/preserving-and-resetting-state)
* [react-reconciler on GitHub](https://github.com/facebook/react/tree/main/packages/react-reconciler)
* [React Fiber Architecture](https://github.com/acdlite/react-fiber-architecture)
* [React Profiler API](https://react.dev/reference/react/Profiler)
