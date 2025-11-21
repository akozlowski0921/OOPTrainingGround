# Zaawansowane Typowanie Komponentów

## React.FC Antywzorzec
**Problem:** Implicitly adds `children`, mylące dla komponentów bez children  
**Rozwiązanie:** Explicit props typing bez React.FC

## Discriminated Unions w State
```tsx
type State = 
  | { status: 'loading' }
  | { status: 'success'; data: T }
  | { status: 'error'; error: string };
```
**Korzyści:** Type narrowing, niemożliwe nieprawidłowe kombinacje

## Generic Components
```tsx
function List<T>({ items, renderItem }: ListProps<T>) { }
```
**Użycie:** Reusable components z type safety

## Children Typing
- `ReactNode` - any valid React child
- `ReactElement` - only React elements
- `ReactElement | ReactElement[]` - array of elements

## Best Practices
✅ Explicit props interface  
✅ Discriminated unions dla state  
✅ Generics dla reusable components  
✅ Proper event typing (`FormEvent`, `ChangeEvent`)  
✅ Utility types (`Omit`, `Pick`)  
✅ Conditional props (union types)
