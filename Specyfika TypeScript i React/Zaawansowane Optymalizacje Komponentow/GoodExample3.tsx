import React, { useState, useMemo, memo, useCallback, Profiler } from 'react';

// ✅ GOOD: Using React DevTools Profiler and Profiler API

// Profiler callback do logowania performance metrics
function onRenderCallback(
  id: string,
  phase: 'mount' | 'update',
  actualDuration: number,
  baseDuration: number,
  startTime: number,
  commitTime: number
) {
  console.log(`[Profiler] ${id} (${phase})`);
  console.log(`  Actual duration: ${actualDuration.toFixed(2)}ms`);
  console.log(`  Base duration: ${baseDuration.toFixed(2)}ms`);
  console.log(`  Start time: ${startTime.toFixed(2)}ms`);
  console.log(`  Commit time: ${commitTime.toFixed(2)}ms`);
}

export function ProfiledApplication() {
  const [data, setData] = useState(generateData());

  function generateData() {
    return Array.from({ length: 100 }, (_, i) => ({
      id: i,
      value: Math.random() * 1000
    }));
  }

  return (
    <Profiler id="Application" onRender={onRenderCallback}>
      <div>
        <button onClick={() => setData(generateData())}>Refresh Data</button>
        <Profiler id="DataGrid" onRender={onRenderCallback}>
          <DataGridOptimized data={data} />
        </Profiler>
      </div>
    </Profiler>
  );
}

// Optimized with memoization
const DataGridOptimized = memo(({ 
  data 
}: { 
  data: Array<{ id: number; value: number }> 
}) => {
  // useMemo dla kosztownych obliczeń
  const processedData = useMemo(() => {
    console.log('Processing data...');
    return data.map(item => ({
      ...item,
      formatted: formatValue(item.value),
      category: categorize(item.value)
    }));
  }, [data]);

  return (
    <div>
      {processedData.map(item => (
        <DataRowOptimized key={item.id} item={item} />
      ))}
    </div>
  );
});

const DataRowOptimized = memo(({ item }: { item: any }) => {
  console.log('Rendering DataRow', item.id);
  
  return (
    <div>
      <span>{item.id}</span>
      <span>{item.formatted}</span>
      <span>{item.category}</span>
    </div>
  );
});

// Memoized expensive functions
const formatValue = (value: number): string => {
  let result = value.toFixed(2);
  for (let i = 0; i < 1000; i++) {
    result = result.split('').reverse().join('');
  }
  return result;
};

const categorize = (value: number): string => {
  if (value < 200) return 'Low';
  if (value < 500) return 'Medium';
  if (value < 800) return 'High';
  return 'Very High';
};

// Profiled complex form with insights
export function ProfiledComplexForm() {
  const [formState, setFormState] = useState({
    name: '',
    email: '',
    address: '',
    city: '',
    country: ''
  });

  const handleChange = useCallback((field: string, value: string) => {
    setFormState(prev => ({ ...prev, [field]: value }));
  }, []);

  return (
    <Profiler id="ComplexForm" onRender={onRenderCallback}>
      <div>
        <Profiler id="FormInputs" onRender={onRenderCallback}>
          <FormInput
            label="Name"
            value={formState.name}
            onChange={(v) => handleChange('name', v)}
          />
          <FormInput
            label="Email"
            value={formState.email}
            onChange={(v) => handleChange('email', v)}
          />
        </Profiler>
        
        <Profiler id="Validation" onRender={onRenderCallback}>
          <OptimizedValidation formState={formState} />
        </Profiler>
        
        <Profiler id="Suggestions" onRender={onRenderCallback}>
          <OptimizedSuggestions formState={formState} />
        </Profiler>
      </div>
    </Profiler>
  );
}

const FormInput = memo(({ 
  label, 
  value, 
  onChange 
}: { 
  label: string; 
  value: string; 
  onChange: (value: string) => void;
}) => (
  <input
    value={value}
    onChange={(e) => onChange(e.target.value)}
    placeholder={label}
  />
));

const OptimizedValidation = memo(({ formState }: { formState: any }) => {
  const errors = useMemo(() => {
    console.log('Running validation...');
    return Object.keys(formState).filter(key => {
      for (let i = 0; i < 10000; i++) {
        Math.random();
      }
      return !formState[key];
    });
  }, [formState]);

  return <div>Errors: {errors.length}</div>;
});

const OptimizedSuggestions = memo(({ formState }: { formState: any }) => {
  const suggestions = useMemo(() => {
    console.log('Generating suggestions...');
    const result = [];
    for (let i = 0; i < 100; i++) {
      result.push(`Suggestion ${i}`);
    }
    return result;
  }, [formState.email, formState.name]); // Only depend on relevant fields

  return <div>Suggestions: {suggestions.length}</div>;
});

// Profiled cascading updates
export function ProfiledCascadingUpdates() {
  const [trigger, setTrigger] = useState(0);

  return (
    <Profiler id="CascadingUpdates" onRender={onRenderCallback}>
      <div>
        <button onClick={() => setTrigger(t => t + 1)}>Trigger Update</button>
        <Level1Profiled trigger={trigger} />
      </div>
    </Profiler>
  );
}

const Level1Profiled = memo(({ trigger }: { trigger: number }) => {
  const state1 = useMemo(() => trigger * 2, [trigger]);

  return (
    <Profiler id="Level1" onRender={onRenderCallback}>
      <Level2Profiled state={state1} />
    </Profiler>
  );
});

const Level2Profiled = memo(({ state }: { state: number }) => {
  const state2 = useMemo(() => state * 3, [state]);

  return (
    <Profiler id="Level2" onRender={onRenderCallback}>
      <Level3Profiled state={state2} />
    </Profiler>
  );
});

const Level3Profiled = memo(({ state }: { state: number }) => (
  <Profiler id="Level3" onRender={onRenderCallback}>
    <div>Final state: {state}</div>
  </Profiler>
));

// Proper cleanup to avoid memory leaks
export function NoMemoryLeak() {
  const [data, setData] = useState<any[]>([]);

  React.useEffect(() => {
    const interval = setInterval(() => {
      setData(prev => {
        // Ograniczamy rozmiar aby uniknąć memory leak
        const newData = [...prev, { id: Date.now(), value: Math.random() }];
        return newData.slice(-100); // Keep only last 100 items
      });
    }, 100);

    // Proper cleanup
    return () => clearInterval(interval);
  }, []);

  return (
    <Profiler id="DataMonitor" onRender={onRenderCallback}>
      <div>Data points: {data.length}</div>
    </Profiler>
  );
}

// Profiled różne scenariusze
export function ProfiledScenarios() {
  const [scenario, setScenario] = useState<'small' | 'medium' | 'large'>('small');

  const dataSize = {
    small: 10,
    medium: 100,
    large: 1000
  };

  const data = useMemo(
    () => Array.from({ length: dataSize[scenario] }, (_, i) => i),
    [scenario, dataSize]
  );

  return (
    <Profiler id={`Scenario-${scenario}`} onRender={onRenderCallback}>
      <div>
        <select value={scenario} onChange={(e) => setScenario(e.target.value as any)}>
          <option value="small">Small (10 items)</option>
          <option value="medium">Medium (100 items)</option>
          <option value="large">Large (1000 items)</option>
        </select>
        
        {data.map(item => (
          <OptimizedExpensiveItem key={item} value={item} />
        ))}
      </div>
    </Profiler>
  );
}

const OptimizedExpensiveItem = memo(({ value }: { value: number }) => {
  const result = useMemo(() => {
    return Array.from({ length: 100 }, () => Math.random()).reduce((a, b) => a + b);
  }, []);
  
  return <div>{value}: {result}</div>;
});

// Custom hook dla performance monitoring
export function usePerformanceMonitor(componentName: string) {
  const renderCount = React.useRef(0);
  const startTime = React.useRef(Date.now());

  React.useEffect(() => {
    renderCount.current++;
    const renderTime = Date.now() - startTime.current;
    
    console.log(`[Performance] ${componentName}`);
    console.log(`  Render count: ${renderCount.current}`);
    console.log(`  Time since mount: ${renderTime}ms`);
    
    startTime.current = Date.now();
  });

  return { renderCount: renderCount.current };
}

// Example using custom performance hook
export function MonitoredComponent() {
  const { renderCount } = usePerformanceMonitor('MonitoredComponent');
  const [count, setCount] = useState(0);

  return (
    <div>
      <p>Renders: {renderCount}</p>
      <p>Count: {count}</p>
      <button onClick={() => setCount(c => c + 1)}>Increment</button>
    </div>
  );
}
