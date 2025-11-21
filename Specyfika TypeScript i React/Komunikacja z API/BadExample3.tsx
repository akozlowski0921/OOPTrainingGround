// ❌ BAD: Brak automatycznego odświeżania danych po mutacji, duplikacja logiki
import { useState } from 'react';

interface Todo {
  id: number;
  title: string;
  completed: boolean;
}

function TodoList() {
  const [todos, setTodos] = useState<Todo[]>([]);

  const fetchTodos = async () => {
    const response = await fetch('https://api.example.com/todos');
    const data = await response.json();
    setTodos(data);
  };

  const addTodo = async (title: string) => {
    // Brak obsługi błędów, brak loading state
    await fetch('https://api.example.com/todos', {
      method: 'POST',
      body: JSON.stringify({ title }),
    });
    
    // Ręczne ponowne pobieranie danych - nie optymalne
    await fetchTodos();
  };

  const toggleTodo = async (id: number) => {
    const todo = todos.find(t => t.id === id);
    
    await fetch(`https://api.example.com/todos/${id}`, {
      method: 'PUT',
      body: JSON.stringify({ ...todo, completed: !todo?.completed }),
    });
    
    // Znowu ręczne pobieranie
    await fetchTodos();
  };

  return (
    <div>
      <button onClick={() => addTodo('New Todo')}>Add Todo</button>
      {todos.map(todo => (
        <div key={todo.id} onClick={() => toggleTodo(todo.id)}>
          {todo.title} - {todo.completed ? '✓' : '○'}
        </div>
      ))}
    </div>
  );
}

export default TodoList;
