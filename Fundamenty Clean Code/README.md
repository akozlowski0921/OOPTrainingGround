# Fundamenty Clean Code

Ten folder zawiera 5 fundamentalnych zasad czystego kodu z praktycznymi przykładami w C# i TypeScript.

## 📂 Struktura

Każdy przykład zawiera:
- **BadExample** - Realistyczny kod legacy pokazujący typowe błędy
- **GoodExample** - Ten sam kod po refaktoryzacji zgodnie z zasadami Clean Code
- **Explanation.md** - Zwięzłe wyjaśnienie problemu, rozwiązania i korzyści

## 🎯 Zasady

### 01-DRY (Don't Repeat Yourself)
**Technologia:** C#  
**Problem:** Duplikacja logiki biznesowej obliczającej status zamówienia w 3 różnych miejscach  
**Rozwiązanie:** Wydzielenie wspólnej logiki do klasy `OrderStatusCalculator`  
**Kluczowa lekcja:** Jedna zmiana w logice biznesowej powinna wymagać modyfikacji tylko w jednym miejscu

### 02-KISS (Keep It Simple, Stupid)
**Technologia:** TypeScript  
**Problem:** Przekombinowana walidacja formularza z zagnieżdżonymi if-ami i dziwnymi flagami  
**Rozwiązanie:** Prosta walidacja z early return i pomocniczymi metodami  
**Kluczowa lekcja:** Prostota zwiększa czytelność i zmniejsza liczbę błędów

### 03-YAGNI (You Aren't Gonna Need It)
**Technologia:** C#  
**Problem:** Klasa pełna nieużywanych metod "na przyszłość"  
**Rozwiązanie:** Tylko metody faktycznie potrzebne biznesowi  
**Kluczowa lekcja:** Dodawaj funkcjonalność gdy jest POTRZEBNA, nie gdy "może kiedyś będzie potrzebna"

### 04-Magic-Numbers-Strings
**Technologia:** C#  
**Problem:** Hardcoded wartości jak `status == 4` czy `type == "admin"`  
**Rozwiązanie:** Użycie Enum i named constants  
**Kluczowa lekcja:** Nazwane stałe są samodokumentujące i łatwe w utrzymaniu

### 05-Fail-Fast
**Technologia:** TypeScript  
**Problem:** Wielopoziomowe zagnieżdżone if-y ("Arrow Code")  
**Rozwiązanie:** Early return pattern - warunki brzegowe najpierw  
**Kluczowa lekcja:** Spłaszcz warunki używając guard clauses dla lepszej czytelności

## 💡 Jak korzystać z tego materiału

1. **Przeczytaj BadExample** - Zrozum problem i dlaczego ten kod jest problematyczny
2. **Przeanalizuj GoodExample** - Zobacz jak te same wymagania można zaimplementować lepiej
3. **Przeczytaj Explanation.md** - Zrozum korzyści biznesowe i techniczne
4. **Zastosuj w praktyce** - Szukaj podobnych wzorców w swoim kodzie

## 🎓 Poziom trudności

**Poziom:** Mid → Senior  
**Czas nauki:** ~2-3 godziny  
**Prerequisite:** Podstawowa znajomość C# i TypeScript

## 🔗 Powiązane tematy

Po opanowaniu tych fundamentów, przejdź do:
- SOLID Principles
- Design Patterns (GoF)
- Refactoring Techniques
