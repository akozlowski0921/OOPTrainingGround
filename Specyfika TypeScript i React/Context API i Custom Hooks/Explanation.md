# Context API + Custom Hooks

## Problem: Prop Drilling

Przekazywanie props przez wiele poziomów komponentów, które ich nie używają.

## Rozwiązanie: Context API

Context pozwala na współdzielenie danych bez prop drilling.

## Problemy w BadExample

### 1. Jeden wielki Context
```tsx
// ❌ Wszystko w jednym Context
interface BadAppContextType {
  user, theme, language, notifications, settings...
}
```

**Problem:** Każdy komponent re-renderuje się przy KAŻDEJ zmianie

### 2. Brak custom hooks
```tsx
// ❌ Duplikacja sprawdzania
const context = useContext(BadAppContext);
if (!context) throw new Error('...');
```

### 3. Nowe referencje przy każdym renderze
```tsx
// ❌ Nowy obiekt
<Context.Provider value={{ user, setUser, ... }}>
```

## Rozwiązania w GoodExample

### 1. Segregacja kontekstów
```tsx
// ✅ Oddzielne konteksty
<UserProvider>
  <ThemeProvider>
    <NotificationsProvider>
```

**Korzyści:**
- Komponenty re-renderują się tylko gdy ich context się zmienia
- Lepsze separation of concerns
- Łatwiejsze testowanie

### 2. Custom hooks
```tsx
// ✅ Reusable hook
export function useUser() {
  const context = useContext(UserContext);
  if (!context) throw new Error('...');
  return context;
}

// Użycie
const { user } = useUser(); // Czysto i type-safe
```

### 3. useMemo dla stabilnych referencji
```tsx
const value = useMemo(() => ({
  user,
  setUser,
  isLoggedIn: user !== null
}), [user]); // ✅ Re-compute tylko gdy user się zmienia
```

### 4. useCallback dla funkcji
```tsx
const toggleTheme = useCallback(() => {
  setTheme(prev => prev === 'light' ? 'dark' : 'light');
}, []); // ✅ Stabilna referencja funkcji
```

## Wzorce

### Pattern 1: Provider composition
```tsx
export function AppProviders({ children }) {
  return (
    <UserProvider>
      <ThemeProvider>
        {children}
      </ThemeProvider>
    </UserProvider>
  );
}
```

### Pattern 2: Custom hooks dla logiki
```tsx
export function useAuth() {
  const { user, setUser } = useUser();
  
  const login = useCallback(async (email, password) => {
    // Login logic
  }, [setUser]);
  
  return { user, login, logout };
}
```

### Pattern 3: Selector pattern
```tsx
// Re-render tylko gdy selected value się zmienia
const userName = useStore(state => state.user?.name);
```

## Performance

### Bez optymalizacji:
- Jeden Context ze wszystkim → każdy update = wszystkie komponenty re-renderują

### Z optymalizacją:
- Segregowane Contexty → tylko relevantne komponenty re-renderują
- useMemo → stabilne referencje
- useCallback → stabilne funkcje

## Best Practices

### DO:
✅ Segreguj konteksty według domeny  
✅ Używaj custom hooks  
✅ useMemo/useCallback dla value  
✅ Throw error gdy Context użyty bez Provider  
✅ Logika biznesowa w custom hooks  

### DON'T:
❌ Jeden wielki Context  
❌ Nowe referencje przy każdym renderze  
❌ Brak error handling  
❌ Bezpośrednie useContext wszędzie  
