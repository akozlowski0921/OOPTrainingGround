import React, { useState, useRef, useEffect, memo } from 'react';

// ✅ GOOD: Proper keys and reconciliation-friendly patterns

interface DataItem {
  id: string;
  value: number;
  timestamp: number;
}

// Proper use of unique, stable keys
export function GoodKeysExample() {
  const [items, setItems] = useState<DataItem[]>([
    { id: 'a', value: 1, timestamp: Date.now() },
    { id: 'b', value: 2, timestamp: Date.now() },
    { id: 'c', value: 3, timestamp: Date.now() }
  ]);

  const addItem = () => {
    // Używamy unique ID jako key
    setItems(prev => [
      { id: `item-${Date.now()}`, value: 0, timestamp: Date.now() },
      ...prev
    ]);
  };

  const removeItem = (id: string) => {
    setItems(prev => prev.filter(item => item.id !== id));
  };

  return (
    <div>
      <button onClick={addItem}>Add Item</button>
      <ul>
        {/* Proper key: unique, stable ID */}
        {items.map(item => (
          <li key={item.id}>
            <ItemEditor item={item} />
            <button onClick={() => removeItem(item.id)}>Remove</button>
          </li>
        ))}
      </ul>
    </div>
  );
}

// Reset component state when key changes
function ItemEditor({ item }: { item: DataItem }) {
  const [localValue, setLocalValue] = useState(item.value);

  // Reset local state gdy item.id się zmienia (nowy element)
  useEffect(() => {
    setLocalValue(item.value);
  }, [item.id, item.value]);

  return (
    <input
      type="number"
      value={localValue}
      onChange={(e) => setLocalValue(Number(e.target.value))}
    />
  );
}

// Stable keys for list items
export function StableKeysExample() {
  const [items, setItems] = useState(['apple', 'banana', 'cherry']);

  const shuffleItems = () => {
    setItems(prev => [...prev].sort(() => Math.random() - 0.5));
  };

  return (
    <div>
      <button onClick={shuffleItems}>Shuffle</button>
      <ul>
        {items.map(item => (
          // Key bazuje na unikalnej wartości item, nie na indexie czy random
          <li key={item}>
            <ExpensiveComponent name={item} />
          </li>
        ))}
      </ul>
    </div>
  );
}

// Memo + stable keys = optimal reconciliation
const ExpensiveComponent = memo(({ name }: { name: string }) => {
  useEffect(() => {
    console.log(`${name} mounted`);
    return () => console.log(`${name} unmounted`);
  }, [name]);

  return <span>{name}</span>;
});

// Proper state management instead of forceUpdate
export function ProperStateManagement() {
  const [items, setItems] = useState<Map<string, string>>(new Map([
    ['a', 'Apple'],
    ['b', 'Banana'],
    ['c', 'Cherry']
  ]));

  const updateItem = (key: string, newValue: string) => {
    // Immutable update - React reconciliation działa prawidłowo
    setItems(prev => new Map(prev).set(key, newValue));
  };

  return (
    <div>
      {Array.from(items.entries()).map(([key, value]) => (
        <div key={key}>
          <input
            value={value}
            onChange={(e) => updateItem(key, e.target.value)}
          />
        </div>
      ))}
    </div>
  );
}

// Unique keys from composite values
export function UniqueCompositeKeys() {
  const items = [
    { category: 'fruit', name: 'apple', id: 1 },
    { category: 'fruit', name: 'banana', id: 2 },
    { category: 'vegetable', name: 'carrot', id: 3 },
    { category: 'vegetable', name: 'potato', id: 4 }
  ];

  return (
    <div>
      {items.map(item => (
        // Używamy unikalnego ID lub composite key
        <div key={item.id}>
          {item.category}: {item.name}
        </div>
      ))}
    </div>
  );
}

// Proper keys in fragments
export function FragmentWithKeys() {
  const sections = [
    { id: 's1', title: 'Section 1', items: ['a', 'b'] },
    { id: 's2', title: 'Section 2', items: ['c', 'd'] }
  ];

  return (
    <div>
      {sections.map(section => (
        // Fragment z key
        <React.Fragment key={section.id}>
          <h2>{section.title}</h2>
          {section.items.map(item => (
            <p key={item}>{item}</p>
          ))}
        </React.Fragment>
      ))}
    </div>
  );
}

// Alternative: Short syntax for fragments with keys
export function ShortFragmentSyntax() {
  const sections = [
    { id: 's1', title: 'Section 1', items: ['a', 'b'] },
    { id: 's2', title: 'Section 2', items: ['c', 'd'] }
  ];

  return (
    <div>
      {sections.map(section => (
        <div key={section.id}>
          <h2>{section.title}</h2>
          {section.items.map(item => (
            <p key={item}>{item}</p>
          ))}
        </div>
      ))}
    </div>
  );
}

// Proper controlled component
export function ControlledInput() {
  // Zawsze inicjalizuj z wartością (nawet pustym stringiem)
  const [value, setValue] = useState('');

  return (
    <div>
      <input
        value={value}
        onChange={(e) => setValue(e.target.value)}
        placeholder="Always controlled"
      />
      <button onClick={() => setValue('Updated')}>
        Update Value
      </button>
    </div>
  );
}

// Pattern: Key reset trick dla wymuszenia remount
export function KeyResetPattern() {
  const [resetKey, setResetKey] = useState(0);
  const [data, setData] = useState({ value: 100 });

  const resetComponent = () => {
    // Zmiana key powoduje demount i remount komponentu
    setResetKey(prev => prev + 1);
  };

  return (
    <div>
      <button onClick={resetComponent}>Reset Component</button>
      <button onClick={() => setData({ value: Math.random() * 100 })}>
        Change Data
      </button>
      {/* Zmiana key powoduje pełny remount */}
      <StatefulComponent key={resetKey} initialValue={data.value} />
    </div>
  );
}

function StatefulComponent({ initialValue }: { initialValue: number }) {
  const [count, setCount] = useState(initialValue);

  return (
    <div>
      <p>Count: {count}</p>
      <button onClick={() => setCount(c => c + 1)}>Increment</button>
    </div>
  );
}

// Reconciliation optimization: React.memo with custom comparison
interface ItemProps {
  item: DataItem;
  onUpdate: (id: string, value: number) => void;
}

const OptimizedItem = memo(
  ({ item, onUpdate }: ItemProps) => {
    console.log(`Rendering item ${item.id}`);
    
    return (
      <div>
        <span>{item.value}</span>
        <button onClick={() => onUpdate(item.id, item.value + 1)}>
          Increment
        </button>
      </div>
    );
  },
  // Custom comparison - tylko re-render gdy item.value się zmienia
  (prevProps, nextProps) => {
    return prevProps.item.value === nextProps.item.value &&
           prevProps.item.id === nextProps.item.id;
  }
);

export function OptimizedList() {
  const [items, setItems] = useState<DataItem[]>([
    { id: 'a', value: 1, timestamp: Date.now() },
    { id: 'b', value: 2, timestamp: Date.now() }
  ]);

  const handleUpdate = (id: string, newValue: number) => {
    setItems(prev => prev.map(item =>
      item.id === id ? { ...item, value: newValue } : item
    ));
  };

  return (
    <div>
      {items.map(item => (
        <OptimizedItem
          key={item.id}
          item={item}
          onUpdate={handleUpdate}
        />
      ))}
    </div>
  );
}
