# Typy Referencyjne vs Wartościowe - Boxing, Unboxing, Span<T>

## Podstawy

### Value Types (typy wartościowe)
- Przechowywane na **stosie** (stack) lub inline w obiektach
- Zawierają **rzeczywistą wartość**
- Kopiowane **przez wartość**
- Przykłady: `int`, `double`, `bool`, `struct`, `enum`, `DateTime`

### Reference Types (typy referencyjne)
- Przechowywane na **stercie** (heap)
- Zawierają **referencję** do obiektu
- Kopiowane **przez referencję**
- Przykłady: `class`, `interface`, `delegate`, `string`, `array`

## Problemy w BadExample

### 1. Boxing i Unboxing - Performance Hit
```csharp
int value = 42;
object boxed = value;    // ❌ Boxing - alokacja na heap
int unboxed = (int)boxed; // ❌ Unboxing - performance cost
```

**Boxing:**
1. Alokacja na heap (~80 bytes overhead)
2. Kopiowanie wartości
3. Garbage Collection pressure

**Koszt:** ~10-100x wolniejsze niż operacje bez boxing

### 2. List<object> zamiast Generics
```csharp
List<object> numbers = new List<object>();
numbers.Add(42); // ❌ Boxing każdego int
```

**Problem:**
- Boxing przy każdym Add
- Unboxing przy każdym dostępie
- Brak type safety

### 3. Mutable Struct
```csharp
public struct MutablePoint {
    public int X { get; set; } // ❌ Mutable
    
    public void Move(int delta) {
        X += delta; // Modyfikuje kopię, nie oryginał!
    }
}

var point = new MutablePoint { X = 10 };
point.Move(5); // Działa
var copy = point;
copy.Move(5); // Modyfikuje tylko kopię
```

**Problem:** Nieprzewidywalne zachowanie przez defensywne kopiowanie

### 4. Duży Struct
```csharp
public struct LargeStruct { // 48 bytes
    public int Field1, Field2, Field3, Field4;
    public long Field5, Field6;
    public double Field7, Field8;
}

void Process(LargeStruct s) { } // ❌ Kopiuje 48 bytes
```

**Zasada:** Struct powinien być **≤ 16 bytes**

### 5. Defensive Copy
```csharp
public struct Point {
    public int X { get; } // Wygląda immutable
    
    public Point(int x) { X = x; }
    
    public void Print() { Console.WriteLine(X); }
}

// Kompilator tworzy defensive copy przy wywołaniu metody!
```

**Rozwiązanie:** `readonly struct`

## Rozwiązania w GoodExample

### 1. Generics zamiast Object - Zero Boxing
```csharp
public void Process<T>(List<T> items) {
    foreach (T item in items) {
        Console.WriteLine(item); // ✅ Brak boxing
    }
}
```

### 2. Readonly Struct - Immutable
```csharp
public readonly struct Point {
    public int X { get; }
    public int Y { get; }
    
    public Point(int x, int y) {
        X = x;
        Y = y;
    }
    
    public Point Move(int dx, int dy) =>
        new Point(X + dx, Y + dy); // Zwraca nową instancję
}
```

**Korzyści:**
- Immutable - brak defensywnych kopii
- Thread-safe
- Przewidywalne zachowanie

### 3. Record Struct (C# 10+)
```csharp
public readonly record struct Point(int X, int Y) {
    public Point Move(int dx, int dy) => new(X + dx, Y + dy);
}
```

**Auto-generuje:**
- Constructor
- Equals/GetHashCode (value equality)
- ToString
- Deconstruction

### 4. Span<T> - Zero Allocation Slicing
```csharp
int[] array = { 1, 2, 3, 4, 5 };

// ❌ Traditional - nowa alokacja
int[] subArray = array[1..4]; // Kopiuje 3 elementy

// ✅ Span - zero allocation
ReadOnlySpan<int> span = array.AsSpan(1, 3); // Widok, brak kopii
```

**Span<T> użycie:**
- Zero allocation slicing
- Performance-critical code
- Stack-only scenarios
- String manipulation bez alokacji

**Ograniczenia:**
- Nie może być używany w async methods
- Nie może być polem w klasie (tylko ref struct)
- Stack-only

### 5. Memory<T> dla Async
```csharp
public async Task ProcessAsync(ReadOnlyMemory<byte> data) {
    // ✅ Memory<T> działa w async (w przeciwieństwie do Span<T>)
    await Task.Delay(100);
    return data.Length;
}
```

**Memory<T> vs Span<T>:**
- `Span<T>` - stack-only, nie async
- `Memory<T>` - heap, async-friendly

### 6. In Parameter - Readonly Reference
```csharp
public void Process(in LargeStruct data) {
    // 'in' - readonly reference, zero copy
    Console.WriteLine(data.Field1);
    // data.Field1 = 10; // ❌ Błąd kompilacji
}
```

**Kiedy używać:**
- Struct > 16 bytes
- Readonly access
- Performance-critical code

### 7. Ref Return
```csharp
public ref int GetElement(int[] array, int index) {
    return ref array[index]; // Referencja, nie kopia
}

ref int element = ref GetElement(array, 2);
element = 100; // Modyfikuje array[2]
```

## Kiedy używać Struct vs Class

### Używaj STRUCT gdy:
✅ Typ jest mały (≤ 16 bytes)  
✅ Immutable (readonly struct)  
✅ Wartościowa semantyka (kopiowanie ma sens)  
✅ Krótki lifetime  
✅ Używany w kolekcjach (lepsze cache locality)  

**Przykłady:** `Point`, `Money`, `GUID`, `DateTime`, `Color`

### Używaj CLASS gdy:
✅ Typ jest duży (> 16 bytes)  
✅ Mutable state  
✅ Potrzeba dziedziczenia  
✅ Referencjna semantyka (współdzielenie)  
✅ Długi lifetime  

**Przykłady:** `Entity`, `ViewModel`, `Service`, `Controller`

## Performance Comparison

### Boxing Cost
```csharp
// ❌ Boxing
List<object> list = new();
for (int i = 0; i < 1000; i++)
    list.Add(i); // 1000 heap allocations

// ✅ No boxing
List<int> list = new();
for (int i = 0; i < 1000; i++)
    list.Add(i); // 0 heap allocations (poza listą)
```

**Benchmark:**
- Boxing: ~100 ns per operation
- No boxing: ~5 ns per operation
- **20x faster**

### Span<T> vs Array
```csharp
int[] array = new int[1000];

// ❌ Traditional substring
int[] sub = array[100..200]; // Alokacja + kopiowanie

// ✅ Span
ReadOnlySpan<int> span = array.AsSpan(100, 100); // Zero allocation
```

**Benchmark:**
- Array copy: ~200 ns
- Span: ~1 ns
- **200x faster**

## Best Practices

### DO:
✅ Używaj `readonly struct` dla small immutable types  
✅ Używaj `Span<T>` dla slicing bez alokacji  
✅ Używaj generics zamiast `object` (unikanie boxing)  
✅ Używaj `record struct` dla value objects  
✅ Używaj `in` dla pass-by-reference large structs  
✅ Struct ≤ 16 bytes  

### DON'T:
❌ Mutable structs  
❌ Large structs (> 16 bytes)  
❌ Struct z domyślnym constructorem modyfikującym pola  
❌ Boxing value types (użyj generics)  
❌ `new int[].Skip(10).Take(5)` (użyj Span)  

## Zaawansowane scenariusze

### StackAlloc dla hot path
```csharp
Span<int> buffer = stackalloc int[100]; // Stack allocation
for (int i = 0; i < 100; i++)
    buffer[i] = i;
// Zero GC pressure
```

### Ref struct dla custom buffers
```csharp
public ref struct BufferWriter {
    private Span<byte> _buffer;
    
    public BufferWriter(Span<byte> buffer) => _buffer = buffer;
    
    public void Write(byte value) { /* ... */ }
}
```

### String manipulation z Span
```csharp
string text = "Hello World";

// ❌ Alokacje
string upper = text.Substring(0, 5).ToUpper(); // 2 alokacje

// ✅ Zero allocation
ReadOnlySpan<char> span = text.AsSpan(0, 5);
Span<char> upper = stackalloc char[5];
span.ToUpperInvariant(upper, CultureInfo.InvariantCulture);
```

## Memory Management

### Stack vs Heap
- **Stack:** Fast, automatic cleanup, limited size (~1MB)
- **Heap:** Slow, GC managed, unlimited size

### GC Impact
- Boxing: Więcej alokacji → więcej GC
- Large structs: Więcej kopiowania → więcej CPU
- Span<T>: Zero alokacji → zero GC pressure
