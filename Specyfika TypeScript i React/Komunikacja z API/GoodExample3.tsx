// ✅ GOOD: Custom hook dla API, optimistic updates, proper mutation handling
import { useState, useCallback, useEffect } from 'react';

interface Todo {
  id: number;
  title: string;
  completed: boolean;
}

// Custom hook dla zarządzania API calls
function useTodos() {
  const [todos, setTodos] = useState<Todo[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchTodos = useCallback(async () => {
    setLoading(true);
    setError(null);
    
    try {
      const response = await fetch('https://api.example.com/todos');
      if (!response.ok) throw new Error('Failed to fetch todos');
      const data = await response.json();
      setTodos(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unknown error');
    } finally {
      setLoading(false);
    }
  }, []);

  const addTodo = useCallback(async (title: string) => {
    try {
      const response = await fetch('https://api.example.com/todos', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ title, completed: false }),
      });
      
      if (!response.ok) throw new Error('Failed to add todo');
      const newTodo = await response.json();
      
      // Optimistic update - natychmiastowa aktualizacja UI
      setTodos(prev => [...prev, newTodo]);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to add todo');
      // W przypadku błędu możemy zrobić rollback
      throw err;
    }
  }, []);

  const toggleTodo = useCallback(async (id: number) => {
    const todo = todos.find(t => t.id === id);
    if (!todo) return;
    
    // Optimistic update
    setTodos(prev => 
      prev.map(t => t.id === id ? { ...t, completed: !t.completed } : t)
    );
    
    try {
      const response = await fetch(`https://api.example.com/todos/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ...todo, completed: !todo.completed }),
      });
      
      if (!response.ok) throw new Error('Failed to update todo');
    } catch (err) {
      // Rollback przy błędzie
      setTodos(prev => 
        prev.map(t => t.id === id ? todo : t)
      );
      setError(err instanceof Error ? err.message : 'Failed to update todo');
    }
  }, [todos]);

  useEffect(() => {
    fetchTodos();
  }, [fetchTodos]);

  return { todos, loading, error, addTodo, toggleTodo, refetch: fetchTodos };
}

// Komponent używający custom hooka
function TodoList() {
  const { todos, loading, error, addTodo, toggleTodo } = useTodos();

  if (loading) return <div>Loading todos...</div>;
  if (error) return <div>Error: {error}</div>;

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
