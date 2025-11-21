import React from 'react';

// ✅ GOOD: Test-friendly component

export function GoodCounter() {
  const [count, setCount] = React.useState(0);

  return (
    <div>
      <span data-testid="count">{count}</span>
      <button onClick={() => setCount(count + 1)}>Increment</button>
    </div>
  );
}

// Test example:
// import { render, screen } from '@testing-library/react';
// import userEvent from '@testing-library/user-event';
//
// test('increments counter', async () => {
//   render(<GoodCounter />);
//   const button = screen.getByRole('button', { name: /increment/i });
//   await userEvent.click(button);
//   expect(screen.getByTestId('count')).toHaveTextContent('1');
// });
