import React from 'react';

// ❌ BAD: Brak accessibility w testach

export function BadForm() {
  return (
    <form>
      <div>Email:</div>
      <input name="email" /> {/* ❌ Brak label */}
      <button>Submit</button>
    </form>
  );
}

// Test byłby trudny bez proper labels
