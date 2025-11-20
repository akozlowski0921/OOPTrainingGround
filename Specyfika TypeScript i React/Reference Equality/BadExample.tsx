import React, { useState, memo } from 'react';

interface User {
  id: number;
  name: string;
}

// Problem: Komponent zmemoizowany, ale otrzymuje nową referencję funkcji przy każdym renderze
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

// Problem: Niepotrzebne re-rendery przez nowe referencje funkcji i obiektów
export function UserList() {
  const [users, setUsers] = useState<User[]>([
    { id: 1, name: 'Jan Kowalski' },
    { id: 2, name: 'Anna Nowak' },
    { id: 3, name: 'Piotr Wiśniewski' }
  ]);
  const [counter, setCounter] = useState(0);

  // Problem: Te funkcje są tworzone na nowo przy każdym renderze
  const handleEdit = (id: number) => {
    console.log(`Editing user ${id}`);
  };

  const handleDelete = (id: number) => {
    setUsers(users.filter(u => u.id !== id));
  };

  // Problem: Nowy obiekt config jest tworzony przy każdym renderze
  const config = {
    showActions: true,
    theme: 'light'
  };

  return (
    <div>
      <h2>User List</h2>
      <button onClick={() => setCounter(counter + 1)}>
        Increment Counter: {counter}
      </button>
      {/* Problem: Każde kliknięcie przycisku powoduje re-render wszystkich UserCard,
          mimo że users się nie zmienili - bo handleEdit i handleDelete mają nowe referencje */}
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

// Problem: Komponent zmemoizowany otrzymuje nowy obiekt przy każdym renderze
const ConfigDisplay = memo(({ config }: { config: { showActions: boolean; theme: string } }) => {
  console.log('Rendering ConfigDisplay');
  return (
    <div>
      Theme: {config.theme}, Actions: {config.showActions ? 'shown' : 'hidden'}
    </div>
  );
});

// Problem: Inline funkcje w props powodują niepotrzebne re-rendery
export function TodoList() {
  const [todos, setTodos] = useState([
    { id: 1, text: 'Buy groceries', completed: false },
    { id: 2, text: 'Clean house', completed: true }
  ]);

  return (
    <div>
      {todos.map(todo => (
        <TodoItem
          key={todo.id}
          todo={todo}
          // Problem: Nowa funkcja tworzona dla każdego todo przy każdym renderze
          onToggle={() => {
            setTodos(todos.map(t => 
              t.id === todo.id ? { ...t, completed: !t.completed } : t
            ));
          }}
        />
      ))}
    </div>
  );
}

const TodoItem = memo(({ todo, onToggle }: {
  todo: { id: number; text: string; completed: boolean };
  onToggle: () => void;
}) => {
  console.log(`Rendering TodoItem: ${todo.text}`);
  return (
    <div>
      <input 
        type="checkbox" 
        checked={todo.completed}
        onChange={onToggle}
      />
      <span>{todo.text}</span>
    </div>
  );
});
