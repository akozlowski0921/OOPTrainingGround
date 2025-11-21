# Komunikacja z API

## Fetch API vs Axios

**Fetch API (native):**
```tsx
const response = await fetch(url, { 
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify(data)
});
if (!response.ok) throw new Error('HTTP error');
const data = await response.json();
```

**Axios (library):**
```tsx
const { data } = await axios.post(url, data);
// Automatyczna konwersja JSON, error handling, interceptors
```

**Kiedy używać:**
- **Fetch:** Małe projekty, nie chcesz dodatkowych zależności
- **Axios:** Większe projekty, potrzebujesz interceptors, automatic retries

## Obsługa błędów i loading state

**Discriminated Union dla stanu:**
```tsx
type AsyncState<T> = 
  | { status: 'idle' }
  | { status: 'loading' }
  | { status: 'success'; data: T }
  | { status: 'error'; error: string };
```

**Zalety:**
- Type-safe access do danych
- Niemożliwe niepoprawne stany (np. loading + error jednocześnie)
- Łatwe pattern matching w UI

## AbortController - anulowanie requestów

```tsx
useEffect(() => {
  const controller = new AbortController();
  
  fetch(url, { signal: controller.signal })
    .then(...)
    .catch(err => {
      if (err.name === 'AbortError') return; // Ignoruj abort
      // Handle inne błędy
    });
  
  return () => controller.abort(); // Cleanup
}, [dependency]);
```

**Dlaczego ważne:**
- Zapobiega race conditions (stare requesty nadpisują nowe)
- Zapobiega memory leaks
- Poprawia performance (nie przetwarzamy niepotrzebnych odpowiedzi)

## Automatyczne odświeżanie danych

**Optimistic Updates:**
```tsx
const updateTodo = async (id: number, updates: Partial<Todo>) => {
  // 1. Natychmiast zaktualizuj UI
  setTodos(prev => prev.map(t => t.id === id ? {...t, ...updates} : t));
  
  try {
    // 2. Wyślij request
    await api.updateTodo(id, updates);
  } catch (error) {
    // 3. Rollback przy błędzie
    setTodos(originalTodos);
    throw error;
  }
};
```

**Cache Invalidation:**
```tsx
// Po mutacji - refetch powiązanych danych
const addTodo = async (todo: Todo) => {
  await api.addTodo(todo);
  queryClient.invalidateQueries(['todos']); // React Query
};
```

## Custom Hooks dla API

```tsx
function useApi<T>(url: string) {
  const [state, setState] = useState<AsyncState<T>>({ status: 'idle' });
  
  const execute = useCallback(async () => {
    setState({ status: 'loading' });
    try {
      const data = await fetchApi<T>(url);
      setState({ status: 'success', data });
    } catch (error) {
      setState({ status: 'error', error: error.message });
    }
  }, [url]);
  
  return { ...state, execute, refetch: execute };
}
```

## Libraries dla API management

**React Query (TanStack Query):**
```tsx
const { data, isLoading, error, refetch } = useQuery({
  queryKey: ['todos'],
  queryFn: fetchTodos,
  staleTime: 5000,
});
```

**SWR (Vercel):**
```tsx
const { data, error, isLoading, mutate } = useSWR('/api/todos', fetcher);
```

**Zalety:**
- Automatic caching
- Background refetching
- Optimistic updates
- Deduplication
- Retry on error

## Best Practices

✅ Zawsze obsługuj błędy i loading states  
✅ Używaj AbortController dla cleanup  
✅ Discriminated unions dla type-safe state  
✅ Custom hooks dla reusability  
✅ Optimistic updates dla lepszego UX  
✅ React Query/SWR dla zaawansowanych przypadków  
✅ Proper HTTP status code handling  
✅ Timeout dla długo trwających requestów

❌ Nie ignoruj błędów  
❌ Nie zapomnij o cleanup w useEffect  
❌ Nie rób fetcha w każdym renderze  
❌ Nie używaj console.log dla error handling  
❌ Nie miej race conditions
