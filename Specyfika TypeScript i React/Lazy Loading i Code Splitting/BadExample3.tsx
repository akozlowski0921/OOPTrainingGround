import React from 'react';

// ❌ BAD: Inline dynamic imports bez lazy

export function BadInlineImport() {
  const [Component, setComponent] = React.useState<any>(null);

  const loadComponent = () => {
    import('./HeavyComponent').then(mod => {
      setComponent(() => mod.default); // ❌ Complex pattern
    });
  };

  return (
    <div>
      <button onClick={loadComponent}>Load</button>
      {Component && <Component />}
    </div>
  );
}
