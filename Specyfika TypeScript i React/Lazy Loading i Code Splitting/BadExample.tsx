import React from 'react';
import HeavyComponent from './HeavyComponent';
import AnotherHeavyComponent from './AnotherHeavyComponent';

// ❌ BAD: Wszystko importowane eagerly

export function BadApp() {
  const [showHeavy, setShowHeavy] = React.useState(false);

  return (
    <div>
      <button onClick={() => setShowHeavy(true)}>Show Heavy</button>
      {showHeavy && <HeavyComponent />} {/* ❌ Already in bundle */}
      <AnotherHeavyComponent /> {/* ❌ Loaded even if never used */}
    </div>
  );
}
