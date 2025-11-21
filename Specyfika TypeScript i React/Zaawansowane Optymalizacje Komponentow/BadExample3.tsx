import React, { useState } from 'react';

// ❌ BAD: Not using React DevTools Profiler to identify performance bottlenecks

// Problem: Nie wiemy które komponenty są wolne i dlaczego
export function SlowApplication() {
  const [data, setData] = useState(generateData());

  function generateData() {
    return Array.from({ length: 100 }, (_, i) => ({
      id: i,
      value: Math.random() * 1000
    }));
  }

  return (
    <div>
      <button onClick={() => setData(generateData())}>Refresh Data</button>
      <DataGrid data={data} />
    </div>
  );
}

// Problem: Komponenty without performance profiling
function DataGrid({ data }: { data: Array<{ id: number; value: number }> }) {
  // Problem: Kosztowne obliczenia bez widoczności w Profiler
  const processedData = data.map(item => ({
    ...item,
    formatted: formatValue(item.value),
    category: categorize(item.value)
  }));

  return (
    <div>
      {processedData.map(item => (
        <DataRow key={item.id} item={item} />
      ))}
    </div>
  );
}

function DataRow({ item }: { item: any }) {
  // Problem: Re-renders bez możliwości łatwego zidentyfikowania w Profiler
  console.log('Rendering DataRow', item.id); // Podstawowy debugging
  
  return (
    <div>
      <span>{item.id}</span>
      <span>{item.formatted}</span>
      <span>{item.category}</span>
    </div>
  );
}

function formatValue(value: number): string {
  // Symulacja kosztownej operacji
  let result = value.toFixed(2);
  for (let i = 0; i < 1000; i++) {
    result = result.split('').reverse().join('');
  }
  return result;
}

function categorize(value: number): string {
  // Kosztowna kategoryzacja
  if (value < 200) return 'Low';
  if (value < 500) return 'Medium';
  if (value < 800) return 'High';
  return 'Very High';
}

// Problem: Brak insights do commit phases
export function ComplexForm() {
  const [formState, setFormState] = useState({
    name: '',
    email: '',
    address: '',
    city: '',
    country: ''
  });

  const handleChange = (field: string, value: string) => {
    // Problem: Nie wiemy ile czasu zajmuje update
    setFormState(prev => ({ ...prev, [field]: value }));
  };

  return (
    <div>
      {/* Problem: Każde pole re-renderuje całą formę */}
      <input
        value={formState.name}
        onChange={(e) => handleChange('name', e.target.value)}
        placeholder="Name"
      />
      <input
        value={formState.email}
        onChange={(e) => handleChange('email', e.target.value)}
        placeholder="Email"
      />
      <ExpensiveValidation formState={formState} />
      <ExpensiveSuggestions formState={formState} />
    </div>
  );
}

function ExpensiveValidation({ formState }: { formState: any }) {
  // Problem: Kosztowna walidacja bez insights
  const errors = Object.keys(formState).filter(key => {
    // Symulacja walidacji
    for (let i = 0; i < 10000; i++) {
      Math.random();
    }
    return !formState[key];
  });

  return <div>Errors: {errors.length}</div>;
}

function ExpensiveSuggestions({ formState }: { formState: any }) {
  // Problem: Kosztowne sugestie bez możliwości zmierzenia
  const suggestions = [];
  for (let i = 0; i < 100; i++) {
    suggestions.push(`Suggestion ${i}`);
  }

  return <div>Suggestions: {suggestions.length}</div>;
}

// Problem: Cascading updates bez widoczności
export function CascadingUpdates() {
  const [trigger, setTrigger] = useState(0);

  return (
    <div>
      <button onClick={() => setTrigger(t => t + 1)}>Trigger Update</button>
      <Level1 trigger={trigger} />
    </div>
  );
}

function Level1({ trigger }: { trigger: number }) {
  const [state1, setState1] = useState(0);
  
  React.useEffect(() => {
    setState1(trigger * 2);
  }, [trigger]);

  return <Level2 state={state1} />;
}

function Level2({ state }: { state: number }) {
  const [state2, setState2] = useState(0);
  
  React.useEffect(() => {
    setState2(state * 3);
  }, [state]);

  return <Level3 state={state2} />;
}

function Level3({ state }: { state: number }) {
  // Problem: Nie wiemy ile czasu zajmuje cała cascada
  return <div>Final state: {state}</div>;
}

// Problem: Memory leaks bez monitoringu
export function PotentialMemoryLeak() {
  const [data, setData] = useState<any[]>([]);

  React.useEffect(() => {
    const interval = setInterval(() => {
      // Problem: Potential memory leak - nie wiemy ile to zajmuje
      setData(prev => [...prev, { id: Date.now(), value: Math.random() }]);
    }, 100);

    // Missing cleanup - memory leak!
    // return () => clearInterval(interval);
  }, []);

  return <div>Data points: {data.length}</div>;
}

// Problem: Nie profilujemy różnych scenariuszy
export function UntestedScenarios() {
  const [scenario, setScenario] = useState<'small' | 'medium' | 'large'>('small');

  const dataSize = {
    small: 10,
    medium: 100,
    large: 1000
  };

  const data = Array.from({ length: dataSize[scenario] }, (_, i) => i);

  // Problem: Nie wiemy jak aplikacja się zachowuje przy różnych rozmiarach danych
  return (
    <div>
      <select value={scenario} onChange={(e) => setScenario(e.target.value as any)}>
        <option value="small">Small</option>
        <option value="medium">Medium</option>
        <option value="large">Large</option>
      </select>
      
      {data.map(item => (
        <ExpensiveItem key={item} value={item} />
      ))}
    </div>
  );
}

function ExpensiveItem({ value }: { value: number }) {
  // Symulacja kosztownego komponentu
  const result = Array.from({ length: 100 }, () => Math.random()).reduce((a, b) => a + b);
  return <div>{value}: {result}</div>;
}
