import React, { createContext, useContext, useState, useMemo, useCallback, ReactNode } from 'react';

// ✅ GOOD: Optimized context patterns

// ✅ Split state and actions
interface CounterState {
  count: number;
}

interface CounterActions {
  increment: () => void;
  decrement: () => void;
}

const CounterStateContext = createContext<CounterState | undefined>(undefined);
const CounterActionsContext = createContext<CounterActions | undefined>(undefined);

export function GoodCounterProvider({ children }: { children: ReactNode }) {
  const [count, setCount] = useState(0);

  // ✅ Stable actions reference
  const actions = useMemo(() => ({
    increment: () => setCount(c => c + 1),
    decrement: () => setCount(c => c - 1)
  }), []);

  // ✅ Separate state - components subscribe only to what they need
  const state = useMemo(() => ({ count }), [count]);

  return (
    <CounterStateContext.Provider value={state}>
      <CounterActionsContext.Provider value={actions}>
        {children}
      </CounterActionsContext.Provider>
    </CounterStateContext.Provider>
  );
}

// ✅ Custom hooks with proper error handling
export function useCounterState() {
  const context = useContext(CounterStateContext);
  if (!context) {
    throw new Error('useCounterState must be used within CounterProvider');
  }
  return context;
}

export function useCounterActions() {
  const context = useContext(CounterActionsContext);
  if (!context) {
    throw new Error('useCounterActions must be used within CounterProvider');
  }
  return context;
}

// ✅ Components subscribe to specific context
export function GoodDisplay() {
  const { count } = useCounterState(); // Only re-renders when count changes
  return <div>Count: {count}</div>;
}

export function GoodControls() {
  const { increment, decrement } = useCounterActions(); // Never re-renders
  return (
    <div>
      <button onClick={increment}>+</button>
      <button onClick={decrement}>-</button>
    </div>
  );
}

// ✅ Selector pattern
export function useCounterSelector<T>(selector: (state: CounterState) => T): T {
  const state = useCounterState();
  return useMemo(() => selector(state), [state, selector]);
}

export function SelectiveComponent() {
  // ✅ Re-renders only when count > 10 changes
  const isHighCount = useCounterSelector(state => state.count > 10);
  return <div>High count: {isHighCount ? 'Yes' : 'No'}</div>;
}
