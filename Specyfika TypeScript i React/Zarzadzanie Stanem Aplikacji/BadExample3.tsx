// ❌ BAD: Brak selektorów, re-renders całego drzewa, brak normalizacji danych
import { useSelector, useDispatch } from 'react-redux';
import { useEffect } from 'react';

interface Todo {
  id: number;
  title: string;
  completed: boolean;
  userId: number;
}

interface User {
  id: number;
  name: string;
}

interface AppState {
  todos: Todo[];
  users: User[];
  loading: boolean;
}

// Komponent pobiera cały state - rerenderuje się przy KAŻDEJ zmianie
function TodoListWithUsers() {
  const state = useSelector((state: AppState) => state); // Cały state!
  const dispatch = useDispatch();

  // Przetwarzanie danych w komponencie - wykonuje się przy każdym renderze
  const todosWithUsers = state.todos.map(todo => ({
    ...todo,
    user: state.users.find(u => u.id === todo.userId),
  }));

  // Filtrowanie w komponencie - brak memoizacji
  const completedTodos = todosWithUsers.filter(t => t.completed);
  const incompleteTodos = todosWithUsers.filter(t => !t.completed);

  return (
    <div>
      <h2>Completed ({completedTodos.length})</h2>
      {completedTodos.map(todo => (
        <div key={todo.id}>
          {todo.title} by {todo.user?.name}
        </div>
      ))}
      
      <h2>Incomplete ({incompleteTodos.length})</h2>
      {incompleteTodos.map(todo => (
        <div key={todo.id}>
          {todo.title} by {todo.user?.name}
        </div>
      ))}
    </div>
  );
}

// Inny komponent również pobiera cały state
function TodoStats() {
  const state = useSelector((state: AppState) => state); // Znowu cały state
  
  // Rerenderuje się nawet gdy zmieni się pojedyncze todo
  return (
    <div>
      Total: {state.todos.length}
    </div>
  );
}

export { TodoListWithUsers, TodoStats };
