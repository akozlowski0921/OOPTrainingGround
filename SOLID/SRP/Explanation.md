# SRP - Single Responsibility Principle (Zasada Pojedynczej Odpowiedzialności)

## 🔴 Problem w BadExample

Klasa `UserRegistration` jest tzw. **God Class** (Klasa-Bóg) - robi wszystko:
- Waliduje dane wejściowe (email, hasło, imię)
- Hashuje hasła
- Zapisuje dane do bazy
- Loguje zdarzenia (info, błędy)

### Dlaczego to źle?

1. **Trudność w utrzymaniu**: Zmiana jednej funkcjonalności wymaga modyfikacji tej samej klasy, co zwiększa ryzyko wprowadzenia błędów
2. **Brak możliwości reużycia**: Nie możemy użyć samego walidatora lub loggera w innych miejscach bez ciągnięcia całej klasy
3. **Trudność w testowaniu**: Nie da się przetestować walidacji oddzielnie od zapisu do bazy
4. **Naruszenie SRP**: Klasa ma wiele powodów do zmiany:
   - Zmiana zasad walidacji
   - Zmiana algorytmu hashowania
   - Zmiana sposobu zapisu do bazy
   - Zmiana sposobu logowania

## ✅ Rozwiązanie w GoodExample

Każda klasa ma **jedną, dobrze zdefiniowaną odpowiedzialność**:

- `UserValidator` - walidacja danych
- `PasswordHasher` - hashowanie haseł
- `UserRepository` - operacje na bazie danych
- `Logger` - logowanie zdarzeń
- `UserRegistrationService` - koordynacja procesu rejestracji

### Korzyści

1. **Łatwość utrzymania**: Zmiana logiki walidacji? Edytujesz tylko `UserValidator`
2. **Reużywalność**: Możesz użyć `Logger` w dowolnym miejscu aplikacji
3. **Testowalność**: Każda klasa może być testowana niezależnie
4. **Przejrzystość**: Od razu wiadomo, gdzie szukać konkretnej funkcjonalności
5. **Skalowalność**: Łatwo dodać nowe funkcje (np. walidacja siły hasła w `PasswordHasher`)

## 💼 Kontekst Biznesowy

W zespole różne osoby mogą pracować nad różnymi aspektami:
- DevOps zmienia sposób logowania (centralizacja logów) → modyfikuje tylko `Logger`
- Security team zmienia algorytm hashowania → modyfikuje tylko `PasswordHasher`
- Backend developer dodaje nowe walidacje → modyfikuje tylko `UserValidator`

Żadna z tych zmian nie wpływa na inne części systemu.

## 🎯 Kiedy stosować SRP?

- Zawsze! To podstawowa zasada dobrego projektowania
- Szczególnie ważne w:
  - Klasach serwisowych (services)
  - Repozytoriach danych
  - Validatorach
  - Klasach biznesowych

## 📏 Jak poznać naruszenie SRP?

Pytanie: "Za co odpowiada ta klasa?" ma więcej niż jedną odpowiedź → naruszenie SRP
- "UserRegistration waliduje, hashuje, zapisuje i loguje" → ❌ Źle
- "UserValidator waliduje dane użytkownika" → ✅ Dobrze
