# KISS (Keep It Simple, Stupid)

## Problem w BadExample

* **Nadmierna złożoność:** Zagnieżdżone if-y i dziwne flagi (`validationStage`, `emailValid`, itp.)
* **Ręczna pętla przez znaki:** Zamiast użyć prostych narzędzi (regex, metody stringowe)
* **Trudna czytelność:** Ciężko zrozumieć flow walidacji przez wszystkie zagnieżdżenia
* **Łatwo wprowadzić błąd:** Logika zależna od flag i stadiów jest podatna na błędy

## Rozwiązanie w GoodExample

* **Early return:** Sprawdzamy każde pole niezależnie, dodajemy błędy do listy
* **Wydzielone metody pomocnicze:** `isValidEmail()`, `hasRequiredPasswordCharacters()`
* **Używamy gotowych narzędzi:** Regex do walidacji emaila i hasła
* **Liniowy flow:** Kod czyta się od góry do dołu bez zagnieżdżeń

## Korzyści biznesowe

* **Szybsze onboarding:** Nowi deweloperzy rozumieją kod znacznie szybciej
* **Mniej błędów:** Prostszy kod = mniej miejsc na pomyłki
* **Łatwiejsza rozbudowa:** Dodanie nowej walidacji to kilka linijek

## Korzyści techniczne

* **Łatwiejsza testowanie:** Każda metoda pomocnicza może być testowana osobno
* **Mniej kodu:** Krótszy, bardziej zwięzły kod (55 vs 125 linii)
* **Lepsza czytelność:** Natychmiastowe zrozumienie co robi każda sekcja
* **Idiomatyczny kod:** Używamy standardowych praktyk TypeScript/JavaScript
