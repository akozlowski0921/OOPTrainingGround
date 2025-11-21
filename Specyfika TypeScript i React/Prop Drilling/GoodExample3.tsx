import React, { createContext, useContext, useState } from 'react';

// ✅ GOOD: Using Context + custom hook for state management

interface Todo {
  id: number;
  text: string;
  completed: boolean;
}

interface TodoContextType {
  todos: Todo[];
  addTodo: (text: string) => void;
  toggleTodo: (id: number) => void;
  deleteTodo: (id: number) => void;
}

const TodoContext = createContext<TodoContextType | undefined>(undefined);

export function TodoApp() {
  const [todos, setTodos] = useState<Todo[]>([]);

  const contextValue: TodoContextType = {
    todos,
    addTodo: (text) => {
      setTodos([...todos, { id: Date.now(), text, completed: false }]);
    },
    toggleTodo: (id) => {
      setTodos(todos.map(t => t.id === id ? {...t, completed: !t.completed} : t));
    },
    deleteTodo: (id) => {
      setTodos(todos.filter(t => t.id !== id));
    }
  };

  return (
    <TodoContext.Provider value={contextValue}>
      <AppContainer />
    </TodoContext.Provider>
  );
}

function useTodos() {
  const context = useContext(TodoContext);
  if (!context) {
    throw new Error('useTodos must be used within TodoContext.Provider');
  }
  return context;
}

function AppContainer() {
  // Nie musi przyjmować props
  return (
    <div>
      <TodoForm />
      <TodoListContainer />
    </div>
  );
}

function TodoForm() {
  // Używa hooka bezpośrednio
  const { addTodo } = useTodos();
  const [input, setInput] = useState('');
  
  return (
    <div>
      <input value={input} onChange={e => setInput(e.target.value)} />
      <button onClick={() => { addTodo(input); setInput(''); }}>Add</button>
    </div>
  );
}

function TodoListContainer() {
  // Nie musi przekazywać props
  return (
    <div>
      <TodoList />
    </div>
  );
}

function TodoList() {
  // Używa hooka bezpośrednio
  const { todos } = useTodos();
  
  return (
    <ul>
      {todos.map(todo => (
        <TodoItem key={todo.id} todo={todo} />
      ))}
    </ul>
  );
}

function TodoItem({ todo }: { todo: Todo }) {
  // Używa hooka bezpośrednio
  const { toggleTodo, deleteTodo } = useTodos();
  
  return (
    <li>
      <input 
        type="checkbox" 
        checked={todo.completed}
        onChange={() => toggleTodo(todo.id)}
      />
      <span style={{ textDecoration: todo.completed ? 'line-through' : 'none' }}>
        {todo.text}
      </span>
      <button onClick={() => deleteTodo(todo.id)}>Delete</button>
    </li>
  );
}
