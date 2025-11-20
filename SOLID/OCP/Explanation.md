# OCP - Open/Closed Principle (Zasada Otwarte-Zamknięte)

## 🔴 Problem w BadExample

Klasa `ReportGenerator` używa potężnego **switch statement** do wyboru formatu raportu:

```csharp
switch (format.ToUpper())
{
    case "PDF": return GeneratePdfReport(data);
    case "HTML": return GenerateHtmlReport(data);
    case "CSV": return GenerateCsvReport(data);
    case "XML": return GenerateXmlReport(data);
    // ...
}
```

### Dlaczego to źle?

1. **Modyfikacja istniejącego kodu**: Każdy nowy format wymaga dodania nowego `case` w switch
2. **Naruszenie OCP**: Klasa NIE jest zamknięta na modyfikacje
3. **Ryzyko regresji**: Zmiana w jednym miejscu może zepsuć inne formaty
4. **Trudne w testowaniu**: Nie da się przetestować nowego formatu w izolacji
5. **Konflity w Git**: Wszyscy programiści modyfikują ten sam switch - konflikty gwarantowane

## ✅ Rozwiązanie w GoodExample

Używamy **wzorca Strategy** z interfejsem `IReportFormatter`:

```csharp
public interface IReportFormatter
{
    string Format(List<SalesData> data);
}
```

Każdy format to osobna klasa:
- `PdfReportFormatter`
- `HtmlReportFormatter`
- `CsvReportFormatter`
- `XmlReportFormatter`
- `JsonReportFormatter` ← dodany bez modyfikacji istniejącego kodu!

### Korzyści

1. **Rozszerzalność bez modyfikacji**: Nowy format = nowa klasa, zero zmian w istniejącym kodzie
2. **Zgodność z OCP**: Kod otwarty na rozszerzenia, zamknięty na modyfikacje
3. **Brak ryzyka regresji**: Nowy formatter nie może zepsuć istniejących
4. **Łatwe testowanie**: Każdy formatter testowany osobno
5. **Brak konfliktów Git**: Każdy programista pracuje na swojej klasie
6. **Dependency Injection**: Generator przyjmuje formatter w konstruktorze

## 💼 Kontekst Biznesowy

### Scenariusz: Nowy klient potrzebuje raportu w formacie JSON

**BadExample** (❌):
1. Otwórz `ReportGenerator.cs`
2. Znajdź switch statement
3. Dodaj case "JSON"
4. Zaimplementuj `GenerateJsonReport`
5. Przetestuj WSZYSTKIE formaty (ryzyko regresji)
6. Code review - wszyscy muszą sprawdzić zmianę
7. Konflikty Git z innymi programistami

**GoodExample** (✅):
1. Utwórz `JsonReportFormatter.cs`
2. Zaimplementuj interface `IReportFormatter`
3. Przetestuj tylko JSON formatter
4. Code review - tylko nowa klasa
5. Zero konfliktów, zero ryzyka regresji

### Oszczędności

- Czas: 2h → 30 min
- Ryzyko: wysokie → minimalne
- Konflikty: częste → brak

## 🎯 Kiedy stosować OCP?

- **Plugin systems**: Dodawanie nowych pluginów bez modyfikacji core
- **Formatters/Exporters**: Różne formaty output (PDF, CSV, JSON, XML)
- **Payment providers**: Dodawanie nowych bramek płatności
- **Notification systems**: Email, SMS, Push, Slack
- **Validation rules**: Dynamiczne dodawanie nowych reguł

## 📏 Jak rozpoznać naruszenie OCP?

Czerwone flagi:
- ❌ Długie switch/if-else statements
- ❌ "Dla nowej funkcji muszę zmodyfikować istniejącą klasę"
- ❌ Kod pełen `instanceof` lub `is` checks
- ❌ Strach przed dodawaniem nowych features (ryzyko regresji)

## 🔧 Narzędzia wspierające OCP

- **Strategy Pattern**: Wymienne algorytmy (jak w przykładzie)
- **Factory Pattern**: Tworzenie obiektów bez modyfikacji kodu
- **Dependency Injection**: Wstrzykiwanie implementacji przez interface
- **Template Method**: Rozszerzanie przez dziedziczenie

## 💡 Złota zasada OCP

> "Powinieneś być w stanie dodać nowe zachowanie bez zmiany kodu, który już działa"

Jeśli dodanie funkcji wymaga zmiany istniejącego kodu → naruszenie OCP
