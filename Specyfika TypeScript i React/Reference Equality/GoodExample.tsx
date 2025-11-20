import React, { useState, memo, useCallback, useMemo } from 'react';

interface User {
  id: number;
  name: string;
}

// Rozwiązanie: React.memo działa poprawnie z useCallback
const UserCard = memo(({ user, onEdit, onDelete }: {
  user: User;
  onEdit: (id: number) => void;
  onDelete: (id: number) => void;
}) => {
  console.log(`Rendering UserCard for ${user.name}`);
  return (
    <div>
      <h3>{user.name}</h3>
      <button onClick={() => onEdit(user.id)}>Edit</button>
      <button onClick={() => onDelete(user.id)}>Delete</button>
    </div>
  );
});

export function UserList() {
  const [users, setUsers] = useState<User[]>([
    { id: 1, name: 'Jan Kowalski' },
    { id: 2, name: 'Anna Nowak' },
    { id: 3, name: 'Piotr Wiśniewski' }
  ]);
  const [counter, setCounter] = useState(0);

  // useCallback zapewnia stabilną referencję funkcji
  const handleEdit = useCallback((id: number) => {
    console.log(`Editing user ${id}`);
  }, []); // brak dependencies - funkcja nigdy się nie zmieni

  const handleDelete = useCallback((id: number) => {
    setUsers(prevUsers => prevUsers.filter(u => u.id !== id));
  }, []); // używamy functional update - nie potrzebujemy users w dependencies

  // useMemo dla obiektów zapewnia stabilną referencję
  const config = useMemo(() => ({
    showActions: true,
    theme: 'light'
  }), []); // config nigdy się nie zmieni

  return (
    <div>
      <h2>User List</h2>
      <button onClick={() => setCounter(counter + 1)}>
        Increment Counter: {counter}
      </button>
      {/* Teraz kliknięcie przycisku nie powoduje re-render UserCard,
          bo handleEdit i handleDelete mają stabilne referencje */}
      {users.map(user => (
        <UserCard 
          key={user.id}
          user={user}
          onEdit={handleEdit}
          onDelete={handleDelete}
        />
      ))}
      <ConfigDisplay config={config} />
    </div>
  );
}

const ConfigDisplay = memo(({ config }: { config: { showActions: boolean; theme: string } }) => {
  console.log('Rendering ConfigDisplay');
  return (
    <div>
      Theme: {config.theme}, Actions: {config.showActions ? 'shown' : 'hidden'}
    </div>
  );
});

// Rozwiązanie: useCallback dla handler functions
export function TodoList() {
  const [todos, setTodos] = useState([
    { id: 1, text: 'Buy groceries', completed: false },
    { id: 2, text: 'Clean house', completed: true }
  ]);

  // Jedna stabilna funkcja zamiast wielu inline funkcji
  const handleToggle = useCallback((id: number) => {
    setTodos(prevTodos => prevTodos.map(t => 
      t.id === id ? { ...t, completed: !t.completed } : t
    ));
  }, []);

  return (
    <div>
      {todos.map(todo => (
        <TodoItem
          key={todo.id}
          todo={todo}
          onToggle={handleToggle}
        />
      ))}
    </div>
  );
}

// TodoItem przyjmuje id w onToggle zamiast inline funkcji
const TodoItem = memo(({ todo, onToggle }: {
  todo: { id: number; text: string; completed: boolean };
  onToggle: (id: number) => void;
}) => {
  console.log(`Rendering TodoItem: ${todo.text}`);
  return (
    <div>
      <input 
        type="checkbox" 
        checked={todo.completed}
        onChange={() => onToggle(todo.id)}
      />
      <span>{todo.text}</span>
    </div>
  );
});

// Alternatywne rozwiązanie: Custom comparison function dla complex objects
const ComplexUserCard = memo(({ 
  user, 
  metadata 
}: {
  user: User;
  metadata: { timestamp: Date; source: string };
}) => {
  console.log(`Rendering ComplexUserCard for ${user.name}`);
  return (
    <div>
      <h3>{user.name}</h3>
      <small>Source: {metadata.source}</small>
    </div>
  );
}, (prevProps, nextProps) => {
  // Custom comparison - porównujemy wartości zamiast referencji
  return (
    prevProps.user.id === nextProps.user.id &&
    prevProps.user.name === nextProps.user.name &&
    prevProps.metadata.source === nextProps.metadata.source
  );
});

export function ComplexUserList() {
  const [users] = useState<User[]>([
    { id: 1, name: 'Jan Kowalski' }
  ]);

  return (
    <div>
      {users.map(user => (
        <ComplexUserCard
          key={user.id}
          user={user}
          metadata={{ timestamp: new Date(), source: 'api' }}
        />
      ))}
    </div>
  );
}
