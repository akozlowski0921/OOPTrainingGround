// ❌ BAD: Brak obsługi błędów, loading state i proper error handling
import { useEffect, useState } from 'react';

interface User {
  id: number;
  name: string;
  email: string;
}

function UserList() {
  const [users, setUsers] = useState<User[]>([]);

  useEffect(() => {
    // Brak try-catch, brak loading state
    fetch('https://api.example.com/users')
      .then(res => res.json())
      .then(data => setUsers(data));
    // Brak obsługi błędów!
  }, []);

  // Brak loading state, brak error state
  return (
    <div>
      <h1>Users</h1>
      {users.map(user => (
        <div key={user.id}>
          {user.name} - {user.email}
        </div>
      ))}
    </div>
  );
}

export default UserList;
