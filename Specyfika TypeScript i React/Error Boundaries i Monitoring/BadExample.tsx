import React, { useState } from 'react';

// ❌ BAD: No error boundaries, crashes break entire app

export function ComponentWithoutErrorBoundary() {
  const [count, setCount] = useState(0);

  // Problem: Error crashes całą aplikację
  if (count === 5) {
    throw new Error('Count reached 5!');
  }

  return (
    <div>
      <p>Count: {count}</p>
      <button onClick={() => setCount(count + 1)}>Increment</button>
    </div>
  );
}

// Problem: Brak error handling dla async operations
export function AsyncWithoutHandling() {
  const [data, setData] = useState<any>(null);

  const fetchData = async () => {
    // Problem: Unhandled promise rejection
    const response = await fetch('/api/data');
    const json = await response.json();
    setData(json);
  };

  return (
    <div>
      <button onClick={fetchData}>Fetch Data</button>
      <div>{data?.value}</div>
    </div>
  );
}
