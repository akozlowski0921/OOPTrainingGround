import React from 'react';

// ❌ BAD: Fighting React's reconciliation with incorrect keys and refs

interface DataItem {
  id: string;
  value: number;
  timestamp: number;
}

// Problem: Używanie index jako key w dynamicznej liście
export function BadKeysExample() {
  const [items, setItems] = React.useState<DataItem[]>([
    { id: 'a', value: 1, timestamp: Date.now() },
    { id: 'b', value: 2, timestamp: Date.now() },
    { id: 'c', value: 3, timestamp: Date.now() }
  ]);

  const addItem = () => {
    // Dodajemy nowy element na początku
    setItems(prev => [
      { id: Math.random().toString(), value: 0, timestamp: Date.now() },
      ...prev
    ]);
  };

  const removeItem = (index: number) => {
    setItems(prev => prev.filter((_, i) => i !== index));
  };

  return (
    <div>
      <button onClick={addItem}>Add Item</button>
      <ul>
        {/* Problem: index jako key - React pomyli elementy podczas re-order */}
        {items.map((item, index) => (
          <li key={index}>
            {/* Input state zostanie przypisany do złego elementu! */}
            <ItemEditor item={item} />
            <button onClick={() => removeItem(index)}>Remove</button>
          </li>
        ))}
      </ul>
    </div>
  );
}

function ItemEditor({ item }: { item: DataItem }) {
  const [localValue, setLocalValue] = React.useState(item.value);

  // Problem: Stan komponentu nie jest resetowany gdy key się nie zmienia
  // ale item się zmienia (bo używamy index jako key)
  return (
    <input
      type="number"
      value={localValue}
      onChange={(e) => setLocalValue(Number(e.target.value))}
    />
  );
}

// Problem: Niestabilne keys - zmieniają się przy każdym renderze
export function UnstableKeysExample() {
  const [items, setItems] = React.useState(['apple', 'banana', 'cherry']);

  const shuffleItems = () => {
    setItems(prev => [...prev].sort(() => Math.random() - 0.5));
  };

  return (
    <div>
      <button onClick={shuffleItems}>Shuffle</button>
      <ul>
        {items.map(item => (
          // Problem: key bazuje na Math.random() - zawsze nowy!
          // React zdemontuje i zamontuje każdy element przy każdym renderze
          <li key={`${item}-${Math.random()}`}>
            <ExpensiveComponent name={item} />
          </li>
        ))}
      </ul>
    </div>
  );
}

function ExpensiveComponent({ name }: { name: string }) {
  React.useEffect(() => {
    console.log(`${name} mounted - expensive operation!`);
    return () => console.log(`${name} unmounted`);
  }, [name]);

  return <span>{name}</span>;
}

// Problem: Nadużywanie forceUpdate i refs do obejścia reconciliation
export class ForceUpdateAntipattern extends React.Component<
  { items: string[] },
  {}
> {
  private cache = new Map<string, React.RefObject<HTMLDivElement>>();

  // Problem: Przechowywanie refs w cache zamiast pozwolić React zarządzać
  getRef(key: string) {
    if (!this.cache.has(key)) {
      this.cache.set(key, React.createRef<HTMLDivElement>());
    }
    return this.cache.get(key)!;
  }

  updateItemDirectly(key: string, newValue: string) {
    // Problem: Bezpośrednia modyfikacja DOM przez ref
    const ref = this.getRef(key);
    if (ref.current) {
      ref.current.textContent = newValue;
    }

    // Problem: forceUpdate bo zmiany w DOM nie są w state
    this.forceUpdate();
  }

  render() {
    return (
      <div>
        {this.props.items.map(item => (
          // Problem: Refs cache może prowadzić do stale references
          // co łamie React reconciliation
          <div key={item} ref={this.getRef(item)}>
            {item}
          </div>
        ))}
      </div>
    );
  }
}

// Problem: Ignorowanie key warning przez duplicate keys
export function DuplicateKeysExample() {
  const items = [
    { category: 'fruit', name: 'apple' },
    { category: 'fruit', name: 'banana' },
    { category: 'vegetable', name: 'carrot' },
    { category: 'vegetable', name: 'potato' }
  ];

  return (
    <div>
      {items.map(item => (
        // Problem: category nie jest unique - duplicate keys!
        // React nie będzie mógł prawidłowo śledzić elementów
        <div key={item.category}>
          {item.name}
        </div>
      ))}
    </div>
  );
}

// Problem: Keys w fragmentach - łatwo zapomnieć
export function FragmentKeysExample() {
  const sections = [
    { title: 'Section 1', items: ['a', 'b'] },
    { title: 'Section 2', items: ['c', 'd'] }
  ];

  return (
    <div>
      {sections.map(section => (
        // Problem: Fragment bez key - React warning
        // React nie może śledzić które fragmenty się zmieniły
        <React.Fragment>
          <h2>{section.title}</h2>
          {section.items.map(item => (
            <p key={item}>{item}</p>
          ))}
        </React.Fragment>
      ))}
    </div>
  );
}

// Problem: Mieszanie controlled i uncontrolled components
export function MixedControlExample() {
  const [value, setValue] = React.useState<string | undefined>(undefined);

  return (
    <div>
      {/* Problem: Zmiana z uncontrolled na controlled */}
      <input
        value={value} // undefined na początku = uncontrolled
        onChange={(e) => setValue(e.target.value)}
      />
      <button onClick={() => setValue('controlled')}>
        Make Controlled
      </button>
    </div>
  );
}
