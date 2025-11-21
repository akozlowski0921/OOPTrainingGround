// ✅ GOOD: Reselect selectors, normalized state, memoization, performance optimized
import { createSelector } from '@reduxjs/toolkit';
import { useSelector } from 'react-redux';
import { useMemo } from 'react';

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

// Normalized state structure
interface AppState {
  todos: {
    byId: Record<number, Todo>;
    allIds: number[];
  };
  users: {
    byId: Record<number, User>;
    allIds: number[];
  };
  loading: boolean;
}

// Base selectors
const selectTodosById = (state: AppState) => state.todos.byId;
const selectTodoIds = (state: AppState) => state.todos.allIds;
const selectUsersById = (state: AppState) => state.users.byId;

// Memoized selector - wykonuje się tylko gdy zmieni się input
const selectAllTodos = createSelector(
  [selectTodosById, selectTodoIds],
  (todosById, allIds) => allIds.map(id => todosById[id])
);

// Selector z parametrem
const selectTodosByUser = createSelector(
  [selectAllTodos, (_state: AppState, userId: number) => userId],
  (todos, userId) => todos.filter(todo => todo.userId === userId)
);

// Złożony selector - memoizowany, oblicza się tylko gdy zmieni się input
const selectTodosWithUsers = createSelector(
  [selectTodosById, selectTodoIds, selectUsersById],
  (todosById, todoIds, usersById) =>
    todoIds.map(id => ({
      ...todosById[id],
      user: usersById[todosById[id].userId],
    }))
);

// Selector dla statystyk
const selectTodoStats = createSelector(
  [selectAllTodos],
  (todos) => {
    const completed = todos.filter(t => t.completed).length;
    const total = todos.length;
    return {
      total,
      completed,
      incomplete: total - completed,
      completionRate: total > 0 ? (completed / total) * 100 : 0,
    };
  }
);

// Komponent - używa selektora, rerenderuje się tylko gdy zmieni się output selektora
function TodoListWithUsers() {
  const todosWithUsers = useSelector(selectTodosWithUsers);

  // Dodatkowa memoizacja w komponencie jeśli potrzebna
  const completedTodos = useMemo(
    () => todosWithUsers.filter(t => t.completed),
    [todosWithUsers]
  );

  const incompleteTodos = useMemo(
    () => todosWithUsers.filter(t => !t.completed),
    [todosWithUsers]
  );

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

// Ten komponent rerenderuje się TYLKO gdy zmienią się statystyki
function TodoStats() {
  const stats = useSelector(selectTodoStats);
  
  return (
    <div>
      <p>Total: {stats.total}</p>
      <p>Completed: {stats.completed}</p>
      <p>Incomplete: {stats.incomplete}</p>
      <p>Completion Rate: {stats.completionRate.toFixed(1)}%</p>
    </div>
  );
}

// Selector z parametrem - użycie w komponencie
function UserTodos({ userId }: { userId: number }) {
  const userTodos = useSelector((state: AppState) => 
    selectTodosByUser(state, userId)
  );

  return (
    <div>
      <h3>Todos for user {userId}</h3>
      {userTodos.map(todo => (
        <div key={todo.id}>{todo.title}</div>
      ))}
    </div>
  );
}

export { TodoListWithUsers, TodoStats, UserTodos };
