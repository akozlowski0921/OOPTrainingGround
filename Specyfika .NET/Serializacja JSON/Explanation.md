# Serializacja JSON - System.Text.Json vs Newtonsoft.Json

## System.Text.Json vs Newtonsoft.Json

### System.Text.Json (.NET Core 3.0+)
**Zalety:**
- ✅ Wbudowane w .NET Core/5+
- ✅ Lepsza wydajność (~2x szybsze)
- ✅ Mniejsze zużycie pamięci
- ✅ Source Generators (AOT compilation)
- ✅ Span<T> i Memory<T> support

**Wady:**
- ❌ Mniej funkcji niż Newtonsoft
- ❌ Mniej elastyczne (celowo - bezpieczeństwo)

### Newtonsoft.Json (Json.NET)
**Zalety:**
- ✅ Bardzo bogata funkcjonalność
- ✅ Elastyczne - wiele opcji konfiguracji
- ✅ Działa na .NET Framework

**Wady:**
- ❌ Wolniejsze niż System.Text.Json
- ❌ Większe zużycie pamięci
- ❌ External dependency

### Kiedy używać którego?

**System.Text.Json:**
- ✅ Nowe projekty .NET 5+
- ✅ High-performance scenarios
- ✅ ASP.NET Core Web APIs
- ✅ Proste DTOs

**Newtonsoft.Json:**
- ✅ .NET Framework projects
- ✅ Complex serialization scenarios
- ✅ Legacy codebase
- ✅ Potrzebujesz zaawansowanych features

## Problemy w BadExample

### 1. Publiczne pola zamiast properties
```csharp
public string InternalNote; // ❌ Nie serializuje się w System.Text.Json
```
**System.Text.Json** domyślnie serializuje tylko **properties**, nie pola.

### 2. Niespójne naming conventions
```csharp
public int ProductId { get; set; } // Serializuje jako "ProductId"
```
JavaScript/TypeScript convention: `camelCase`  
C# convention: `PascalCase`

### 3. Mieszanie bibliotek
```csharp
// ❌ Używanie obu bibliotek
System.Text.Json.JsonSerializer.Serialize(user);
JsonConvert.DeserializeObject<User>(json);
```
**Problemy:**
- Różne domyślne zachowania
- 2 dependencies zamiast 1
- Trudniejsze utrzymanie

### 4. Brak obsługi błędów
```csharp
return JsonSerializer.Deserialize<Product>(json); // ❌ Co jeśli json = null?
```
**Możliwe wyjątki:**
- `ArgumentNullException` - json is null
- `JsonException` - nieprawidłowy format

### 5. Tworzenie options przy każdym wywołaniu
```csharp
// ❌ Tworzy nową instancję za każdym razem
var options = new JsonSerializerOptions { ... };
```
**Problem:** Performance overhead - options powinny być singleton

### 6. Cykliczne referencje
```csharp
Order -> Customer -> Orders -> Order -> ... // ❌ Infinite loop
```
**Domyślnie:** `JsonException: A possible object cycle was detected`

## Rozwiązania w GoodExample

### 1. Singleton JsonSerializerOptions
```csharp
private static readonly JsonSerializerOptions DefaultOptions = new()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};
```
**Korzyści:**
- Jedna instancja dla całej aplikacji
- Lepsze performance
- Spójność konfiguracji

### 2. Camel case naming
```csharp
PropertyNamingPolicy = JsonNamingPolicy.CamelCase
// ProductId -> productId
```

**Lub per-property:**
```csharp
[JsonPropertyName("id")]
public int ProductId { get; set; }
```

### 3. JsonIgnore dla wrażliwych danych
```csharp
[JsonIgnore]
public string Password { get; set; }
```
**Nigdy nie serializuj:**
- Passwords
- Security tokens
- Internal implementation details

### 4. Obsługa cyklicznych referencji
```csharp
var options = new JsonSerializerOptions
{
    ReferenceHandler = ReferenceHandler.Preserve
};
```
**Output:**
```json
{
  "$id": "1",
  "customer": {
    "$id": "2",
    "orders": [{ "$ref": "1" }]
  }
}
```

**Alternatywa:** `[JsonIgnore]` na jednej ze stron relacji

### 5. Custom Converter
```csharp
public class TemperatureConverter : JsonConverter<Temperature>
{
    public override void Write(Utf8JsonWriter writer, Temperature value, ...)
    {
        writer.WriteNumberValue(value.Celsius);
    }
    
    public override Temperature Read(ref Utf8JsonReader reader, ...)
    {
        return new Temperature { Celsius = reader.GetDouble() };
    }
}
```

**Użycie:**
```csharp
[JsonConverter(typeof(TemperatureConverter))]
public Temperature Temp { get; set; }
```

### 6. Source Generators (.NET 6+)
```csharp
[JsonSerializable(typeof(ProductDto))]
public partial class AppJsonContext : JsonSerializerContext { }

// Użycie
JsonSerializer.Serialize(product, AppJsonContext.Default.ProductDto);
```

**Korzyści:**
- AOT (Ahead-of-Time) compilation
- ~30% faster
- Mniejsze zużycie pamięci
- Native AOT support (trimming)

### 7. Enum jako string
```csharp
// ❌ Domyślnie: 0, 1, 2...
// ✅ Z converterem: "Pending", "Processing"...

[JsonConverter(typeof(JsonStringEnumConverter))]
public OrderStatus Status { get; set; }
```

**Lub globalnie:**
```csharp
var options = new JsonSerializerOptions
{
    Converters = { new JsonStringEnumConverter() }
};
```

### 8. Polymorphic serialization (.NET 7+)
```csharp
[JsonDerivedType(typeof(CreditCardPayment), "creditCard")]
[JsonDerivedType(typeof(PayPalPayment), "paypal")]
public abstract class Payment { }
```

**Output:**
```json
{
  "$type": "creditCard",
  "amount": 100,
  "cardNumber": "..."
}
```

## Performance Comparison

### System.Text.Json
- Serialize: ~2x faster
- Deserialize: ~2x faster
- Memory: ~50% mniej alokacji

### Source Generator
- Serialize: ~30% faster niż reflection
- Deserialize: ~25% faster
- Trimming-friendly (Native AOT)

## Best Practices

### DO:
✅ Używaj System.Text.Json dla nowych projektów .NET 5+  
✅ Singleton JsonSerializerOptions  
✅ CamelCase naming policy dla Web APIs  
✅ `[JsonIgnore]` dla wrażliwych danych  
✅ Custom converters dla specjalnych przypadków  
✅ Source Generators dla best performance  
✅ Obsługa `null` i `JsonException`  

### DON'T:
❌ Mieszaj System.Text.Json i Newtonsoft.Json  
❌ Twórz nowe options przy każdym wywołaniu  
❌ Serializuj hasła i tokeny  
❌ Ignoruj cykliczne referencje  
❌ Używaj pól zamiast properties (System.Text.Json)  

## Migration z Newtonsoft.Json

### Różnice w domyślnym zachowaniu:
| Feature | Newtonsoft.Json | System.Text.Json |
|---------|----------------|------------------|
| Case sensitivity | Case-insensitive | Case-sensitive |
| Property names | PascalCase | PascalCase |
| Fields | Serializuje | NIE serializuje |
| Nulls | Serializuje | Opcjonalne |
| Enum | Number (default) | Number (default) |

### Zmiany w migracji:
```csharp
// Newtonsoft.Json
[JsonProperty("id")]
public int ProductId { get; set; }

// System.Text.Json
[JsonPropertyName("id")]
public int ProductId { get; set; }
```

## ASP.NET Core Configuration

**Program.cs / Startup.cs:**
```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
```

## Debugging Tips

**Sprawdź output:**
```csharp
var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(json);
```

**Deserializacja z komentarzami:**
```csharp
var options = new JsonSerializerOptions
{
    ReadCommentHandling = JsonCommentHandling.Skip
};
```

**Case-insensitive deserializacja:**
```csharp
var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};
```
