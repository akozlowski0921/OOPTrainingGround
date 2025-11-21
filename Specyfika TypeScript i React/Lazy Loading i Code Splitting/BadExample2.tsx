import React from 'react';

// ❌ BAD: Brak Suspense boundary

const HeavyComponent = React.lazy(() => import('./HeavyComponent'));

export function BadNoSuspense() {
  return (
    <div>
      <HeavyComponent /> {/* ❌ Crash bez Suspense */}
    </div>
  );
}
