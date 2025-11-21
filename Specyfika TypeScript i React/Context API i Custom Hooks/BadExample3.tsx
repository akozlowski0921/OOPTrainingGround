import React, { createContext, useContext, useState } from 'react';

// ❌ BAD: Więcej context anti-patterns

// BŁĄD 1: Context jako global state dump
interface BadGlobalState {
  user: any;
  theme: string;
  language: string;
  cart: any[];
  notifications: any[];
  settings: any;
  // ...100 innych rzeczy
  // ❌ Mega-context - re-render hell
}

// BŁĄD 2: Brak lazy initialization
export function BadLazyProvider({ children }: any) {
  // ❌ Expensive initialization przy każdym renderze
  const [data] = useState(() => {
    return Array(10000).fill(0).map((_, i) => ({ id: i, name: `Item ${i}` }));
  });

  return <div>{children}</div>;
}

// BŁĄD 3: Context dla derived state
export function BadDerivedStateProvider({ children }: any) {
  const [items, setItems] = useState<number[]>([]);
  const [total, setTotal] = useState(0);
  const [average, setAverage] = useState(0);

  // ❌ Redundant state - można wyliczyć
  const addItem = (item: number) => {
    const newItems = [...items, item];
    setItems(newItems);
    setTotal(newItems.reduce((a, b) => a + b, 0)); // ❌ Duplikacja
    setAverage(newItems.reduce((a, b) => a + b, 0) / newItems.length); // ❌ Duplikacja
  };

  return <div>{children}</div>;
}

// BŁĄD 4: Context bez memoization
export function BadNoMemoProvider({ children }: any) {
  const [state, setState] = useState({ count: 0 });

  // ❌ Nowa funkcja przy każdym renderze
  const increment = () => setState({ count: state.count + 1 });

  // ❌ Nowy obiekt value
  return (
    <div>
      {/* Provider value tworzy nowy obiekt */}
      <MyContext.Provider value={{ state, increment }}>
        {children}
      </MyContext.Provider>
    </div>
  );
}

const MyContext = createContext<any>(null);
