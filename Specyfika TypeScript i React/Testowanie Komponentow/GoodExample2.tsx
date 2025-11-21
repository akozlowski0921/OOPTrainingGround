import React from 'react';

// ✅ GOOD: Accessible component

export function GoodForm() {
  return (
    <form aria-label="Contact form">
      <label htmlFor="email">Email:</label>
      <input id="email" name="email" type="email" />
      <button type="submit">Submit</button>
    </form>
  );
}

// Test example:
// test('submits form', async () => {
//   render(<GoodForm />);
//   const emailInput = screen.getByLabelText(/email/i);
//   const submitButton = screen.getByRole('button', { name: /submit/i });
//   
//   await userEvent.type(emailInput, 'test@example.com');
//   await userEvent.click(submitButton);
// });
