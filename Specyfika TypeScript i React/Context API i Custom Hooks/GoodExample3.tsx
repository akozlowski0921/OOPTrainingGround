import React, { createContext, useContext, useState, useMemo, useReducer, ReactNode } from 'react';

// ✅ GOOD: Advanced context patterns

// ✅ Reducer pattern dla complex state
type TodoAction =
  | { type: 'ADD'; payload: string }
  | { type: 'REMOVE'; payload: number }
  | { type: 'TOGGLE'; payload: number };

interface Todo {
  id: number;
  text: string;
  completed: boolean;
}

interface TodoState {
  todos: Todo[];
}

function todoReducer(state: TodoState, action: TodoAction): TodoState {
  switch (action.type) {
    case 'ADD':
      return {
        todos: [...state.todos, { id: Date.now(), text: action.payload, completed: false }]
      };
    case 'REMOVE':
      return {
        todos: state.todos.filter(t => t.id !== action.payload)
      };
    case 'TOGGLE':
      return {
        todos: state.todos.map(t =>
          t.id === action.payload ? { ...t, completed: !t.completed } : t
        )
      };
    default:
      return state;
  }
}

const TodoContext = createContext<{
  state: TodoState;
  dispatch: React.Dispatch<TodoAction>;
} | undefined>(undefined);

export function TodoProvider({ children }: { children: ReactNode }) {
  const [state, dispatch] = useReducer(todoReducer, { todos: [] });

  const value = useMemo(() => ({ state, dispatch }), [state]);

  return <TodoContext.Provider value={value}>{children}</TodoContext.Provider>;
}

// ✅ Derived state computed on the fly
export function useTodos() {
  const context = useContext(TodoContext);
  if (!context) throw new Error('useTodos must be used within TodoProvider');
  
  // ✅ Computed values
  const { state, dispatch } = context;
  const completedCount = useMemo(
    () => state.todos.filter(t => t.completed).length,
    [state.todos]
  );
  const totalCount = state.todos.length;

  return {
    todos: state.todos,
    completedCount,
    totalCount,
    dispatch
  };
}

// ✅ Custom hooks for common operations
export function useTodoActions() {
  const context = useContext(TodoContext);
  if (!context) throw new Error('...');
  
  const { dispatch } = context;

  return useMemo(() => ({
    addTodo: (text: string) => dispatch({ type: 'ADD', payload: text }),
    removeTodo: (id: number) => dispatch({ type: 'REMOVE', payload: id }),
    toggleTodo: (id: number) => dispatch({ type: 'TOGGLE', payload: id })
  }), [dispatch]);
}

// ✅ Lazy initialization
export function LazyProvider({ children }: { children: ReactNode }) {
  const [data] = useState(() => {
    // ✅ Funkcja wykonana tylko raz
    return Array(10000).fill(0).map((_, i) => ({ id: i, name: `Item ${i}` }));
  });

  return <div>{children}</div>;
}
