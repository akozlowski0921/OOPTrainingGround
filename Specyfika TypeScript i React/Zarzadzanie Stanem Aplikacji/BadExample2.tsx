// ❌ BAD: Redux bez middleware, async logic w komponentach, brak type safety
import { createStore } from 'redux';
import { useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';

// Akcje bez type safety
const ADD_TODO = 'ADD_TODO';
const TOGGLE_TODO = 'TOGGLE_TODO';
const SET_TODOS = 'SET_TODOS';

// Reducer bez TypeScript typing
function todosReducer(state = { todos: [], loading: false }, action: any) {
  switch (action.type) {
    case ADD_TODO:
      return { ...state, todos: [...state.todos, action.payload] };
    case TOGGLE_TODO:
      return {
        ...state,
        todos: state.todos.map((t: any) =>
          t.id === action.payload ? { ...t, completed: !t.completed } : t
        ),
      };
    case SET_TODOS:
      return { ...state, todos: action.payload, loading: false };
    default:
      return state;
  }
}

const store = createStore(todosReducer);

// Komponent - async logic w komponencie zamiast w middleware
function TodoList() {
  const dispatch = useDispatch();
  const todos = useSelector((state: any) => state.todos); // any - brak type safety

  useEffect(() => {
    // Async logic w komponencie - nie da się reuse, testować
    fetch('https://api.example.com/todos')
      .then(res => res.json())
      .then(data => dispatch({ type: SET_TODOS, payload: data }));
  }, [dispatch]);

  const addTodo = async () => {
    // Async w event handlerze - nie da się łatwo testować
    const response = await fetch('https://api.example.com/todos', {
      method: 'POST',
      body: JSON.stringify({ title: 'New Todo' }),
    });
    const todo = await response.json();
    dispatch({ type: ADD_TODO, payload: todo });
  };

  return (
    <div>
      <button onClick={addTodo}>Add Todo</button>
      {todos.map((todo: any) => (
        <div key={todo.id}>{todo.title}</div>
      ))}
    </div>
  );
}

export default TodoList;
