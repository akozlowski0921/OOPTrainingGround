# Dynamiczne Formy z Logicznym Flow

## Problem w BadExample

### BadExample.tsx - Ręczne tworzenie formularzy
* **Brak centralizacji:** Każde pole wymaga osobnej implementacji w wielu miejscach.
* **Duplikacja kodu:** Validation logic powtarzana dla każdego pola.
* **Trudność rozbudowy:** Dodanie pola wymaga zmian w state, validation, JSX.
* **Brak type safety:** Łatwo o błędy przy dodawaniu nowych pól.

### BadExample2.tsx - Ręczna walidacja
* **Duplikacja walidacji:** Każde pole ma osobną funkcję walidującą.
* **Rozrzucona logika:** Reguły walidacji w wielu miejscach kodu.
* **Conditional validation:** Trudna do zarządzania logika warunkowa.
* **Brak reusability:** Nie można łatwo współdzielić reguł walidacji.

### BadExample3.tsx - Ręczny wizard
* **Fragmentacja stanu:** Dane każdego kroku w osobnym state.
* **Duplikacja walidacji:** Osobna walidacja dla każdego kroku.
* **Trudne zarządzanie:** Łączenie danych z wielu kroków przy submicie.
* **Brak progress tracking:** Trudno śledzić postęp użytkownika.

## Rozwiązanie w GoodExample

### GoodExample.tsx - Schema-driven forms
* **React Hook Form:** Minimalizuje re-renders, lepka wydajność.
* **JSON Schema:** Automatyczna generacja pól z konfiguracji.
* **Type safety:** Zod zapewnia pełne bezpieczeństwo typów.
* **Centralizacja:** Cała struktura formularza w jednym miejscu.

### GoodExample2.tsx - Zod validation
* **Deklaratywna walidacja:** Wszystkie reguły w schema.
* **Conditional validation:** Discriminated unions dla warunkowej walidacji.
* **Cross-field validation:** Refine dla walidacji między polami.
* **Async validation:** Wsparcie dla asynchronicznych walidacji.
* **Error messages:** Automatyczne, skonfigurowane komunikaty błędów.

### GoodExample3.tsx - Wizard forms
* **Unified state:** Wszystkie dane w jednym formie.
* **Step validation:** trigger() dla walidacji konkretnych pól.
* **Progress tracking:** Wizualne wskaźniki postępu.
* **Conditional flow:** Dynamiczne kroki bazujące na danych użytkownika.

## Kluczowe koncepcje

### Schema-driven development
* **Single source of truth:** Schema definiuje strukturę i walidację.
* **Type inference:** TypeScript types automatycznie z schema.
* **Validation rules:** Deklaratywne reguły zamiast imperatywnego kodu.

### React Hook Form
* **Uncontrolled approach:** Minimalizuje re-renders.
* **register():** Rejestracja pól w formie.
* **handleSubmit():** Walidacja i submit handling.
* **formState:** Errors, touched, dirty, isValid.

### Zod
* **Runtime validation:** Type-safe validation w runtime.
* **Composition:** Łączenie schematów dla reusability.
* **Transformations:** parse(), safeParse(), transform().
* **Error handling:** Szczegółowe informacje o błędach.

## Kiedy używać?

### Schema-driven forms
* **Dynamiczne formularze:** Generowane z API/konfiguracji.
* **Duże formularze:** Wiele pól z złożoną walidacją.
* **Reusable forms:** Te same pola w wielu miejscach.

### Wizard forms
* **Multi-step processes:** Rejestracja, onboarding, checkout.
* **Complex flows:** Warunkowe kroki bazujące na danych.
* **Progress tracking:** Użytkownik musi widzieć postęp.

## Korzyści

* **Mniej kodu:** Schema generuje formularz automatycznie.
* **Type safety:** Pełne bezpieczeństwo typów end-to-end.
* **Łatwiejsza konserwacja:** Zmiany tylko w schema.
* **Lepsza wydajność:** React Hook Form minimalizuje re-renders.
* **Reusability:** Schematy i komponenty łatwo współdzielić.

## Potencjalne pułapki

* **Learning curve:** React Hook Form i Zod wymagają nauki.
* **Over-engineering:** Dla prostych form może być przesadą.
* **Bundle size:** Dodatkowe zależności zwiększają rozmiar.
* **Async validation:** Wymaga careful handling dla UX.

## Zasoby

* [React Hook Form](https://react-hook-form.com/)
* [Zod Documentation](https://zod.dev/)
* [Formik Alternative](https://formik.org/)
* [Yup Validation](https://github.com/jquense/yup)
