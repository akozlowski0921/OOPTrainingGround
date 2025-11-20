# Adapter Pattern

## 📌 Problem w Bad Example
Bezpośrednie używanie zewnętrznego API z dziwnymi nazwami pól (`tmp`, `hmd`, `wnd_spd`) w całym kodzie aplikacji prowadzi do:
- **Nieczytelności** – trzeba pamiętać co oznacza każdy skrót
- **Ścisłego sprzężenia** – cała aplikacja zależy od konkretnej struktury API
- **Trudności w zmianach** – zmiana API wymaga modyfikacji w wielu miejscach
- **Problemów z testowaniem** – trzeba mockować nieintuicyjną strukturę
- **Niemożności wymiany dostawcy** – zmiana API = przepisanie całego kodu

## ✅ Rozwiązanie: Adapter Pattern
Adapter to wzorzec strukturalny, który **konwertuje interfejs klasy na inny interfejs** oczekiwany przez klienta, umożliwiając współpracę klas o niekompatybilnych interfejsach.

### Kluczowe elementy:
1. **Docelowy interfejs** (`Weather`) – czytelny, domenowy model używany w aplikacji
2. **Adaptowany interfejs** (`WeatherApiResponse`) – zewnętrzne API o dziwnej strukturze
3. **Adapter** (`WeatherApiAdapter`) – tłumaczy jeden interfejs na drugi

## 🎯 Korzyści

### 1. Czytelność
```typescript
// Zamiast:
if (data.tmp > 35) { ... }  // Co to tmp?

// Mamy:
if (weather.temperature > 35) { ... }  // Jasne!
```

### 2. Separacja warstw
```typescript
// Adapter izoluje aplikację od zewnętrznego API:
class WeatherApiAdapter {
    adapt(apiResponse: WeatherApiResponse): Weather {
        return {
            temperature: apiResponse.tmp,  // Translacja w JEDNYM miejscu
            // ...
        };
    }
}
```

### 3. Łatwa zmiana dostawcy
```typescript
// Nowy dostawca API = nowy adapter
class AlternativeWeatherAdapter {
    adapt(apiResponse: AlternativeWeatherApi): Weather {
        // Komponenty NIE wymagają zmian!
    }
}
```

### 4. Testowalność
```typescript
// Łatwe mockowanie domenowego modelu:
const mockWeather: Weather = {
    temperature: 25,
    humidity: 60,
    // czytelne, intuicyjne dane testowe
};
```

## 🔄 Kiedy stosować?
- Chcesz użyć istniejącej klasy, ale jej interfejs **nie pasuje** do twojego kodu
- Pracujesz z **zewnętrznymi API** o nieczytelnej strukturze
- Potrzebujesz **ujednolicić** interfejs wielu różnych źródeł danych
- Chcesz **odizolować** logikę biznesową od szczegółów implementacji zewnętrznych
- Planujesz **możliwość wymiany** dostawcy/biblioteki w przyszłości

## 📦 Przykłady w praktyce

### 1. API Integration
```typescript
// Adapter dla różnych payment gateways:
interface PaymentGateway {
    processPayment(amount: number): Promise<PaymentResult>;
}

class StripeAdapter implements PaymentGateway { ... }
class PayPalAdapter implements PaymentGateway { ... }
class BraintreeAdapter implements PaymentGateway { ... }
```

### 2. Legacy Code
```csharp
// Adapter do starego systemu:
public interface IModernRepository
{
    Task<User> GetUserAsync(int id);
}

public class LegacyDatabaseAdapter : IModernRepository
{
    private LegacyDatabase _legacyDb;
    
    public async Task<User> GetUserAsync(int id)
    {
        var legacyUser = _legacyDb.GetUser(id);
        return MapToModernUser(legacyUser);
    }
}
```

### 3. React - API Data
```typescript
// Adapter w React hooks:
function useWeather(city: string) {
    const [weather, setWeather] = useState<Weather | null>(null);
    
    useEffect(() => {
        fetch(`/api/weather?city=${city}`)
            .then(res => res.json())
            .then(apiData => {
                const adapter = new WeatherApiAdapter();
                setWeather(adapter.adapt(apiData));  // Czysta translacja
            });
    }, [city]);
    
    return weather;
}
```

## 🆚 Adapter vs. inne wzorce

### Adapter vs. Facade
- **Adapter** – zmienia interfejs istniejącego obiektu
- **Facade** – upraszcza złożony system przez nowy, prostszy interfejs

### Adapter vs. Decorator
- **Adapter** – zmienia interfejs obiektu
- **Decorator** – dodaje funkcjonalności bez zmiany interfejsu

### Adapter vs. Proxy
- **Adapter** – zapewnia inny interfejs
- **Proxy** – zapewnia ten sam interfejs z dodatkową logiką

## ⚠️ Uwagi
- Adapter dodaje dodatkową warstwę abstrakcji (niewielki overhead)
- Dla bardzo prostych przypadków może być przesadą
- W TypeScript można użyć **type mapping** dla prostych translacji:
  ```typescript
  type Weather = {
      [K in keyof WeatherApiResponse as RenameKey<K>]: WeatherApiResponse[K]
  };
  ```
- Adapter powinien być **cienki** – tylko translacja, bez logiki biznesowej

## 📝 Podsumowanie
- Adapter **izoluje** aplikację od zewnętrznych zależności
- Umożliwia pracę z **czytelnym, domenowym modelem**
- Ułatwia **testowanie** i **wymianę** implementacji
- W nowoczesnych aplikacjach często łączony z **Repository Pattern**
