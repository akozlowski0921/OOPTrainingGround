import React from 'react';

// ✅ GOOD: Testable async component

interface Data {
  message: string;
}

export function GoodAsyncComponent() {
  const [data, setData] = React.useState<Data | null>(null);
  const [loading, setLoading] = React.useState(true);

  React.useEffect(() => {
    fetch('/api/data')
      .then(r => r.json())
      .then((d: Data) => {
        setData(d);
        setLoading(false);
      });
  }, []);

  if (loading) return <div role="status">Loading...</div>;
  return <div role="main">{data?.message}</div>;
}

// Test example:
// test('loads and displays data', async () => {
//   global.fetch = jest.fn(() =>
//     Promise.resolve({
//       json: () => Promise.resolve({ message: 'Hello' })
//     })
//   ) as jest.Mock;
//
//   render(<GoodAsyncComponent />);
//   expect(screen.getByRole('status')).toHaveTextContent('Loading');
//   const content = await screen.findByRole('main');
//   expect(content).toHaveTextContent('Hello');
// });
