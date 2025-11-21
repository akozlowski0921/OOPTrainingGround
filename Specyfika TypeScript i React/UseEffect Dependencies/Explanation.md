# UseEffect Dependencies

## Problem w BadExample

* **Stale Closure:** Zmienna `retryCount` jest używana wewnątrz useEffect, ale nie jest dodana do tablicy dependencies. Funkcja effect "zamyka" początkową wartość zmiennej i nigdy nie widzi jej aktualizacji.
* **Brakująca funkcja w dependencies:** Funkcja `performSearch` jest używana w useEffect, ale nie jest w tablicy zależności, co powoduje że effect zawsze używa starej wersji funkcji ze starymi wartościami closure.
* **Nieprzewidywalne zachowanie:** Komponent może nie reagować na zmiany stanu, które powinny wywołać ponowne wykonanie effectu.

## Rozwiązanie w GoodExample

* **Kompletna tablica dependencies:** Wszystkie zmienne i funkcje używane w useEffect są dodane do tablicy zależności.
* **useCallback dla funkcji:** Funkcje używane w useEffect są opakowane w `useCallback`, co zapewnia stabilną referencję i kontrolowaną re-kreację.
* **Functional updates:** Użycie `setState(prev => ...)` pozwala uniknąć dodawania zmiennej stanu do dependencies, gdy potrzebujemy tylko jej aktualnej wartości.

## Korzyści

* **Przewidywalność:** Effect wykonuje się dokładnie wtedy, gdy powinien - po zmianie którejkolwiek ze swoich zależności.
* **Brak stale closures:** Zawsze mamy dostęp do aktualnych wartości zmiennych.
* **Łatwiejsze debugowanie:** ESLint z regułą `exhaustive-deps` automatycznie wykrywa problemy.
* **Mniej bugów:** Eliminacja trudnych do wyśledzenia problemów z niewłaściwym odświeżaniem danych.

## Najważniejsze zasady

1. **Zawsze dodawaj wszystkie dependencies** używane wewnątrz useEffect
2. **Używaj useCallback** dla funkcji, które są dependencies dla useEffect
3. **Używaj functional updates** (`setState(prev => ...)`) gdy potrzebujesz tylko aktualnej wartości stanu
4. **Włącz ESLint regułę** `react-hooks/exhaustive-deps` - ostrzeże Cię o brakujących dependencies

---

## 🎯 FAQ / INSIGHT

### Po co dodawać dependencies do useEffect?

**Problem bez dependencies:**
- **Stale closures** – effect używa starych wartości zmiennych
- **Brak reaktywności** – effect nie reaguje na zmiany
- **Subtelne bugi** – aplikacja działa "prawie dobrze" ale ma edge case bugs
- **Trudny debugging** – problemy pojawiają się sporadycznie

**Korzyści z prawidłowych dependencies:**
- **Spójność danych** – effect zawsze widzi aktualne wartości
- **Przewidywalność** – effect wykonuje się gdy powinien
- **ESLint protection** – automatyczne wykrywanie problemów
- **Reactive by design** – effect reaguje na zmiany w danych

### W czym pomaga prawidłowe użycie dependencies?

✅ **Eliminuje stale closures** – zawsze aktualne dane  
✅ **Automatyczna reaktywność** – effect śledzi zależności  
✅ **Mniej bugów** – ESLint wykrywa brakujące dependencies  
✅ **Lepsze performance** – effect wykonuje się tylko gdy potrzeba  
✅ **Łatwiejszy reasoning** – jasne co wpływa na effect  
✅ **Testowanie** – łatwiej testować reactive behavior  

### ⚖️ Zalety i wady

#### Zalety
✅ **Automatic reactivity** – system sam śledzi zależności  
✅ **Type safety** – TypeScript pomaga wykryć błędy  
✅ **ESLint support** – automatyczne warnings  
✅ **Declarative** – deklarujesz czego effect potrzebuje  
✅ **Optimized** – effect nie wykonuje się niepotrzebnie  

#### Wady
❌ **Learning curve** – wymaga zrozumienia closures  
❌ **Verbose** – useCallback/useMemo dla funkcji  
❌ **Infinite loops** – łatwo stworzyć przez omyłkę  
❌ **Object references** – zmiany w obiektach mogą nie triggerować  

### ⚠️ Na co uważać?

#### 1. **Nieskończone pętle z objects/arrays**
```typescript
// ❌ BAD: Nowy obiekt przy każdym renderze
function BadComponent() {
    const config = { timeout: 1000 };  // Nowa referencja!
    
    useEffect(() => {
        fetchData(config);
    }, [config]);  // Effect wykonuje się przy każdym renderze!
}

// ✅ GOOD: Stabilna referencja
function GoodComponent() {
    const config = useMemo(() => ({ timeout: 1000 }), []);
    
    useEffect(() => {
        fetchData(config);
    }, [config]);  // Effect tylko raz
}

// ✅ BETTER: Primitive values
function BetterComponent() {
    const timeout = 1000;
    
    useEffect(() => {
        fetchData({ timeout });
    }, [timeout]);  // Primitive, stabilne porównanie
}
```

#### 2. **Funkcje w dependencies bez useCallback**
```typescript
// ❌ BAD: Nowa funkcja przy każdym renderze
function BadSearch() {
    const [query, setQuery] = useState('');
    
    const performSearch = () => {  // Nowa referencja!
        api.search(query);
    };
    
    useEffect(() => {
        performSearch();
    }, [performSearch]);  // Infinite loop!
}

// ✅ GOOD: useCallback
function GoodSearch() {
    const [query, setQuery] = useState('');
    
    const performSearch = useCallback(() => {
        api.search(query);
    }, [query]);  // Re-create tylko gdy query się zmienia
    
    useEffect(() => {
        performSearch();
    }, [performSearch]);  // OK!
}
```

#### 3. **Brakujące dependencies (stale closure)**
```typescript
// ❌ BAD: userId nie w dependencies
function BadProfile() {
    const [userId, setUserId] = useState(1);
    const [profile, setProfile] = useState(null);
    
    useEffect(() => {
        fetchProfile(userId);  // userId użyte!
    }, []);  // ❌ Puste array - stary userId!
    
    // Po zmianie userId, effect się nie wykona!
}

// ✅ GOOD: Wszystkie dependencies
function GoodProfile() {
    const [userId, setUserId] = useState(1);
    const [profile, setProfile] = useState(null);
    
    useEffect(() => {
        fetchProfile(userId);
    }, [userId]);  // ✅ Effect wykonuje się gdy userId się zmienia
}
```

#### 4. **setState w dependencies zamiast functional update**
```typescript
// ❌ BAD: count w dependencies
function BadCounter() {
    const [count, setCount] = useState(0);
    
    useEffect(() => {
        const timer = setInterval(() => {
            setCount(count + 1);  // Stary count!
        }, 1000);
        
        return () => clearInterval(timer);
    }, [count]);  // Effect re-runs co sekundę!
}

// ✅ GOOD: Functional update
function GoodCounter() {
    const [count, setCount] = useState(0);
    
    useEffect(() => {
        const timer = setInterval(() => {
            setCount(c => c + 1);  // Zawsze aktualny count
        }, 1000);
        
        return () => clearInterval(timer);
    }, []);  // Effect tylko raz!
}
```

#### 5. **Props/state w dependencies powodują częste re-runs**
```typescript
// ❌ Problem: Effect wykonuje się za często
function BadComponent({ config }) {
    const [data, setData] = useState(null);
    
    useEffect(() => {
        fetchData(config.apiUrl);
    }, [config]);  // Config może się zmieniać często!
}

// ✅ GOOD: Extract tylko potrzebne properties
function GoodComponent({ config }) {
    const [data, setData] = useState(null);
    const { apiUrl } = config;  // Primitive value
    
    useEffect(() => {
        fetchData(apiUrl);
    }, [apiUrl]);  // Tylko gdy apiUrl się zmienia
}
```

### 🚨 Najczęstsze pomyłki

#### 1. **Puste array dependencies "żeby wykonał się raz"**
```typescript
// ❌ BAD: Ignorowanie eslint warnings
function BadComponent() {
    const [userId, setUserId] = useState(1);
    
    useEffect(() => {
        fetchUser(userId);  // userId użyte!
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);  // ❌ Stary userId zawsze!
}

// ✅ GOOD: Prawidłowe dependencies
function GoodComponent() {
    const [userId, setUserId] = useState(1);
    
    useEffect(() => {
        fetchUser(userId);
    }, [userId]);  // ✅ Reaguje na zmiany
}

// ✅ Jeśli naprawdę chcesz tylko raz:
function GoodComponentOnce() {
    useEffect(() => {
        fetchInitialData();  // Nie używa żadnych dependencies
    }, []);  // OK - brak zewnętrznych wartości
}
```

#### 2. **Całe obiekty w dependencies**
```typescript
// ❌ BAD: Cały obiekt w deps
function BadForm({ formData }) {
    useEffect(() => {
        validateEmail(formData.email);
    }, [formData]);  // Re-run przy każdej zmianie formData!
}

// ✅ GOOD: Tylko potrzebne property
function GoodForm({ formData }) {
    const { email } = formData;
    
    useEffect(() => {
        validateEmail(email);
    }, [email]);  // Re-run tylko gdy email się zmienia
}
```

#### 3. **Async funkcje bezpośrednio w useEffect**
```typescript
// ❌ BAD: async w useEffect
useEffect(async () => {  // ❌ useEffect nie może być async!
    const data = await fetchData();
    setData(data);
}, []);

// ✅ GOOD: Async IIFE lub osobna funkcja
useEffect(() => {
    const loadData = async () => {
        const data = await fetchData();
        setData(data);
    };
    
    loadData();
}, []);

// ✅ BETTER: useCallback dla reusability
function GoodComponent() {
    const loadData = useCallback(async () => {
        const data = await fetchData();
        setData(data);
    }, []);
    
    useEffect(() => {
        loadData();
    }, [loadData]);
}
```

#### 4. **useRef w dependencies**
```typescript
// ❌ BAD: ref w dependencies
function BadComponent() {
    const countRef = useRef(0);
    
    useEffect(() => {
        console.log(countRef.current);
    }, [countRef.current]);  // ❌ Nie triggeruje re-run!
}

// ✅ GOOD: Refs nie potrzebują być w dependencies
function GoodComponent() {
    const countRef = useRef(0);
    
    useEffect(() => {
        console.log(countRef.current);
    }, []);  // Refs są stable, nie zmieniają się
    
    // Jeśli chcesz reaktywności, użyj useState zamiast useRef
}
```

#### 5. **Dispatch/setState w dependencies**
```typescript
// ❌ Niepotrzebne: setState zawsze stabilny
function BadComponent() {
    const [count, setCount] = useState(0);
    
    useEffect(() => {
        setCount(c => c + 1);
    }, [setCount]);  // Niepotrzebne!
}

// ✅ GOOD: Brak setState w deps
function GoodComponent() {
    const [count, setCount] = useState(0);
    
    useEffect(() => {
        setCount(c => c + 1);
    }, []);  // setState jest zawsze stabilny
}
```

### 💼 Kontekst biznesowy

**Scenariusz: Search feature z debouncing**

**Bez prawidłowych dependencies:**
```typescript
// ❌ Bug: Szuka starym query
function BadSearch() {
    const [query, setQuery] = useState('');
    const [results, setResults] = useState([]);
    
    useEffect(() => {
        const timer = setTimeout(() => {
            searchAPI(query);  // query użyte!
        }, 500);
        
        return () => clearTimeout(timer);
    }, []);  // ❌ Pusty array - stary query!
    
    // User types "react" → szuka ""
    // Bug reports: "Search doesn't work!"
}
```

**Z prawidłowymi dependencies:**
```typescript
// ✅ Działa prawidłowo
function GoodSearch() {
    const [query, setQuery] = useState('');
    const [results, setResults] = useState([]);
    
    useEffect(() => {
        const timer = setTimeout(() => {
            searchAPI(query);
        }, 500);
        
        return () => clearTimeout(timer);
    }, [query]);  // ✅ Re-run gdy query się zmienia
    
    // User types "react" → szuka "react"
    // Feature works perfectly!
}
```

**Impact:**
- Bug reports: 10 → 0
- User satisfaction: ↑ 50%
- Support tickets: ↓ 30%
- Development time saved: 5 hours/week

### 📝 Podsumowanie

- **Dependencies** definiują kiedy useEffect się wykonuje
- **Zawsze** dodawaj wszystkie użyte wartości do dependency array
- **Używaj** useCallback dla funkcji, useMemo dla obiektów
- **Uważaj** na infinite loops (objects/arrays), stale closures, async
- **Włącz** ESLint rule `react-hooks/exhaustive-deps`
- **Functional updates** (`setState(prev => ...)`) eliminują potrzebę stanu w deps
