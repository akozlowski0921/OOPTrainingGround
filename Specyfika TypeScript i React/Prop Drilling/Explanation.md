# Prop Drilling

## Problem w BadExample

* **Nadmierne przekazywanie props:** Props `theme` i `user` muszą przejść przez 4-5 poziomów komponentów, zanim dotrą do komponentu, który faktycznie ich potrzebuje.
* **Komponenty pośredniczące:** Komponenty takie jak `Layout`, `Header`, `Navigation` nie używają przekazywanych props - są tylko "przekaźnikami".
* **Trudność w refaktoryzacji:** Dodanie nowego prop wymaga aktualizacji wszystkich komponentów pośrednich.
* **Niska czytelność:** Trudno zrozumieć, który komponent faktycznie używa danych.
* **Słaba skalowalność:** Im głębsza struktura komponentów, tym większy problem.

## Rozwiązanie w GoodExample

* **Context API:** Dane globalne (theme, user) są dostępne przez Context, bez przekazywania przez komponenty pośrednie.
* **Custom Hooks:** `useTheme()` i `useUser()` zapewniają wygodny dostęp do danych z walidacją.
* **Component Composition:** Dla prostszych przypadków - komponowanie komponentów bezpośrednio tam, gdzie są potrzebne.
* **Separacja odpowiedzialności:** Komponenty pośrednie nie muszą znać szczegółów danych, które przepływają przez aplikację.

## Kiedy używać Context, a kiedy Composition?

**Context API:**
* Dane używane w wielu miejscach aplikacji (theme, auth, language)
* Głęboka struktura komponentów (>3 poziomy)
* Dane często się zmieniają i wiele komponentów musi reagować

**Component Composition:**
* Płytka struktura komponentów (2-3 poziomy)
* Dane używane tylko w kilku miejscach
* Prostsze przypadki, gdzie Context byłby przesadą

## Korzyści

* **Czytelność:** Komponenty deklarują tylko to, czego faktycznie potrzebują.
* **Łatwość rozbudowy:** Dodanie nowych props nie wymaga zmian w komponentach pośrednich.
* **Reużywalność:** Komponenty nie są związane ze specyficzną strukturą props.
* **Testowalność:** Łatwiej mockować Context niż przekazywać wiele poziomów props.
* **Wydajność:** Context z React.memo może ograniczyć niepotrzebne re-rendery.

## Potencjalne pułapki

* **Nie nadużywaj Context:** Dla lokalnego stanu (1-2 poziomy) zwykłe props są lepsze.
* **Rozdziel Contexty:** Nie wrzucaj wszystkiego do jednego Context - rozdziel według odpowiedzialności.
* **Optymalizacja re-renderów:** Użyj `useMemo` dla value w Provider, aby uniknąć niepotrzebnych re-renderów.
