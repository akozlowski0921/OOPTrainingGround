import React from 'react';

// ❌ BAD: More typing anti-patterns

// BŁĄD 1: Brak generic constraints
function BadGenericComponent<T>({ items }: { items: T[] }) {
  return <div>{items.length}</div>; // ❌ T może nie mieć length
}

// BŁĄD 2: Type assertion zamiast proper typing
interface User {
  id: number;
  name: string;
}

function BadUserList({ users }: { users: any[] }) {
  return (
    <ul>
      {users.map(user => (
        <li key={(user as User).id}>{(user as User).name}</li>
        // ❌ Type assertion - brak safety
      ))}
    </ul>
  );
}

// BŁĄD 3: Optional props bez default values
interface BadButtonProps {
  label?: string; // ❌ Może być undefined
  onClick?: () => void;
}

function BadButton({ label, onClick }: BadButtonProps) {
  return <button onClick={onClick}>{label.toUpperCase()}</button>;
  // ❌ Crash gdy label jest undefined
}

// BŁĄD 4: Weak types
interface BadModalProps {
  isOpen: boolean;
  content: any; // ❌ any
  onClose: Function; // ❌ Function zamiast konkretnego typu
}
