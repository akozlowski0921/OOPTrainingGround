import React, { Suspense } from 'react';

// ✅ GOOD: React.lazy with Suspense

const HeavyComponent = React.lazy(() => import('./HeavyComponent'));
const AnotherHeavyComponent = React.lazy(() => import('./AnotherHeavyComponent'));

export function GoodApp() {
  const [showHeavy, setShowHeavy] = React.useState(false);

  return (
    <div>
      <button onClick={() => setShowHeavy(true)}>Show Heavy</button>
      {showHeavy && (
        <Suspense fallback={<div>Loading...</div>}>
          <HeavyComponent />
        </Suspense>
      )}
    </div>
  );
}
