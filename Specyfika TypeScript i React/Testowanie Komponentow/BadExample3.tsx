import React from 'react';

// ❌ BAD: Async bez proper handling

export function BadAsyncComponent() {
  const [data, setData] = React.useState(null);

  React.useEffect(() => {
    fetch('/api/data')
      .then(r => r.json())
      .then(setData);
  }, []);

  if (!data) return <div>Loading...</div>;
  return <div>{data}</div>;
}

// Test bez await findBy będzie fail
