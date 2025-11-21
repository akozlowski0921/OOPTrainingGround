// ✅ GOOD: Redux Toolkit z TypeScript, redux-thunk, proper typing, async w thunks
import { configureStore, createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { useSelector, useDispatch, TypedUseSelectorHook } from 'react-redux';

interface Todo {
  id: number;
  title: string;
  completed: boolean;
}

interface TodosState {
  items: Todo[];
  loading: boolean;
  error: string | null;
}

// Async thunk - async logic oddzielona od komponentów
export const fetchTodos = createAsyncThunk(
  'todos/fetchTodos',
  async () => {
    const response = await fetch('https://api.example.com/todos');
    if (!response.ok) throw new Error('Failed to fetch');
    return response.json() as Promise<Todo[]>;
  }
);

export const addTodo = createAsyncThunk(
  'todos/addTodo',
  async (title: string) => {
    const response = await fetch('https://api.example.com/todos', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ title, completed: false }),
    });
    if (!response.ok) throw new Error('Failed to add todo');
    return response.json() as Promise<Todo>;
  }
);

// Redux Toolkit slice - automatic action creators, immutable updates
const todosSlice = createSlice({
  name: 'todos',
  initialState: {
    items: [],
    loading: false,
    error: null,
  } as TodosState,
  reducers: {
    toggleTodo: (state, action: PayloadAction<number>) => {
      const todo = state.items.find(t => t.id === action.payload);
      if (todo) {
        todo.completed = !todo.completed; // Immer pozwala na mutable syntax
      }
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch todos
      .addCase(fetchTodos.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchTodos.fulfilled, (state, action) => {
        state.loading = false;
        state.items = action.payload;
      })
      .addCase(fetchTodos.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to fetch todos';
      })
      // Add todo
      .addCase(addTodo.fulfilled, (state, action) => {
        state.items.push(action.payload);
      });
  },
});

export const { toggleTodo } = todosSlice.actions;

// Store configuration
const store = configureStore({
  reducer: {
    todos: todosSlice.reducer,
  },
});

// Type-safe hooks
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;

// Komponent - czysty, tylko dispatch akcji
function TodoList() {
  const dispatch = useAppDispatch();
  const { items: todos, loading, error } = useAppSelector(state => state.todos);

  // Async logic w thunks - łatwe do testowania i reuse
  const handleFetchTodos = () => {
    dispatch(fetchTodos());
  };

  const handleAddTodo = () => {
    dispatch(addTodo('New Todo'));
  };

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div>
      <button onClick={handleFetchTodos}>Refresh</button>
      <button onClick={handleAddTodo}>Add Todo</button>
      {todos.map(todo => (
        <div key={todo.id} onClick={() => dispatch(toggleTodo(todo.id))}>
          {todo.title} - {todo.completed ? '✓' : '○'}
        </div>
      ))}
    </div>
  );
}

export default TodoList;
