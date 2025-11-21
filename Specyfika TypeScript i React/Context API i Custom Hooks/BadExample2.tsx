import React, { createContext, useContext, useState, ReactNode } from 'react';

// ❌ BAD: Context performance issues część 2

// BŁĄD 1: Context z częstymi zmianami
interface BadCounterContextType {
  count: number;
  increment: () => void;
  decrement: () => void;
  // Każda zmiana count re-renderuje wszystkich konsumentów!
}

const BadCounterContext = createContext<BadCounterContextType | undefined>(undefined);

export function BadCounterProvider({ children }: { children: ReactNode }) {
  const [count, setCount] = useState(0);

  // ❌ Nowy obiekt przy każdym renderze
  const value = {
    count,
    increment: () => setCount(c => c + 1),
    decrement: () => setCount(c => c - 1)
  };

  return <BadCounterContext.Provider value={value}>{children}</BadCounterContext.Provider>;
}

// BŁĄD 2: useContext bez sprawdzenia undefined
export function BadCounter() {
  const context = useContext(BadCounterContext);
  // ❌ Brak sprawdzenia - może być undefined
  return <div>Count: {context.count}</div>;
}

// BŁĄD 3: Duplikacja context logic
export function BadDisplay1() {
  const context = useContext(BadCounterContext);
  if (!context) throw new Error('...');
  return <div>{context.count}</div>;
}

export function BadDisplay2() {
  const context = useContext(BadCounterContext);
  if (!context) throw new Error('...'); // ❌ Duplikacja
  return <div>{context.count}</div>;
}

// BŁĄD 4: Tightly coupled components
interface BadFormContextType {
  values: Record<string, any>;
  setFieldValue: (name: string, value: any) => void;
  errors: Record<string, string>;
  // ❌ Wszystko w jednym - trudno testować
}
