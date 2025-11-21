import React from 'react';

// ❌ BAD: Testowanie implementation details

export function BadCounter() {
  const [count, setCount] = React.useState(0);

  return (
    <div>
      <span className="count-value">{count}</span>
      <button onClick={() => setCount(count + 1)}>Increment</button>
    </div>
  );
}

// Test byłby:
// const el = container.querySelector('.count-value'); // ❌ Implementation detail
// expect(el.textContent).toBe('0');
