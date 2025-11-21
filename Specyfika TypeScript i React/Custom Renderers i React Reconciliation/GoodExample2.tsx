import React, { useState, useReducer, useTransition } from 'react';

// ✅ GOOD: Let React handle reconciliation and Fiber tree management

interface ListItem {
  id: number;
  text: string;
  completed: boolean;
}

type TodoAction =
  | { type: 'ADD'; text: string }
  | { type: 'TOGGLE'; id: number }
  | { type: 'REMOVE'; id: number };

// Proper state management with reducer
function todoReducer(state: ListItem[], action: TodoAction): ListItem[] {
  switch (action.type) {
    case 'ADD':
      return [...state, {
        id: Date.now(),
        text: action.text,
        completed: false
      }];
    case 'TOGGLE':
      return state.map(item =>
        item.id === action.id ? { ...item, completed: !item.completed } : item
      );
    case 'REMOVE':
      return state.filter(item => item.id !== action.id);
    default:
      return state;
  }
}

// React obsługuje wszystkie aktualizacje DOM przez reconciliation
export function TodoListReact() {
  const [items, dispatch] = useReducer(todoReducer, [
    { id: 1, text: 'Learn React', completed: false },
    { id: 2, text: 'Learn TypeScript', completed: false }
  ]);

  // React automatycznie zarządza DOM updates
  return (
    <div>
      <ul>
        {items.map(item => (
          <TodoItem
            key={item.id}
            item={item}
            onToggle={() => dispatch({ type: 'TOGGLE', id: item.id })}
            onRemove={() => dispatch({ type: 'REMOVE', id: item.id })}
          />
        ))}
      </ul>
      <button onClick={() => dispatch({ type: 'ADD', text: 'New item' })}>
        Add Item
      </button>
    </div>
  );
}

// Komponent z animacją - React obsługuje lifecycle
function TodoItem({ 
  item, 
  onToggle, 
  onRemove 
}: { 
  item: ListItem; 
  onToggle: () => void;
  onRemove: () => void;
}) {
  const [isRemoving, setIsRemoving] = useState(false);

  // Animacja przez React state, nie przez DOM manipulation
  const handleRemove = () => {
    setIsRemoving(true);
    // Po animacji wywołaj faktyczne usunięcie
    setTimeout(onRemove, 300);
  };

  return (
    <li 
      className={item.completed ? 'completed' : ''}
      style={{
        opacity: isRemoving ? 0 : 1,
        transition: 'opacity 0.3s'
      }}
    >
      {item.text}
      <button onClick={onToggle}>Toggle</button>
      <button onClick={handleRemove}>Remove</button>
    </li>
  );
}

// Concurrent Features - React 18+ wykorzystuje Fiber architecture
export function OptimizedTodoList() {
  const [items, dispatch] = useReducer(todoReducer, []);
  const [filterText, setFilterText] = useState('');
  const [isPending, startTransition] = useTransition();

  // Używamy useTransition dla non-urgent updates
  const handleFilterChange = (text: string) => {
    startTransition(() => {
      setFilterText(text);
    });
  };

  // React Fiber pozwala na przerwanie i wznowienie renderowania
  const filteredItems = items.filter(item =>
    item.text.toLowerCase().includes(filterText.toLowerCase())
  );

  return (
    <div>
      <input
        type="text"
        placeholder="Filter..."
        onChange={(e) => handleFilterChange(e.target.value)}
      />
      {isPending && <span>Filtering...</span>}
      
      <ul style={{ opacity: isPending ? 0.6 : 1 }}>
        {filteredItems.map(item => (
          <TodoItem
            key={item.id}
            item={item}
            onToggle={() => dispatch({ type: 'TOGGLE', id: item.id })}
            onRemove={() => dispatch({ type: 'REMOVE', id: item.id })}
          />
        ))}
      </ul>
      
      <button onClick={() => dispatch({ type: 'ADD', text: 'New item' })}>
        Add Item
      </button>
    </div>
  );
}

// Zaawansowane: Używanie flushSync dla synchronicznych updates (rzadko potrzebne)
import { flushSync } from 'react-dom';

export function SynchronousUpdates() {
  const [count, setCount] = useState(0);
  const [flag, setFlag] = useState(false);

  const handleClick = () => {
    // Normalnie React batchuje updates
    setCount(c => c + 1);
    setFlag(f => !f);

    // flushSync wymusza natychmiastowe wykonanie (synchroniczne)
    // Używaj tylko gdy absolutnie konieczne (np. pomiary DOM)
    flushSync(() => {
      setCount(c => c + 1);
    });

    // Ten console.log pokaże zaktualizowany count
    console.log('Count after flushSync:', count);
  };

  return (
    <div>
      <p>Count: {count}</p>
      <p>Flag: {flag ? 'true' : 'false'}</p>
      <button onClick={handleClick}>Update</button>
    </div>
  );
}

// Profiler API - insight do React Fiber performance
export function ProfiledComponent() {
  const onRenderCallback = (
    id: string,
    phase: 'mount' | 'update',
    actualDuration: number,
    baseDuration: number,
    startTime: number,
    commitTime: number
  ) => {
    console.log(`${id} (${phase}) took ${actualDuration}ms`);
  };

  return (
    <React.Profiler id="TodoList" onRender={onRenderCallback}>
      <OptimizedTodoList />
    </React.Profiler>
  );
}

// Custom hook pokazujący jak React zarządza hookami w Fiber
export function useDebugFiberInfo(componentName: string) {
  React.useEffect(() => {
    console.log(`${componentName} mounted`);
    return () => console.log(`${componentName} unmounted`);
  }, [componentName]);

  React.useEffect(() => {
    console.log(`${componentName} updated`);
  });

  // React Fiber automatycznie śledzi kolejność hooków
  // Nie próbuj "hakować" tego mechanizmu
  return { componentName };
}
