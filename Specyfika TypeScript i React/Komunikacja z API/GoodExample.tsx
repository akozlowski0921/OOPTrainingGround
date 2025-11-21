// ✅ GOOD: Proper error handling, loading state, type-safe fetch wrapper
import { useEffect, useState } from 'react';

interface User {
  id: number;
  name: string;
  email: string;
}

type FetchState<T> = 
  | { status: 'idle' }
  | { status: 'loading' }
  | { status: 'success'; data: T }
  | { status: 'error'; error: string };

function UserList() {
  const [state, setState] = useState<FetchState<User[]>>({ status: 'idle' });

  useEffect(() => {
    setState({ status: 'loading' });

    fetch('https://api.example.com/users')
      .then(async (res) => {
        if (!res.ok) {
          throw new Error(`HTTP error! status: ${res.status}`);
        }
        return res.json();
      })
      .then((data) => setState({ status: 'success', data }))
      .catch((error) => 
        setState({ status: 'error', error: error.message })
      );
  }, []);

  if (state.status === 'loading') return <div>Loading users...</div>;
  if (state.status === 'error') return <div>Error: {state.error}</div>;
  if (state.status === 'idle' || (state.status === 'success' && !state.data)) {
    return <div>No data</div>;
  }

  return (
    <div>
      <h1>Users</h1>
      {state.data.map(user => (
        <div key={user.id}>
          {user.name} - {user.email}
        </div>
      ))}
    </div>
  );
}

export default UserList;
