# Testowanie Komponentów - React Testing Library

## Filozofia
Test components jak user ich używa, nie implementation details.

## Queries Priority
1. **getByRole** - accessibility first
2. **getByLabelText** - forms
3. **getByPlaceholderText** - inputs
4. **getByText** - non-interactive
5. **getByTestId** - last resort

## User Events
```tsx
import userEvent from '@testing-library/user-event';

await userEvent.click(button);
await userEvent.type(input, 'text');
```

## Mocking Fetch
```tsx
global.fetch = jest.fn(() =>
  Promise.resolve({
    json: () => Promise.resolve({ data: 'test' })
  })
);
```

## Best Practices
✅ Test behavior, not implementation  
✅ Use semantic queries (getByRole)  
✅ userEvent over fireEvent  
✅ await dla async operations  
✅ Mock external dependencies  
✅ Aim for high coverage (>80%)

## Example
```tsx
test('submits form', async () => {
  render(<Form />);
  const input = screen.getByLabelText('Email');
  const button = screen.getByRole('button', { name: /submit/i });
  
  await userEvent.type(input, 'test@example.com');
  await userEvent.click(button);
  
  expect(await screen.findByText('Success')).toBeInTheDocument();
});
```
