import React, { useState } from 'react';

// ❌ BAD: Passing callbacks through multiple levels

interface Todo {
  id: number;
  text: string;
  completed: boolean;
}

export function TodoApp() {
  const [todos, setTodos] = useState<Todo[]>([]);

  const addTodo = (text: string) => {
    setTodos([...todos, { id: Date.now(), text, completed: false }]);
  };

  const toggleTodo = (id: number) => {
    setTodos(todos.map(t => t.id === id ? {...t, completed: !t.completed} : t));
  };

  const deleteTodo = (id: number) => {
    setTodos(todos.filter(t => t.id !== id));
  };

  // Problem: Wszystkie callbacks przekazywane przez wiele poziomów
  return <AppContainer todos={todos} onAdd={addTodo} onToggle={toggleTodo} onDelete={deleteTodo} />;
}

function AppContainer({ todos, onAdd, onToggle, onDelete }: {
  todos: Todo[];
  onAdd: (text: string) => void;
  onToggle: (id: number) => void;
  onDelete: (id: number) => void;
}) {
  // AppContainer nie używa callbacks, tylko przekazuje
  return (
    <div>
      <TodoForm onAdd={onAdd} />
      <TodoListContainer todos={todos} onToggle={onToggle} onDelete={onDelete} />
    </div>
  );
}

function TodoForm({ onAdd }: { onAdd: (text: string) => void }) {
  const [input, setInput] = useState('');
  return (
    <div>
      <input value={input} onChange={e => setInput(e.target.value)} />
      <button onClick={() => { onAdd(input); setInput(''); }}>Add</button>
    </div>
  );
}

function TodoListContainer({ todos, onToggle, onDelete }: {
  todos: Todo[];
  onToggle: (id: number) => void;
  onDelete: (id: number) => void;
}) {
  // TodoListContainer przekazuje callbacks dalej
  return (
    <div>
      <TodoList todos={todos} onToggle={onToggle} onDelete={onDelete} />
    </div>
  );
}

function TodoList({ todos, onToggle, onDelete }: {
  todos: Todo[];
  onToggle: (id: number) => void;
  onDelete: (id: number) => void;
}) {
  // TodoList przekazuje callbacks do każdego TodoItem
  return (
    <ul>
      {todos.map(todo => (
        <TodoItem key={todo.id} todo={todo} onToggle={onToggle} onDelete={onDelete} />
      ))}
    </ul>
  );
}

function TodoItem({ todo, onToggle, onDelete }: {
  todo: Todo;
  onToggle: (id: number) => void;
  onDelete: (id: number) => void;
}) {
  return (
    <li>
      <input 
        type="checkbox" 
        checked={todo.completed}
        onChange={() => onToggle(todo.id)}
      />
      <span style={{ textDecoration: todo.completed ? 'line-through' : 'none' }}>
        {todo.text}
      </span>
      <button onClick={() => onDelete(todo.id)}>Delete</button>
    </li>
  );
}
