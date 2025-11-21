# Zarządzanie Stanem Aplikacji

## Context API vs Redux

### Context API

**Kiedy używać:**
- Mała do średniej aplikacja
- Stan globalny używany w kilku miejscach (theme, auth, language)
- Nie potrzebujesz time-travel debugging
- Nie potrzebujesz middleware

**Zalety:**
- Built-in w React
- Prosty setup
- Dobry dla prostych przypadków

**Wady:**
- Każda zmiana context value powoduje re-render wszystkich konsumentów
- Brak built-in dev tools
- Trudniejsze do debugowania w dużych aplikacjach

### Redux (Redux Toolkit)

**Kiedy używać:**
- Duża aplikacja z złożonym stanem
- Stan współdzielony między wieloma komponentami
- Potrzebujesz middleware (async, logging, persistence)
- Potrzebujesz time-travel debugging
- Zespół potrzebuje predictable state management

**Zalety:**
- Przewidywalny stan (single source of truth)
- Excellent dev tools (time-travel, state inspection)
- Middleware ecosystem
- Testability
- Performance optimization przez selectors

**Wady:**
- Więcej boilerplate (choć Redux Toolkit to minimalizuje)
- Dodatkowa zależność
- Krzywa uczenia się

## Segregacja kontekstów

```tsx
// ❌ BAD: Jeden wielki context
const AppContext = createContext({ user, theme, notifications, ... });

// ✅ GOOD: Segregowane konteksty
const UserContext = createContext({ user, setUser });
const ThemeContext = createContext({ theme, setTheme });
const NotificationsContext = createContext({ notifications, addNotification });
```

**Dlaczego:**
- Komponent rerenderuje się tylko gdy zmienia się jego context
- Lepszy code organization
- Łatwiejsze testowanie

## Memoizacja w Context

```tsx
export function UserProvider({ children }) {
  const [user, setUser] = useState(null);
  
  // ✅ Memoizacja value - zapobiega niepotrzebnym re-renderom
  const value = useMemo(() => ({ user, setUser }), [user]);
  
  return <UserContext.Provider value={value}>{children}</UserContext.Provider>;
}
```

## Redux Middleware

### redux-thunk (built-in w Redux Toolkit)

```tsx
// Async action creator
export const fetchUser = createAsyncThunk(
  'user/fetch',
  async (userId: number) => {
    const response = await api.getUser(userId);
    return response.data;
  }
);

// W komponencie
dispatch(fetchUser(123));
```

**Kiedy używać:**
- Proste async operations
- Większość przypadków użycia

### redux-saga

```tsx
function* fetchUserSaga(action) {
  try {
    const user = yield call(api.getUser, action.payload);
    yield put({ type: 'USER_FETCH_SUCCESS', payload: user });
  } catch (error) {
    yield put({ type: 'USER_FETCH_ERROR', error });
  }
}
```

**Kiedy używać:**
- Złożone async flows (retry, debounce, race conditions)
- Potrzebujesz testability async logic
- Background tasks
- Complex orchestration

## Normalized State

```tsx
// ❌ BAD: Nested state
{
  users: [
    { id: 1, name: 'John', todos: [...] }
  ]
}

// ✅ GOOD: Normalized state
{
  users: {
    byId: { 1: { id: 1, name: 'John' } },
    allIds: [1]
  },
  todos: {
    byId: { 1: { id: 1, userId: 1, title: '...' } },
    allIds: [1]
  }
}
```

**Zalety:**
- Brak duplikacji danych
- Łatwe updates
- Lepszy performance
- Proste relationships

## Reselect - Memoized Selectors

```tsx
// Selector bez memoizacji - wykonuje się przy każdym renderze
const selectCompletedTodos = (state) => 
  state.todos.filter(t => t.completed);

// ✅ Memoized selector - wykonuje się tylko gdy zmieni się input
const selectCompletedTodos = createSelector(
  [state => state.todos],
  (todos) => todos.filter(t => t.completed)
);
```

**Zalety:**
- Cachowanie wyników
- Zapobiega re-renders
- Composable (można łączyć selektory)
- Lepszy performance

## Optymalizacje wydajności

### 1. Selective subscribing

```tsx
// ❌ BAD: Subscribe do całego state
const state = useSelector(state => state);

// ✅ GOOD: Subscribe tylko do potrzebnych danych
const todos = useSelector(state => state.todos.items);
```

### 2. Equality checks

```tsx
import { shallowEqual } from 'react-redux';

// Shallow comparison dla obiektów/tablic
const { todos, loading } = useSelector(
  state => ({ todos: state.todos.items, loading: state.todos.loading }),
  shallowEqual
);
```

### 3. Split reducers

```tsx
const rootReducer = combineReducers({
  user: userReducer,
  todos: todosReducer,
  notifications: notificationsReducer,
});
```

### 4. Code splitting

```tsx
// Lazy load reducer
const injectReducer = (key, reducer) => {
  store.replaceReducer(
    combineReducers({
      ...staticReducers,
      [key]: reducer,
    })
  );
};
```

## Best Practices

✅ Context API dla prostych przypadków (theme, auth)  
✅ Redux dla złożonych aplikacji  
✅ Segreguj konteksty według domeny  
✅ Memoizuj context values  
✅ Używaj Redux Toolkit (nie raw Redux)  
✅ Async logic w thunks/sagas, nie w komponentach  
✅ Normalized state dla relational data  
✅ Reselect dla memoized computations  
✅ Type safety z TypeScript  
✅ DevTools dla debugowania

❌ Nie używaj Redux gdy Context wystarcza  
❌ Nie łącz wszystkiego w jeden Context  
❌ Nie przechowuj computed values w state  
❌ Nie mutuj state bezpośrednio (używaj Immer)  
❌ Nie rób async w reducerach
