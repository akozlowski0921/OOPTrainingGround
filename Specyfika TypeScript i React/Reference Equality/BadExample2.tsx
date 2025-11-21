import React, { useState, useCallback } from 'react';

// ❌ BAD: Creating new objects/functions on every render

interface Props {
  userId: number;
}

export function UserProfile({ userId }: Props) {
  const [userData, setUserData] = useState(null);

  // Problem: Nowy obiekt config przy każdym renderze
  const config = {
    apiUrl: 'https://api.example.com',
    timeout: 5000
  };

  // Problem: Nowa funkcja przy każdym renderze
  const fetchUser = () => {
    fetch(`${config.apiUrl}/users/${userId}`)
      .then(r => r.json())
      .then(setUserData);
  };

  // Problem: MemoizedComponent będzie re-renderować się niepotrzebnie
  return (
    <div>
      <MemoizedComponent config={config} onFetch={fetchUser} />
      <div>{userData ? JSON.stringify(userData) : 'No data'}</div>
    </div>
  );
}

const MemoizedComponent = React.memo(({ config, onFetch }: {
  config: { apiUrl: string; timeout: number };
  onFetch: () => void;
}) => {
  console.log('MemoizedComponent rendered!'); // Będzie renderować przy każdej zmianie parenta
  return <button onClick={onFetch}>Fetch</button>;
});
